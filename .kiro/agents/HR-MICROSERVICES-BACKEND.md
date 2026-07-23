# Build Enterprise-Grade ASP.NET Core Microservices Backend for EHR System

**Project**: Electronic Health Records (EHR) Platform Backend  
**Frontend Reference**: The Angular EHR app being built in parallel.  
**Goal**: Create a **highly secure, scalable, HIPAA-compliant** microservices backend using .NET 8/9 that can support thousands of users (hospitals, clinics, doctors).

## Core Architecture (Large-Scale Microservices)

### 1. Overall Structure

- **Multiple bounded-context microservices** (each owns its data).
- **API Gateway** (YARP preferred).
- **Shared Common Library** for cross-cutting concerns.
- **Event-Driven** communication via **Kafka**.

**Recommended Microservices**:
1. **Identity & Access Service** (Auth, RBAC, Consent Management)
2. **Patient Service** (Demographics, Master Patient Index)
3. **Clinical Service** (Visits, SOAP Notes, Diagnoses, Allergies)
4. **Appointment Service** (Scheduling, Calendar)
5. **Medication & Prescription Service** (eRx, Drug Interactions)
6. **Lab & Imaging Service** (Results, Orders, Integration)
7. **Billing & Claims Service**
8. **Analytics & Reporting Service** (Population health, Snowflake DW)
9. **Notification & Real-time Service** (SignalR + Kafka)
10. **Audit & Compliance Service** (Centralized audit log)

### 2. Mandatory Patterns & Technologies

- **CQRS** — MediatR + Vertical Slice Architecture (feature folders).
- **Validation** — FluentValidation + pipeline behaviors.
- **Repository + Unit of Work** — Generic Repo + UoW for EF Core.
- **Mapping** — Mapster (high performance) or AutoMapper.
- **Middleware**:
  - Serilog structured logging
  - Rate limiting (per user/tenant)
  - Correlation ID, Request/Response logging
  - Global exception handling + custom EHR exceptions
- **Audit Trail** — Full immutable audit log for every change (who, what, when, why) — critical for HIPAA.
- **Caching** — **Redis** (distributed cache for patients, lookups, sessions).
- **Search** — **Elasticsearch** for full-text search across records, patients, notes.
- **Databases**:
  - **PostgreSQL** per service (OLTP) — EF Core (80% of code) for CRUD/relationships.
  - **Dapper** for complex queries, reporting, bulk operations, stored procedures.
  - **Snowflake** (Data Warehouse) — Denormalized analytics via CDC + ETL.
- **Messaging** — **Kafka** (events + Outbox Pattern + Sagas for distributed transactions).
- **Real-time** — SignalR integrated with Kafka for live updates (vitals, alerts).
- **Localization** — Multi-language support (EN + AR).
- **Security** — JWT + Policy-based RBAC, data encryption at rest/transit, audit, consent management, HIPAA patterns (access logging, minimum necessary data).

### 3. Cross-Cutting Concerns (Implement in Shared Library)

- AuditableEntity base class + automatic audit logging.
- Outbox Pattern for reliable event publishing.
- CDC (Change Data Capture) to push data to Snowflake.
- Cache invalidation strategy (Redis pub/sub on changes).
- Dead Letter Queue handling in Kafka.
- Health checks, OpenTelemetry tracing.
- Soft delete + data retention policies.

### 4. Development Standards for "Real Production App"

- Vertical Slice per feature (Commands/Queries/Handlers in one folder).
- Domain-Driven Design elements (Entities, Value Objects, Domain Events).
- Comprehensive validation, business rules.
- Unit + Integration tests (xUnit + Testcontainers for Postgres/Redis).
- Docker + docker-compose for all services + infrastructure (Postgres, Redis, Kafka, Elasticsearch, Kibana).
- Configuration via appsettings + environment variables + Azure Key Vault style secrets.
- API Versioning.
- OpenAPI/Swagger with proper security definitions.
- Performance: Compiled queries (EF), indexing strategy, pagination everywhere.

### 5. Checklist (Implement ALL)

- Each microservice owns its PostgreSQL database.
- No direct DB access between services — only via APIs + events.
- Full Outbox + Saga pattern.
- Centralized Audit Service or embedded audit in every write.
- Redis caching with intelligent invalidation.
- Elasticsearch for powerful search (patients, clinical notes, etc.).
- CDC to Snowflake for analytics/warehouse.
- Strong security & compliance focus (audit everything).
- Real-time capabilities via SignalR + Kafka.

### 6. Implementation Order

1. Create solution + **Common** shared library (entities, base classes, middleware, audit, etc.).
2. Set up infrastructure (Docker Compose with all external services).
3. Implement **Identity Service** fully.
4. Implement **Patient Service** as reference (with full audit, cache, ES indexing, CQRS).
5. Apply the same pattern to remaining services.

**Start Now**: 
- Create the full solution structure and show the folder tree.
- Implement the Common library first.
- Set up Docker Compose.
- Build Identity + Patient services with all required patterns (CQRS, Audit, Redis, ES, EF+Dapper).

Make this backend **strong, production-grade, and truly enterprise-ready** for a real hospital/clinic EHR system. Focus on security, performance, scalability, and maintainability. Begin step-by-step and provide high-quality, clean code.

---

## Project Status (As of July 2026)

**Current Completion**: 5/19 Tasks Complete (26%)  
**Backend Location**: `c:\Users\cw_14\Downloads\New folder (5)\backend\`

### ✅ Completed Tasks

1. **Task #1**: Solution Structure + 11 microservices scaffolded
2. **Task #2**: Common Shared Library (BaseEntity, CQRS, DomainException, Events)
3. **Task #3**: Audit & Logging Infrastructure (AuditableEntity, Serilog)
4. **Task #4**: CQRS Infrastructure (MediatR, Behaviors, Pipeline)
5. **Task #5**: Repository & Unit of Work (Generic Repo, EF Core, Soft Delete)

### 🔄 Next Priority Tasks

- **Task #6**: Redis Caching Strategy
- **Task #7**: Elasticsearch Integration
- **Task #8**: Outbox Pattern + Event Publishing
- **Task #9**: Kafka Consumer/Producer Implementation
- **Task #10**: Docker Compose (PostgreSQL, Redis, Kafka, Elasticsearch, Kibana)
- **Task #11**: API Gateway (YARP)
- **Task #12**: Identity Service (Auth, JWT, RBAC)
- **Task #13**: Patient Service (Reference Implementation)
- **Task #14**: Clinical Service
- **Task #15**: Appointment Service
- **Task #16**: Prescription Service
- **Task #17**: Billing Service
- **Task #18**: Notification Service
- **Task #19**: Analytics Service

---

## Usage Instructions

When calling this agent, reference the prompt to:
1. Continue building Task #6+ with full architectural compliance
2. Ensure all new services follow the proven patterns from Tasks 1-5
3. Maintain HIPAA security, audit trails, caching, search indexing
4. Generate production-ready code with comprehensive tests
5. Integrate with Docker Compose infrastructure
6. Provide clear file structure and verification steps
