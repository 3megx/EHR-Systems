# Redis Caching Strategy - Task #6

## Overview

This document describes the comprehensive Redis caching strategy for the EHR Platform. Redis provides distributed caching with sub-millisecond latency, supporting thousands of concurrent users across microservices.

## Architecture

```
Client Request
    ↓
[API Controller]
    ↓
[MediatR Query Handler with CachingBehavior]
    ↓
[ICacheService.GetOrSetAsync()]
    ↓
    ├─→ [Redis] Cache HIT → Return cached value
    │
    └─→ [Database] Cache MISS → Load data → Cache result → Return
    
On Data Mutation (Command):
    ↓
[MediatR Command Handler]
    ↓
[Execute Business Logic]
    ↓
[Repository.Update()]
    ↓
[Publish IntegrationEvent]
    ↓
[CacheInvalidationEventHandler] → [RemoveByPattern()] → Clear affected caches
```

## Key Components

### 1. ICacheService Interface
**File**: `ICacheService.cs`

Core cache abstraction providing:
- `GetAsync<T>()` - Retrieve from cache
- `SetAsync<T>()` - Store in cache with TTL
- `RemoveAsync()` - Delete single or multiple keys
- `RemoveByPatternAsync()` - Pattern-based bulk delete
- `GetOrSetAsync<T>()` - Atomic get-or-load
- `GetManyAsync<T>()` - Batch retrieval (pipeline)
- `ExistsAsync()` - Key existence check
- `ExpireAsync()` - Extend/shorten TTL
- `GetTimeToLiveAsync()` - Check remaining TTL
- `GetStatisticsAsync()` - Monitoring data

### 2. RedisCacheService Implementation
**File**: `RedisCacheService.cs`

Production-grade Redis client using StackExchange.Redis:
- **Connection Pooling**: Reuses single IConnectionMultiplexer
- **JSON Serialization**: Transparent object serialization
- **Error Resilience**: Graceful degradation on cache failures
- **Pattern Scanning**: SCAN-based iteration for bulk operations (non-blocking)
- **Atomic Operations**: GetOrSetAsync prevents thundering herd
- **Health Monitoring**: Statistics collection for observability

### 3. Cache Key Generator
**File**: `CacheKeyGenerator.cs`

Consistent key naming patterns enabling bulk invalidation:

```
Patient Caches:
  patient:{id}                    → Single patient demographics
  patient:{id}:allergies          → Patient allergies
  patient:{id}:conditions         → Patient conditions
  patient:{id}:soapnotes          → Patient clinical notes
  patient:{id}:vitals             → Vital signs
  patient:{id}:diagnoses          → Diagnoses
  patients:list                   → All patients
  patients:paged:{page}:{size}    → Paginated list
  patients:search:{hash}:{p}:{s}  → Search results
  patient:*                       → Pattern: all patient caches

Appointment Caches:
  appointment:{id}                → Single appointment
  appointments:patient:{id}       → By patient
  appointments:doctor:{id}:{date} → By doctor/date

User Caches:
  user:{id}                       → User profile
  user:email:{hash}               → User by email
  user:{id}:roles                 → User roles
  user:{id}:permissions           → User permissions
```

### 4. TTL Policies
**File**: `CacheTTLPolicy.cs`

Adaptive caching strategies by data type:

| Data Type | TTL | Rationale |
|-----------|-----|-----------|
| Session | 1 min | Active user sessions, temporary |
| UserData | 5 min | User profile, standard cache |
| PatientData | 15 min | Demographics (stable), balance freshness |
| ClinicalData | 1 min | Vitals, notes (must be fresh for care) |
| AppointmentData | 15 min | Schedules (slower change rate) |
| MedicationData | 15 min | Prescriptions, drug lists |
| ReferenceData | 1 hour | ICD-10, CPT codes (rarely change) |
| Configuration | 6 hours | App settings, reference lookups |
| StaticData | 24 hours | Facilities, providers (stable) |

**Adaptive TTL** (result-count based):
```
1 result   → 1 hour    (specific query, cache longer)
2-10       → 15 min    (small result set)
11-50      → 5 min     (medium result set)
50+        → 1 min     (large result set, cache shorter)
```

### 5. Cache Invalidation
**File**: `CacheInvalidationEventHandler.cs`

Event-driven invalidation strategy:

```
Command Handler executes mutation
    ↓
Repository.Add/Update/Delete()
    ↓
UnitOfWork.SaveChangesAsync()
    ↓
Publishes IntegrationEvent (e.g., PatientUpdated)
    ↓
Kafka Topic (patient-events)
    ↓
CacheInvalidationEventHandler subscribes
    ↓
RemoveByPatternAsync(pattern)
    ↓
All related caches cleared
```

**Invalidation Rules**:

- **PatientCreated**: Invalidate `patients:*` (all patient lists affected)
- **PatientUpdated**: Invalidate `patient:{id}:*`, `patients:*`
- **PatientDeleted**: Invalidate `patient:{id}:*`, `patients:*`
- **SoapNoteCreated**: Invalidate `patient:{id}:soapnotes`, `patient:{id}:timeline`
- **VitalsUpdated**: Invalidate `patient:{id}:vitals`, `patient:{id}:clinical:*`
- **AppointmentScheduled**: Invalidate `appointments:*`, `appointment:*`
- **ReferenceDataUpdated**: Invalidate `ref:*`, all reference caches

