using EHRPlatform.Common.Data;
using EHRPlatform.Common.Extensions;
using EHRPlatform.Common.Security;
using EHRPlatform.Services.Identity.Application.Identity.Extensions;
using EHRPlatform.Services.Identity.Data;
using EHRPlatform.Services.Identity.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

// Bootstrap logger to capture startup errors
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

    // ── Controllers & Swagger ─────────────────────────────────────────────────
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "EHR Identity Service", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter your JWT token"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id   = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // ── Database (Replit PostgreSQL) ──────────────────────────────────────────
    var connectionString = BuildConnectionString(builder.Configuration);
    builder.Services.AddPostgresDataAccess<IdentityContext>(connectionString);

    // ── CQRS: handlers, validators, mappers ──────────────────────────────────
    builder.Services.AddIdentityServices();

    // ── Security ─────────────────────────────────────────────────────────────
    var encryptionKey = builder.Configuration["Security:EncryptionKey"]
        ?? Environment.GetEnvironmentVariable("ENCRYPTION_KEY")
        ?? throw new InvalidOperationException("ENCRYPTION_KEY secret is required.");

    builder.Services.AddSingleton<IEncryptionService>(new EncryptionService(encryptionKey));
    builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

    var jwtSecret = builder.Configuration["Jwt:Secret"]
        ?? Environment.GetEnvironmentVariable("JWT_SECRET")
        ?? throw new InvalidOperationException("JWT_SECRET secret is required.");

    var jwtIssuer   = builder.Configuration["Jwt:Issuer"]   ?? "ehr-platform";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ehr-api";
    var jwtExpMin   = int.TryParse(builder.Configuration["Jwt:ExpirationMinutes"], out var m) ? m : 60;

    builder.Services.AddSingleton<IJwtTokenService>(
        new JwtTokenService(jwtSecret, jwtIssuer, jwtAudience, jwtExpMin));

    builder.Services.AddJwtAuthentication(jwtSecret, jwtIssuer, jwtAudience);

    // ── CORS ─────────────────────────────────────────────────────────────────
    builder.Services.AddCors(options =>
        options.AddPolicy("AllowAll", p =>
            p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

    // ── Health checks ─────────────────────────────────────────────────────────
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<IdentityContext>();

    // ── Kestrel: listen on port 5000 (Replit preview port) ───────────────────
    builder.WebHost.UseUrls("http://0.0.0.0:5000");

    // ── Build ─────────────────────────────────────────────────────────────────
    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EHR Identity Service v1"));

    app.UseSerilogRequestLogging();
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHealthChecks("/health");

    // ── Auto-create schema on first run (idempotent) ──────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        await db.Database.EnsureCreatedAsync();
        Log.Information("Database schema verified/created");
    }

    Log.Information("EHR Identity Service starting on http://0.0.0.0:5000");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Identity Service terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// ── Build Npgsql connection string from Replit PG* env vars ──────────────────
static string BuildConnectionString(IConfiguration config)
{
    var explicit_ = config.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(explicit_)) return explicit_;

    var host = Environment.GetEnvironmentVariable("PGHOST");
    var port = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";
    var db   = Environment.GetEnvironmentVariable("PGDATABASE");
    var user = Environment.GetEnvironmentVariable("PGUSER");
    var pass = Environment.GetEnvironmentVariable("PGPASSWORD");

    if (!string.IsNullOrEmpty(host))
        return $"Host={host};Port={port};Database={db};Username={user};Password={pass};" +
               "SSL Mode=Require;Trust Server Certificate=true";

    throw new InvalidOperationException(
        "Database connection not configured. Set ConnectionStrings__DefaultConnection or PGHOST.");
}
