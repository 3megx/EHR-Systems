# Inter-Service Communication Strategy

**Project**: EHR Platform  
**Version**: 2.0 — Hybrid Kafka + RabbitMQ via MassTransit  
**Last Updated**: July 2026

---

## 1. Architecture Overview

```
┌─────────────┐   HTTPS/REST    ┌───────────────────────────────────────────────┐
│  Angular 18  │◄───────────────►│  YARP API Gateway (:5000)                     │
│  Frontend   │                 │  JWT Auth · Rate Limiting · OpenTelemetry     │
└─────────────┘                 └────────┬──────────────────┬────────────────────┘
                                          │                  │
                              ┌───────────▼──┐    ┌──────────▼──────────┐
                              │   Microservices    │  Notification Svc   │
                              │                   │  SignalR Hub :5006  │
                              │  Identity  :5001  └──────────▲──────────┘
                              │  Patient   :5002             │ WebSocket
                              │  Clinical  :5003             │
                              │  Billing   :5004             │ Angular connects
                              │  Appoint.  :5005             │ to /hubs/notifications
                              └──────┬───────────────────────┘
                                     │
                        ┌────────────┴─────────────────┐
                        │                              │
              ┌─────────▼────────┐          ┌──────────▼────────┐
              │  Apache Kafka    │          │  RabbitMQ          │
              │  (Domain Events) │          │  (Background Jobs) │
              │                  │          │                    │
              │ Partition by     │          │ Queues:            │
              │ TenantId for     │          │ · welcome-notif    │
              │ ordering         │          │ · patient-index    │
              │                  │          │ · report-gen       │
              │ Topics:          │          │ · claim-process    │
              │ · patient-created│          │ Dead-letter:       │
              │ · lab-result     │          │ _error queues      │
              │ · audit-log      │          │                    │
              └─────────────────┘          └───────────────────┘
```

---

## 2. Transport Decision Matrix

| Scenario | Transport | Why |
|----------|-----------|-----|
| Patient created, lab result ready | Kafka | Ordered, durable, replayable; multiple consumers |
| Audit logging | Kafka | High-throughput, long retention for HIPAA |
| Analytics / CDC to Snowflake | Kafka | Stream processing, ksqlDB compatibility |
| Welcome email / SMS | RabbitMQ | Task queue; exactly-once delivery; dead-letter |
| Report generation | RabbitMQ | Long-running background job; priority queue |
| Insurance claim processing | RabbitMQ | Retry with backoff; complex routing |
| Billing account creation | RabbitMQ (Saga step) | Request/reply over messaging |
| In-process calls | MediatR | Same-service CQRS |
| Cross-service sync calls | HTTP via YARP | Immediate response required |

---

## 3. Kafka Topics (Domain Events)

| Topic | Partition Key | Consumers | Retention |
|-------|--------------|-----------|-----------|
| `patient-created-event.{env}` | TenantId | NotificationSvc, BillingSvc | 7 days |
| `patient-updated-event.{env}` | TenantId | SearchIndexer | 7 days |
| `lab-result-ready-event.{env}` | PatientId | NotificationSvc (→ SignalR) | 7 days |
| `appointment-scheduled-event.{env}` | PatientId | NotificationSvc | 3 days |
| `audit-log-event.{env}` | TenantId | AuditSvc | 90 days |

**Topic naming**: `{event-type-kebab-case}.{environment}` (e.g. `patient-created-event.production`)

---

## 4. RabbitMQ Queues (Background Jobs)

| Queue | Bound Exchange | Dead-letter Queue | Priority |
|-------|---------------|-------------------|---------|
| `ehr.patient.welcome-notification` | `ehr.notifications` | `ehr.patient.welcome-notification_error` | Normal |
| `ehr.patient.index` | `ehr.search` | `ehr.patient.index_error` | Normal |
| `ehr.report.generation` | `ehr.reports` | `ehr.report.generation_error` | Low |
| `ehr.billing.claim-process` | `ehr.billing` | `ehr.billing.claim-process_error` | High |

**Virtual host**: `/ehr` (isolated from other RabbitMQ tenants)  
**Management UI**: http://localhost:15672 (user: `ehr_user`)

---

## 5. Outbox Pattern (Guaranteed Delivery)

```
Command Handler
    │
    ▼
Begin Transaction
    │
    ├─ Save entity changes (EF Core)
    │
    ├─ Write OutboxEvent row (same transaction)
    │    - Transport: "kafka" or "rabbitmq"
    │    - AggregateId, EventType, EventData (JSON)
    │
    └─ Commit Transaction
         │
         ▼ (5s poll)
    OutboxProcessor (BackgroundService)
         │
         ├─ Kafka: KafkaEventPublisher (with Polly retry + circuit breaker)
         └─ RabbitMQ: IBus.Publish (MassTransit, with retry policy)
```