### 6. Health Checks
**File**: `CacheHealthCheck.cs`

Automated health monitoring:

```csharp
// Test Redis connectivity
await cache.SetAsync("health-check", "ok", TimeSpan.FromSeconds(10));
var result = await cache.GetAsync("health-check");
// Measure latency, memory usage, key count
```

Returns health status with metrics:
- ✅ **Healthy** - Cache responsive, normal memory usage
- ⚠️ **Degraded** - Elevated latency but functional
- ❌ **Unhealthy** - Connection failed, timeout, or errors

## Configuration

### appsettings.json (Development)

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379,password=redis_password"
  },
  "Cache": {
    "Enabled": true,
    "DefaultDurationSeconds": 300,
    "MaxMemoryMB": 256,
    "EvictionPolicy": "allkeys-lru"
  },
  "HealthChecks": {
    "Enabled": true,
    "Tags": ["cache", "redis"]
  }
}
```

### .env.development

```bash
# Redis Cache
ConnectionStrings__Redis=localhost:6379,password=redis_password

# Optional: Redis configuration
Cache__MaxMemoryMB=256
Cache__EvictionPolicy=allkeys-lru
```

### Dependency Injection

```csharp
// In Program.cs
builder.Services.AddEHRCommon(options =>
{
    options.EnableCaching = true;
    options.RedisConnectionString = "localhost:6379,password=redis_password";
    options.DefaultCacheDurationSeconds = 300;
});
```

## Usage Patterns

### 1. Automatic Query Caching (Recommended)

```csharp
// Define query as cached
public class GetPatientQuery : IQuery<PatientDto>, ICachedQuery
{
    public Guid PatientId { get; set; }
    
    public string CacheKey => $"patient:{PatientId}";
    
    // 15-minute cache for patient data
    public TimeSpan? Duration => TimeSpan.FromMinutes(15);
}

// Handler executes normally; CachingBehavior handles caching
public class GetPatientQueryHandler : IQueryHandler<GetPatientQuery, PatientDto>
{
    private readonly IRepository<Patient> _repository;

    public async Task<PatientDto> Handle(GetPatientQuery request, CancellationToken ct)
    {
        var patient = await _repository.GetByIdAsync(request.PatientId, ct);
        return Mapper.Map<PatientDto>(patient);
    }
}

// Usage: MediatR automatically caches result
var patient = await mediator.Send(new GetPatientQuery { PatientId = id });
```

### 2. Manual Cache Operations

```csharp
public class PatientService
{
    private readonly ICacheService _cache;

    // Get or compute
    public async Task<PatientDto> GetPatientAsync(Guid id)
    {
        return await _cache.GetOrSetAsync(
            key: CacheKeyGenerator.PatientKey(id),
            factory: async ct => 
            {
                var patient = await _repository.GetByIdAsync(id, ct);
                return Mapper.Map<PatientDto>(patient);
            },
            expiration: TimeSpan.FromMinutes(15)
        );
    }

    // Batch operations with pipeline
    public async Task<Dictionary<Guid, PatientDto>> GetPatientsBatchAsync(
        IEnumerable<Guid> patientIds)
    {
        var keys = patientIds
            .Select(id => CacheKeyGenerator.PatientKey(id))
            .ToList();

        var cached = await _cache.GetManyAsync<PatientDto>(keys);
        
        // Load missing from database
        var missing = patientIds
            .Where(id => !cached.ContainsKey(CacheKeyGenerator.PatientKey(id)))
            .ToList();

        if (missing.Any())
        {
            var patients = await _repository.GetByIdsAsync(missing);
            foreach (var patient in patients)
            {
                await _cache.SetAsync(
                    CacheKeyGenerator.PatientKey(patient.Id),
                    Mapper.Map<PatientDto>(patient),
                    TimeSpan.FromMinutes(15)
                );
            }
        }

        return cached;
    }
}
```

### 3. Cache Invalidation in Commands

```csharp
public class UpdatePatientCommand : ICommand<PatientDto>
{
    public Guid PatientId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class UpdatePatientCommandHandler : ICommandHandler<UpdatePatientCommand, PatientDto>
{
    private readonly IRepository<Patient> _repository;
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    public async Task<PatientDto> Handle(UpdatePatientCommand request, CancellationToken ct)
    {
        var patient = await _repository.GetByIdAsync(request.PatientId, ct);
        patient.FirstName = request.FirstName;
        patient.LastName = request.LastName;

        _repository.Update(patient);
        await _uow.SaveChangesAsync(ct); // Publishes IntegrationEvent
        
        // Invalidate related caches
        await _cache.InvalidatePatientCacheAsync(
            request.PatientId,
            invalidateAllPatients: true  // Also clear patient lists
        );

        return Mapper.Map<PatientDto>(patient);
    }
}
```

### 4. Event-Driven Invalidation

```csharp
// In a separate service consuming Kafka events
public class PatientCacheInvalidationService : BackgroundService
{
    private readonly CacheInvalidationEventHandler _handler;
    private readonly IKafkaConsumer _consumer;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await _consumer.SubscribeAsync("patient-events", async message =>
        {
            if (message.Type == "PatientUpdated")
            {
                await _handler.HandlePatientEventAsync(
                    "PatientUpdated",
                    message.PatientId,
                    ct
                );
            }
        }, ct);
    }
}
```

## Performance Guidelines

### Cache Hit Ratio Targets

- **Queries**: 70-85% hit ratio (goal)
- **Lists**: 60-75% hit ratio (depends on update frequency)
- **Reference Data**: 95%+ hit ratio (stable data)

### Memory Management

```
For 100,000 users:
- User objects: ~1 KB each = 100 MB
- Sessions: 20% active = 20 MB
- Search results: Temporary = 50 MB
- Total: ~200 MB (well within typical Redis allocation)

