using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ── Serilog ───────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((ctx, cfg) =>
        cfg.MinimumLevel.Information()
           .WriteTo.Console()
           .Enrich.FromLogContext());

    // ── YARP Reverse Proxy ────────────────────────────────────────────────────
    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    // ── Redis cache (optional — skips if not configured) ──────────────────────
    var redisConn = builder.Configuration.GetConnectionString("Redis");
    if (!string.IsNullOrEmpty(redisConn))
    {
        builder.Services.AddStackExchangeRedisCache(o => o.Configuration = redisConn);
    }

    // ── Swagger ───────────────────────────────────────────────────────────────
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "EHR API Gateway", Version = "v1" });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });
    });

    // ── JWT Authentication ────────────────────────────────────────────────────
    var jwtSecret = builder.Configuration["Jwt:Secret"]
        ?? Environment.GetEnvironmentVariable("JWT_SECRET")
        ?? throw new InvalidOperationException("JWT_SECRET is required");

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ehr-platform",
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ehr-api",
                ValidateLifetime = true
            };
        });

    builder.Services.AddAuthorization();

    // ── CORS ──────────────────────────────────────────────────────────────────
    builder.Services.AddCors(options =>
        options.AddPolicy("AllowAll", p =>
            p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

    // ── Rate limiting (built-in, .NET 7+) ────────────────────────────────────
    builder.Services.AddRateLimiter(options =>
    {
        options.AddFixedWindowLimiter("fixed", fwOptions =>
        {
            fwOptions.PermitLimit = 100;
            fwOptions.Window = TimeSpan.FromSeconds(60);
        });
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    });

    builder.Services.AddHealthChecks();

    // ── Build ─────────────────────────────────────────────────────────────────
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EHR API Gateway v1"));
    }

    app.UseSerilogRequestLogging();
    app.UseCors("AllowAll");
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapHealthChecks("/health");
    app.MapReverseProxy();

    Log.Information("EHR API Gateway starting");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API Gateway terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
