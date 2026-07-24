# EHR Platform — Replit Project

## Project Overview

**Modern EHR Platform** is a full-stack Electronic Health Records system with:

- **Angular 18 Frontend** — complete, 150+ files, Tailwind CSS, NgRx state management
- **ASP.NET Core 8 Microservices Backend** — CQRS, Event-Driven Architecture, HIPAA compliance
- **Docker Compose** — full infrastructure stack (Kafka, RabbitMQ, PostgreSQL, Redis, Elasticsearch)

## Running on Replit

### Frontend Only (Angular)
The Angular frontend can run standalone without any backend services:

```bash
cd frontend
npm install
npm start   # serves on port 4200
```

### Full Stack (requires Docker)
The backend microservices depend on external services (PostgreSQL, Kafka, RabbitMQ, Redis, Elasticsearch) that require Docker:

```bash
# Start infrastructure
docker-compose up -d

# Start microservices
docker-compose -f docker-compose.yml -f docker-compose.services.yml up
```

> **Note**: Docker is not natively available on Replit. Use the Docker Compose files locally or deploy to a Docker-capable environment (Azure, AWS, GCP, on-premises).

## Architecture

### Communication Strategy
See [`docs/COMMUNICATION_STRATEGY.md`](docs/COMMUNICATION_STRATEGY.md) for the full inter-service communication design.

**Summary**:
- **Kafka** — primary domain event bus (PatientCreated, LabResultReady, AuditLog)
- **RabbitMQ** — background job queues (notifications, ES indexing, report generation)
- **MassTransit** — unified abstraction over both transports (sagas, retry, dead-letter)
- **SignalR** — real-time push from Kafka → Angular dashboard (lab results, alerts)
- **YARP** — API Gateway with JWT auth, rate limiting, health checks

### Service Ports
| Service | Port | Description |
|---------|------|-------------|
| API Gateway | 5000 | YARP reverse proxy, entry point |
| Identity | 5001 | JWT auth, user management |
| Patient | 5002 | Patient CRUD + Kafka events + Saga |
| Clinical | 5003 | Clinical records, lab orders |
| Notification | 5006 | SignalR hub + RabbitMQ consumer |
| Kafka UI | 8080 | Topic management |
| RabbitMQ UI | 15672 | Queue management (ehr_user / ehr_password) |
| Elasticsearch | 9200 | Full-text search |
| Kibana | 5601 | Log visualization |
| Frontend | 4200 | Angular dev server |

## Tech Stack

### Backend (ASP.NET Core 8)
- **MassTransit 8** — Kafka + RabbitMQ unified messaging
- **MediatR 12** — CQRS command/query pipeline
- **Polly 8** — retry, circuit breaker, timeout resilience policies
- **OpenTelemetry 1.7** — distributed tracing
- **Entity Framework Core 8** — PostgreSQL (Npgsql)
- **FluentValidation 11** — command validation
- **Serilog 3** — structured logging
- **Mapster 7** — object mapping
- **YARP 2** — reverse proxy / API gateway

### Frontend (Angular 18)
- **NgRx 18** — state management
- **PrimeNG 18** — UI component library
- **Tailwind CSS 3** — utility-first styling
- **FullCalendar 6** — appointment scheduling
- **Chart.js 4** — analytics dashboards

## Key Design Decisions

### Outbox Pattern
All events are written to an outbox table in the same database transaction as the entity changes. A `BackgroundService` polls and publishes with retry — no events are lost on service restart (HIPAA requirement).

### Saga (PatientRegistrationSaga)
Orchestrates post-registration steps (welcome notification, Elasticsearch indexing, billing account creation) using MassTransit StateMachine. State is persisted in PostgreSQL for auditability.

### Polly Resilience
`ResilientEventPublisher` wraps raw Kafka publish with retry (3×, exponential) and circuit breaker (5 failures → open 30s).

## User Preferences

- Keep the existing project structure — do not restructure or migrate
- Prefer explicit DI registration over convention-based scanning
- HIPAA: never include PII in log messages, trace tags, or event payloads without encryption
- Follow the transport decision matrix in `docs/COMMUNICATION_STRATEGY.md` when adding new events