Events survive service crashes because they are persisted **before** the Kafka/RabbitMQ call.  
`ResilientEventPublisher` wraps the raw Kafka publisher with:  
- **Retry**: 3 attempts, exponential back-off (2s, 4s, 8s)  
- **Circuit Breaker**: opens after 5 consecutive failures, resets after 30s  

---

## 6. Saga: PatientRegistrationSaga

Orchestrates post-registration steps across services.

```
PatientCreatedEvent (Kafka)
         │
         ▼
  [State: ProcessingSteps]
         │
    ┌────┴──────────────────────────────────┐
    │                                       │
    ▼                                       ▼
SendWelcomeNotificationMessage         PatientIndexMessage
(RabbitMQ → NotificationSvc)          (RabbitMQ → Patient ES indexer)
    │                                       │
    ▼                                       ▼
WelcomeNotificationSentEvent      PatientIndexedEvent
    │                                       │
    └────────────────┬──────────────────────┘
                     ▼
             All steps done?
                     │ Yes
                     ▼
              [State: Completed]
                     │ No (failure)
                     ▼
              [State: Failed]
              → Publish compensating events
```

Saga state is persisted in the **PatientContext** PostgreSQL database using  
`MassTransit.EntityFrameworkCore`.

---

## 7. SignalR Real-Time Bridge

```
Clinical Service → Kafka: lab-result-ready-event
                                │
                    LabResultConsumer (Notification Service, Kafka)
                                │
                    EHRNotificationHub.Clients.Group("patient:{id}")
                                │
                    Angular SignalR connection
                    hub.on("LabResultReady", payload => ...)
```

**Angular connection example**:
```typescript
const hub = new HubConnectionBuilder()
  .withUrl('/hubs/notifications', { accessTokenFactory: () => authService.token })
  .withAutomaticReconnect()
  .build();

await hub.start();
await hub.invoke('JoinPatientRoom', patientId);

hub.on('LabResultReady', (result) => {
  // update dashboard reactively
  this.store.dispatch(labResultReceived({ result }));
});
```

---

## 8. Resilience & Observability

### Polly Policies (`EHRResiliencePolicies`)
| Policy | Trigger | Behavior |
|--------|---------|---------|
| HTTP Retry | 5xx, 408, 429 | 3 attempts, exponential back-off |
| HTTP Circuit Breaker | 5 consecutive failures | Opens 30s, then half-open probe |
| HTTP Timeout | Per-call | 10s default (configurable) |
| DB Retry | Transient DB errors | 3 attempts, 1/2/4s delays |
| Messaging Retry | Any exception | 3 attempts, exponential |

### OpenTelemetry
- Activity source: `ehr-platform`
- Instrumented: ASP.NET Core, HttpClient, custom spans
- Exporters: OTLP (Jaeger / Grafana Tempo) or Console in development
- HIPAA: only opaque IDs (PatientId, TenantId) in trace tags — **no PII**

---

## 9. Security

| Layer | Mechanism |
|-------|----------|
| Service-to-service HTTP | JWT Bearer (forwarded by YARP) |
| RabbitMQ | Username/password + VirtualHost isolation |
| Kafka | PLAINTEXT in dev; configure mTLS for production |
| SignalR | JWT Bearer via `access_token` query param (WebSocket limitation) |
| Event payloads | Sensitive fields encrypted with AES-256-GCM before serialisation |
| Audit trail | Every command/event logged with UserId, TenantId, CorrelationId |

---

## 10. Adding a New Domain Event (Checklist)

```
□ 1. Define the event record in the domain service:
      public record MyEvent : IntegrationEvent { ... }

□ 2. Raise it in the aggregate:
      patient.RaiseEvent(new MyEvent(...));

□ 3. Persist via OutboxEvent in the command handler:
      await _outbox.AddAsync(new OutboxEvent {
          AggregateId = entity.Id,
          EventType   = nameof(MyEvent),
          EventData   = JsonSerializer.Serialize(myEvent),
          Transport   = "kafka"   // or "rabbitmq"
      });

□ 4. Create a consumer in the downstream service:
      public class MyEventConsumer : IConsumer<MyEvent> { ... }

□ 5. Register the consumer in Program.cs (Kafka rider or RabbitMQ):
      rider.AddConsumer<MyEventConsumer>();

□ 6. Add to the communication matrix in this document.
```

---

## 11. Running the Stack

```bash
# Start all infrastructure (Postgres, Redis, Kafka, RabbitMQ, Elasticsearch)
docker-compose up -d

# Start microservices
docker-compose -f docker-compose.yml -f docker-compose.services.yml up

# Frontend (Angular)
cd frontend && npm install && npm start

# Service endpoints
# API Gateway:         http://localhost:5000
# Identity:            http://localhost:5001
# Patient:             http://localhost:5002
# Notification:        http://localhost:5006
# Kafka UI:            http://localhost:8080
# RabbitMQ UI:         http://localhost:15672  (ehr_user / ehr_password)
# Elasticsearch:       http://localhost:9200
# Kibana:              http://localhost:5601
```