Recommended Redis Memory:
- Development: 256 MB
- Staging: 1 GB
- Production: 4-8 GB (depends on patient volume)

Eviction Policy: allkeys-lru (remove least recently used when full)
```

### Latency Expectations

```
Redis Operations:
- Simple Get/Set: <1 ms
- GetOrSet (cache hit): <1 ms
- GetOrSet (cache miss + load): 50-200 ms (DB dependent)
- RemoveByPattern: 10-100 ms (depends on key count)
- Health check: <10 ms
```

## Monitoring & Observability

### Health Check Endpoint

```csharp
app.MapHealthChecks("/health/cache", new HealthCheckOptions
{
    Predicate = hc => hc.Tags.Contains("cache"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

Returns:
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0523451",
  "entries": {
    "Redis Cache": {
      "data": {
        "KeyCount": 45230,
        "MemoryUsedBytes": 167772160,
        "HitRate": 0.78
      },
      "duration": "00:00:00.0423451",
      "status": "Healthy",
      "description": "Cache is healthy. Keys: 45230, Memory: 160.00 MB"
    }
  }
}
```

### Metrics to Monitor (Serilog)

```csharp
logger.LogDebug("Cache HIT for key: {CacheKey}", cacheKey);
logger.LogDebug("Cache MISS for key: {CacheKey}", cacheKey);
logger.LogWarning("Cache read error for key {CacheKey}", cacheKey);
logger.LogInformation("Cache invalidation completed for event {EventId}. Total entries removed: {Count}");
```

## HIPAA Compliance Notes

1. **No PII Encryption at Rest**: Cache may contain Patient data but Redis stores unencrypted in memory
   - **Mitigation**: Use Redis persistence encryption, network isolation, or data masking
   - **For Production**: Use Redis 6.0+ with ACLs and Encryption at Rest (Enterprise)

2. **Access Logging**: All cache operations are logged (see Serilog configuration)

3. **Data Retention**: TTL ensures data is automatically cleared

4. **Audit Trail**: CacheInvalidationEventHandler logs all invalidations

5. **Compliance Check**: Review cache contents before audit

## Troubleshooting

### Cache Misses High

**Symptoms**: Performance poor despite caching enabled

**Solutions**:
1. Check TTL is not too short
2. Verify cache key generation is consistent
3. Check Redis memory availability (may be evicting)
4. Monitor RemoveByPattern frequency (too aggressive invalidation?)
5. Review query patterns (are queries using same parameters?)

### Redis Connection Errors

**Symptoms**: Cache operations timeout or fail

**Solutions**:
1. Verify Redis is running: `redis-cli ping`
2. Check connection string format: `localhost:6379,password=yourpwd`
3. Verify network connectivity: `telnet localhost 6379`
4. Check Redis logs: `docker logs redis`

### Memory Leaks

**Symptoms**: Redis memory grows unbounded

**Solutions**:
1. Verify TTL is set on all keys
2. Check for unbounded key patterns
3. Monitor top keys: `redis-cli --bigkeys`
4. Review GetOrSetAsync calls (ensure expiration is set)

## Redis Configuration (Production)

```bash
# redis.conf
maxmemory 4gb
maxmemory-policy allkeys-lru

# Persistence (optional, trades speed for durability)
save 900 1  # Save every 15 min if 1+ keys changed
save 300 10
save 60 10000

# Replication (for HA)
# Configure on replica Redis instances
replicaof <master-host> <master-port>

# Sentinel (automatic failover)
# Configure in sentinel.conf for HA
```

## Next Steps

- **Task #7**: Elasticsearch Integration (full-text search)
- **Task #8**: Outbox Pattern (guaranteed event delivery)
- **Task #9**: Kafka Consumer/Producer
- **Task #10**: Docker Compose (Redis + infrastructure)

## References

- [StackExchange.Redis Documentation](https://stackexchange.github.io/StackExchange.Redis/)
- [Redis Commands](https://redis.io/commands/)
- [Redis Best Practices](https://redis.io/docs/management/optimization/)
- [HIPAA Cache Considerations](https://hipaa.cyber.gov)
