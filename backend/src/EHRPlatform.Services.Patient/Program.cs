using EHRPlatform.Common.Extensions;
using EHRPlatform.Services.Patient.Data;
using EHRPlatform.Services.Patient.Messaging.Consumers;
using EHRPlatform.Services.Patient.Sagas;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Logging ──────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, config) =>
    config.ReadFrom.Configuration(ctx.Configuration));

// ── Controllers & Swagger ─────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── Database ──────────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<PatientContext>(options =>
    options.UseNpgsql(
        connectionString,
        b => b.MigrationsAssembly("EHRPlatform.Services.Patient")));

// ── CQRS (MediatR + FluentValidation + pipeline behaviors) ───────────────────
builder.Services.AddCQRSFromCurrentAssembly();

// ── Redis Cache ───────────────────────────────────────────────────────────────
builder.Services.AddEHRCommon(builder.Configuration);

// ── Kafka raw publisher + resilience decorator (outbox uses this) ─────────────
var kafkaServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
builder.Services.AddKafkaMessaging(kafkaServers, builder.Environment.EnvironmentName);
builder.Services.AddResilientEventPublisher();

// ── MassTransit: Kafka (domain events) + RabbitMQ (background jobs + saga) ───
builder.Services.AddMassTransitHybrid(
    builder.Configuration,
    configureRabbitMqConsumers: x =>
    {
        // RabbitMQ consumers
        x.AddConsumer<WelcomeNotificationConsumer>();
        x.AddConsumer<PatientIndexConsumer>();

        // Saga
        x.AddSagaStateMachine<PatientRegistrationSaga, PatientRegistrationSagaState>()
            .EntityFrameworkRepository(r =>
            {
                r.ExistingDbContext<PatientContext>();
                r.UsePostgres();
            });
    },
    configureKafkaRider: rider =>
    {
        // Kafka consumer for PatientCreatedEvent (self-subscription for side effects)
        rider.AddConsumer<PatientCreatedKafkaConsumer>();

        rider.AddProducer<EHRPlatform.Services.Patient.Domain.Events.PatientCreatedEvent>(
            "patient-created-event");
    });

// ── OpenTelemetry ─────────────────────────────────────────────────────────────
builder.Services.AddEHRTelemetry(builder.Configuration, "patient-service");

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT secret not configured");
builder.Services.AddJwtAuthentication(jwtSecret);

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ── Health Checks ─────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddDbContextCheck<PatientContext>();

// ── Build ─────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Migrations ────────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PatientContext>();
    await db.Database.MigrateAsync();
}

// ── Pipeline ──────────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

await app.RunAsync();
