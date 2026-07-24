# Modern EHR Platform

A production-ready, enterprise-grade Electronic Health Records system built with ASP.NET Core microservices and Angular 18.

## Stack

- **Backend**: ASP.NET Core (.NET 8), EF Core, PostgreSQL, MediatR (CQRS), FluentValidation, Serilog, Mapster
- **Frontend**: Angular 18, Tailwind CSS (separate `frontend/` directory — not yet wired to a workflow)
- **Polyglot DB**: PostgreSQL (primary OLTP), Redis (caching), Elasticsearch (search), MongoDB (documents)

## Project Structure

```
backend/
  src/
    EHRPlatform.Common/              # Shared: CQRS, repository, UoW, domain events, security,
                                     #   caching (Redis), search (Elasticsearch), MongoDB repository
    EHRPlatform.Services.Identity/   # Auth microservice (running on port 5000)
    EHRPlatform.Services.Patient/    # Patient microservice (foundation + DB strategy applied)
    EHRPlatform.Services.Clinical/   # Clinical microservice (foundation only)
    ... (8 more microservices — foundation scaffolded)
  docker-compose.yml                 # Full infrastructure: Postgres×4, MongoDB, MySQL, Redis,
                                     #   Kafka, RabbitMQ, Elasticsearch, Kibana
  init-scripts/mongo-init.js         # MongoDB collection + index initialization
frontend/                            # Angular 18 app (complete UI, not yet running on Replit)
docs/                                # Architecture, API spec, security, DB schema
```

## Running the Identity Service

The workflow **Identity Service** runs the backend auth service:

```
cd backend && dotnet run --project src/EHRPlatform.Services.Identity
```

- Starts on **port 5000**
- Swagger UI: `/` (root redirects to Swagger)
- Health check: `/health`
- On first start: automatically creates all DB tables and seeds default roles

## Required Environment Variables

| Variable                    | Purpose                                       |
|-----------------------------|-----------------------------------------------|
| `JWT_SECRET`                | Signs JWT access tokens (64-char hex)         |
| `ENCRYPTION_KEY`            | AES-256 key for PHI encryption (32 chars)     |
| `PGHOST` / `PGDATABASE` etc.| Auto-provided by Replit managed PostgreSQL    |
| `REDIS_CONNECTION_STRING`   | Optional — Redis caching (graceful fallback)  |
| `ELASTICSEARCH_URL`         | Optional — Elasticsearch search               |
| `MONGODB_CONNECTION_STRING` | Optional — MongoDB document store             |

## Database Architecture (Polyglot Persistence)

| Store         | Role                                                       | Services          |
|---------------|------------------------------------------------------------|-------------------|
| PostgreSQL    | Primary OLTP — patients, users, clinical records, billing  | All services      |
| Redis         | Caching, sessions, rate limiting, pub/sub invalidation     | All services      |
| Elasticsearch | Full-text patient/record/medication search, audit queries  | Patient, Clinical |
| MongoDB       | Clinical notes, device vitals, scanned docs, audit logs    | Clinical, Patient |
| MySQL         | Billing/claims integration, legacy insurance systems       | Billing service   |

## Common Library — Key Abstractions

- `IRepository<TEntity>` / `Repository<TEntity>` — EF Core generic CRUD with soft delete
- `IMongoRepository<TDocument>` / `MongoRepository<TDocument>` — MongoDB CRUD with soft delete
- `MongoBaseDocument` — base class for MongoDB documents (schemaVersion, soft-delete, tenantId)
- `IUnitOfWork` — EF Core transaction management + outbox event publishing
- `ICacheService` / `RedisCacheService` — Redis Cache-Aside pattern
- `ISearchService` / `ElasticsearchService` — full-text search with medical synonym analyzer
- `BaseDbContext` — soft delete, audit interceptors, timestamp management
- `OutboxEvent` — transactional outbox pattern for guaranteed Kafka/RabbitMQ delivery

## DI Registration Helpers (Program.cs)

```csharp
services.AddPostgresDataAccess<MyContext>(connectionString);   // EF Core + Dapper + UoW
services.AddRedisCaching(redisConnectionString);               // Redis ICacheService
services.AddElasticsearchSearch(elasticsearchUrl);             // Elasticsearch ISearchService
services.AddMongoDataAccess(mongoConnectionString, dbName);    // MongoDB IMongoRepository<T>
```

All optional stores (Redis, Elasticsearch, MongoDB) degrade gracefully — the service logs a warning and continues without them when they're unavailable.

## Health Checks

Each service exposes `/health` with checks for every configured store:
- `postgres-<service>` — EF Core DbContext check
- `redis-<service>` — Cache SET/GET/DELETE round-trip
- `elasticsearch-<service>` — Cluster ping
- `mongodb-<service>` — Database ping command

## Identity Service — Endpoints

| Method | Path                     | Auth   | Description                 |
|--------|--------------------------|--------|-----------------------------|
| POST   | /api/v1/auth/register    | None   | Self-register a new user    |
| POST   | /api/v1/auth/login       | None   | Login → JWT + refresh token |
| POST   | /api/v1/auth/refresh     | None   | Refresh access token        |
| POST   | /api/v1/auth/logout      | Bearer | Revoke refresh token        |

## User Preferences

- Deep, thorough implementation preferred — clean up duplicates and legacy patterns
- Backend-first focus; Identity Service is the running anchor service
- Follow polyglot database strategy from `attached_assets/` spec document
