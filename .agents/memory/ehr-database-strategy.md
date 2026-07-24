---
name: EHR polyglot database strategy
description: Five-store database architecture; which store each service uses; how optional stores are registered; graceful degradation pattern.
---

## The five stores

| Store         | Package (Common.csproj)                  | DI helper                                      | Use for                                      |
|---------------|------------------------------------------|------------------------------------------------|----------------------------------------------|
| PostgreSQL    | `Npgsql.EntityFrameworkCore.PostgreSQL`  | `AddPostgresDataAccess<TCtx>(connStr)`         | All relational data — patients, users, etc.  |
| Redis         | `StackExchange.Redis`                    | `AddRedisCaching(connStr)`                     | Cache-Aside, sessions, pub/sub invalidation  |
| Elasticsearch | `Elastic.Clients.Elasticsearch`          | `AddElasticsearchSearch(url)`                  | Full-text search, audit log queries          |
| MongoDB       | `MongoDB.Driver 2.24.0`                  | `AddMongoDataAccess(connStr, dbName)`          | Clinical notes, device vitals, audit logs    |
| MySQL         | (per-service, not in Common)             | `AddDbContext<T>` with Pomelo/MySql provider   | Billing/claims, legacy insurance integration |

## Graceful degradation pattern

Optional stores (Redis, Elasticsearch, MongoDB) are wrapped in `try/catch` in Program.cs.
On failure, log a Warning and continue — service starts without that capability.
Only PostgreSQL is required (will throw at startup if missing).

**Why:** Replit only provisions PostgreSQL natively. All other stores require docker-compose or external services. The services must start on Replit without them.

**How to apply:** Every new service should follow the same try/catch pattern from Identity/Patient Program.cs.

## Connection string for PostgreSQL on Replit

The `BuildConnectionString()` helper in each service:
1. Checks `ConnectionStrings:DefaultConnection` first.
2. Falls back to `PGHOST` / `PGPORT` / `PGDATABASE` / `PGUSER` / `PGPASSWORD` env vars (auto-set by Replit).
3. Sets `SSL Mode=Disable` for local hosts, `SSL Mode=Require;Trust Server Certificate=true` for external hosts.

## OutboxEvent in service DbContexts

PatientContext (and all service contexts) must include `DbSet<OutboxEvent> OutboxEvents`.
The OutboxRepository uses `DbContext.Set<OutboxEvent>()` — needs the DbSet in scope.

## MongoDB base document

`MongoBaseDocument` (Common) provides: `Id` (string/GUID), `EntityId` (Guid linking to PG), `TenantId`, `CreatedAt`, `UpdatedAt`, `DeletedAt` (soft delete), `SchemaVersion`.
Collection name: auto-derived as kebab-case plural (`ClinicalNote` → `clinical-notes`).

## Health check pattern

Each optional store gets a typed health check registered alongside its DI wiring:
- `CacheHealthCheck` for Redis (SET/GET/DELETE round-trip)
- `ElasticsearchHealthCheck` for ES (cluster ping)
- `MongoHealthCheck` for Mongo (db ping command)

`HealthStatus` in `ElasticsearchHealthCheck.cs` must be aliased: both `Elastic.Clients.Elasticsearch` and `Microsoft.Extensions.Diagnostics.HealthChecks` define it.
