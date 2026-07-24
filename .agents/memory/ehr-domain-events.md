---
name: EHR domain events pattern
description: Two parallel event hierarchies in the Identity service — know which to use.
---

## Two hierarchies

| Base class | Namespace | Purpose |
|---|---|---|
| `DomainEvent` | `EHRPlatform.Common.Entities` | In-process; raised on entities via `entity.RaiseDomainEvent()` |
| `IntegrationEvent` | `EHRPlatform.Common.Events` | Cross-service messaging via Kafka / MassTransit |

## File locations
- **DomainEvent subclasses** (in-process): `Domain/Events/*DomainEvent.cs` — e.g. `UserRegisteredDomainEvent`, `UserCreatedDomainEvent`, `PasswordChangedDomainEvent`, `MfaEnabledDomainEvent`
- **IntegrationEvent subclasses** (cross-service): `Domain/Events/*Event.cs` — e.g. `UserCreatedEvent`, `PasswordChangedEvent`, `MfaEnabledEvent`

**Why:** The names are similar but the base types are different. A `DomainEvent` cannot be sent to Kafka; an `IntegrationEvent` cannot be raised on an entity. Keep the naming convention (*DomainEvent suffix for in-process) to avoid confusion.

## How to apply
When adding a new in-process event, extend `DomainEvent` and suffix the name with `DomainEvent`. When adding a cross-service event, extend `IntegrationEvent` with no special suffix.
