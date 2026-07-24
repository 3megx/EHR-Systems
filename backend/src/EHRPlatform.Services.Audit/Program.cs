using EHRPlatform.Common.Extensions;
using EHRPlatform.Common.Health;
using EHRPlatform.Services.Audit.Data;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ── Logging ───────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((ctx, cfg) =>
        cfg.MinimumLevel.Information()
           .WriteTo.Console()
           .Enrich.FromLogContext());

    // ── Controllers & Swagger ─────────────────────────────────────────────────
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // ── Database (PostgreSQL) ─────────────────────────────────────────────────
    var connectionString = BuildConnectionString(builder.Configuration);
    builder.Services.AddPostgresDataAccess<AuditContext>(connectionString);

    // ── CQRS ─────────────────────────────────────────────────────────────────
    builder.Services.AddCQRSFromCurrentAssembly();

    // ── Redis Caching (optional) ──────────────────────────────────────────────
    var redisConnStr = builder.Configuration["ConnectionStrings:Redis"]
        ?? builder.Configuration["Redis:ConnectionString"]
        ?? Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
    if (!string.IsNullOrEmpty(redisConnStr))
    {
        try { builder.Services.AddRedisCaching(redisConnStr); }
        catch (Exception ex) { Log.Warning(ex, "Redis not available for Audit Service"); }
    }

    // ── Elasticsearch (optional — used for audit log search) ─────────────────
    var esNodes = builder.Configuration.GetSection("Elasticsearch:Nodes").Get<string[]>();
    var esUrl = (esNodes?.FirstOrDefault())
        ?? builder.Configuration["Elasticsearch:Url"]
        ?? Environment.GetEnvironmentVariable("ELASTICSEARCH_URL");
    if (!string.IsNullOrEmpty(esUrl))
    {
        try { builder.Services.AddElasticsearchSearch(esUrl); }
        catch (Exception ex) { Log.Warning(ex, "Elasticsearch not available for Audit Service"); }
    }

    // ── JWT Authentication ────────────────────────────────────────────────────
    var jwtSecret = builder.Configuration["Jwt:Secret"]
        ?? Environment.GetEnvironmentVariable("JWT_SECRET")
        ?? throw new InvalidOperationException("JWT_SECRET is required");
    builder.Services.AddJwtAuthentication(jwtSecret);

    // ── CORS ──────────────────────────────────────────────────────────────────
    builder.Services.AddCors(options =>
        options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

    // ── Health Checks ─────────────────────────────────────────────────────────
    var healthBuilder = builder.Services.AddHealthChecks()
        .AddDbContextCheck<AuditContext>("postgres-audit", tags: ["db", "postgres"]);
    if (!string.IsNullOrEmpty(redisConnStr))
        healthBuilder.AddCacheHealthCheck("redis-audit");
    if (!string.IsNullOrEmpty(esUrl))
        healthBuilder.AddElasticsearchHealthCheck("elasticsearch-audit");

    // ── Build ─────────────────────────────────────────────────────────────────
    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

    // ── Schema ────────────────────────────────────────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AuditContext>();
        await db.Database.EnsureCreatedAsync();
        Log.Information("Audit database schema verified/created");
    }

    app.UseSerilogRequestLogging();
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHealthChecks("/health");

    Log.Information("EHR Audit Service starting");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Audit Service terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

static string BuildConnectionString(IConfiguration config)
{
    var explicit_ = config.GetConnectionString("AuditDb")
        ?? config.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(explicit_) && !explicit_.Contains("localhost")) return explicit_;

    var host = Environment.GetEnvironmentVariable("PGHOST");
    var port = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";
    var db   = Environment.GetEnvironmentVariable("PGDATABASE");
    var user = Environment.GetEnvironmentVariable("PGUSER");
    var pass = Environment.GetEnvironmentVariable("PGPASSWORD");

    if (!string.IsNullOrEmpty(host))
    {
        var ssl = host.Contains('.') ? "SSL Mode=Require;Trust Server Certificate=true;" : "SSL Mode=Disable;";
        return $"Host={host};Port={port};Database={db};Username={user};Password={pass};{ssl}";
    }

    if (!string.IsNullOrEmpty(explicit_)) return explicit_;
    throw new InvalidOperationException("Database connection not configured. Set PGHOST or ConnectionStrings__DefaultConnection.");
}
