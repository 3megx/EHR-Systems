using EHRPlatform.Common.Data;
using EHRPlatform.Common.Caching;
using EHRPlatform.Common.Search;
using EHRPlatform.Common.Messaging;
using EHRPlatform.Services.Audit;
using EHRPlatform.Services.Audit.Features.Audit.Commands;
using EHRPlatform.Services.Audit.Features.Audit.Queries;
using Serilog;
using Microsoft.EntityFrameworkCore;
using MediatR;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/audit-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Services
builder.Services.AddScoped<AuditContext>();
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork<AuditContext>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// CQRS
builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<RecordAuditEntryCommand>());

// Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Database
builder.Services.AddDbContext<AuditContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("AuditDb"),
        x => x.MigrationsAssembly("EHRPlatform.Services.Audit")));

// Elasticsearch
var elasticOptions = builder.Configuration.GetSection("Elasticsearch").Get<ElasticsearchOptions>()
    ?? throw new InvalidOperationException("Elasticsearch configuration not found");
builder.Services.AddSingleton(elasticOptions);
builder.Services.AddScoped<ISearchService, ElasticsearchService>();

// Kafka
var kafkaOptions = builder.Configuration.GetSection("Kafka").Get<KafkaOptions>()
    ?? throw new InvalidOperationException("Kafka configuration not found");
builder.Services.AddSingleton(kafkaOptions);

// API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Audit Service API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Type = "http",
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter JWT token"
    });
    c.AddSecurityRequirement(new() { { new() { Reference = new() { Type = 0, Id = "Bearer" } }, new string[] { } } });
});

// CORS
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Auth
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = "audit-service";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    await context.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
