using Yarp.ReverseProxy.Configuration;
using Serilog;
using EHRPlatform.Common.Extensions;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Configure Serilog
builder.Services.AddSerilogLogging();

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add services
builder.Services
    // Cache for gateway routing decisions
    .AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
    })
    
    // API documentation
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme"
        });
        
        options.OperationFilter<SecurityRequirementsOperationFilter>();
    })
    
    // Authentication
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT secret not found");
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", fwOptions =>
    {
        fwOptions.PermitLimit = 100;
        fwOptions.Window = TimeSpan.FromSeconds(60);
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Build app
var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseCors("AllowAll");
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Health check
app.MapHealthChecks("/health");

// YARP routes (configured in appsettings.json)
app.MapReverseProxy();

app.Run();
