using EHRPlatform.Common.Extensions;
using EHRPlatform.Services.Prescription;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Logging
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<PrescriptionContext>(options =>
    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("EHRPlatform.Services.Prescription")));

// CQRS, Caching, Search, Messaging
builder.Services.AddCommonServices(builder.Configuration);

// Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT secret not configured");
builder.Services.AddJwtAuthentication(jwtSecret);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PrescriptionContext>();
    await db.Database.MigrateAsync();
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
