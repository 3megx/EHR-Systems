// Template for microservice Program.cs
// Copy and customize for each microservice (Identity, Patient, Clinical, etc.)

using EHRPlatform.Common.Extensions;
using Serilog;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Load configuration
var patientDbConnection = builder.Configuration.GetConnectionString("PatientDb")
    ?? throw new InvalidOperationException("PatientDb connection string not found");
var redisConnection = builder.Configuration.GetConnectionString("Redis")
    ?? throw new InvalidOperationException("Redis connection string not found");
var elasticsearchUrl = builder.Configuration["Elasticsearch:Url"]
    ?? throw new InvalidOperationException("Elasticsearch URL not found");
var kafkaBootstrap = builder.Configuration["Kafka:BootstrapServers"]
    ?? throw new InvalidOperationException("Kafka bootstrap servers not found");

// Configure Serilog
builder.Services.AddSerilogLogging();

// Add services
builder.Services
    // Data Access (Task #5)
    .AddPostgresDataAccess<PatientContext>(patientDbConnection)
    
    // Caching (Task #6)
    .AddRedisCaching(redisConnection)
    
    // Search (Task #7)
    .AddElasticsearchSearch(elasticsearchUrl)
    
    // Messaging (Task #8-9)
    .AddKafkaMessaging(kafkaBootstrap)
    
    // CQRS (Task #4)
    .AddCQRS()
    
    // Mapping
    .AddMapster()
    
    // API
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Add API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter JWT token"
    });
    
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// Add authentication
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT secret not found");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policyBuilder =>
    {
        var origins = builder.Configuration.GetSection("Api:Cors:AllowedOrigins").Get<string[]>();
        policyBuilder
            .WithOrigins(origins ?? new[] { "http://localhost:4200" })
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
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
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// Add request logging
app.UseSerilogRequestLogging();

app.MapControllers();

// Add health checks
app.MapHealthChecks("/health");

// Run migrations at startup
using (var scope = app.Services.CreateScope())
{
    var migrator = scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>();
    await migrator.MigrateDatabaseAsync();
}

await app.RunAsync();
