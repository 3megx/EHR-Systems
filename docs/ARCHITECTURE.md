# System Architecture

## Overview

Modern EHR Platform is a **microservices-based, cloud-native healthcare system** designed for scalability, reliability, and HIPAA compliance.

### Architecture Pattern

```
┌─────────────────────────────────────────────────────────────────┐
│                        Frontend (Angular 18)                    │
│  Standalone Components | NgRx Signals | Tailwind CSS | i18n     │
└────────────────────┬──────────────────────────────────────────┘
                     │ HTTPS/TLS
┌────────────────────▼──────────────────────────────────────────┐
│                   API Gateway (Kong/Nginx)                     │
│         Rate Limiting | Authentication | Routing                │
└────────────────────┬──────────────────────────────────────────┘
                     │
    ┌────────────────┼────────────────┬─────────────────┐
    │                │                │                 │
┌───▼────────┐ ┌────▼────────┐ ┌───▼───────────┐ ┌──▼──────────┐
│  Patient   │ │ Appointment │ │ Medical Record│ │Prescription │
│  Service   │ │  Service    │ │   Service     │ │   Service   │
├────────────┤ ├─────────────┤ ├───────────────┤ ├─────────────┤
│ .NET Core  │ │ .NET Core   │ │ .NET Core     │ │ .NET Core   │
│ + EF Core  │ │ + EF Core   │ │ + EF Core     │ │ + EF Core   │
└────┬───────┘ └──┬──────────┘ └────┬──────────┘ └──┬──────────┘
     │            │                 │              │
┌────▼────────────▼─────────────────▼──────────────▼────────┐
│                  SQL Server Database                       │
│  Patients | Appointments | Medical_Records | Prescriptions│
│            + Audit Logs | Users | Roles                   │
└──────────────────────────────────────────────────────────┘
```

---

## 🏢 Microservices Architecture

### Service Mesh

| Service | Responsibility | Tech Stack | DB |
|---------|-----------------|-----------|-----|
| **API Gateway** | Entry point, routing, auth | Kong/Nginx | - |
| **Auth Service** | JWT, OAuth2, RBAC | .NET Core 8 | SQL Server |
| **Patient Service** | Demographics, medical history | .NET Core 8 | SQL Server |
| **Appointment Service** | Scheduling, calendar | .NET Core 8 | SQL Server |
| **Medical Record Service** | SOAP notes, vitals, diagnostics | .NET Core 8 | SQL Server |
| **Prescription Service** | eRx, medication interactions | .NET Core 8 | SQL Server |
| **Billing Service** | Claims, payments, insurance | .NET Core 8 | SQL Server |

### Service Communication

```
Synchronous:
├─→ REST API (HTTP/HTTPS) - Request/Response
└─→ gRPC - High-performance inter-service

Asynchronous:
├─→ Message Queue (RabbitMQ/Azure Service Bus)
│   └─→ Patient Created Event → Update Search Index
└─→ SignalR - Real-time client notifications
    └─→ Vital signs update → Push to monitoring dashboard
```

---

## 📊 Data Flow

### Patient Creation Flow

```
1. User submits form (Frontend)
   │
2. POST /api/v1/patients (API Gateway)
   │
3. Route to Patient Service
   │
4. Validate & create patient record (DB)
   │
5. Emit "PatientCreated" event (Message Queue)
   │
6. ├─→ Search Service: Index patient
   │ ├─→ Audit Service: Log creation
   │ └─→ SignalR: Notify connected clients
   │
7. Response with patient ID (201 Created)
   │
8. Update UI (Frontend) with confirmation
```

### Appointment Booking Flow

```
1. Clinician selects patient & time slot
   │
2. Check availability (Appointment Service)
   │
3. Create appointment record
   │
4. Emit "AppointmentCreated" event
   │
5. ├─→ Notification Service: Send SMS/Email to patient
   │ ├─→ Audit Log: Record action
   │ └─→ SignalR: Update calendar view
   │
6. Return appointment details
```

---

## 🔐 Security Layers

### 1. Authentication & Authorization

```
User Login Request
    │
    ├─→ Validate credentials (Auth Service)
    │
    ├─→ Generate JWT token (exp: 1 hour)
    │   └─→ Include claims: userId, roles, permissions
    │
    ├─→ Return token + refresh token (exp: 7 days)
    │
    ├─→ Client stores in secure HTTP-only cookie
    │
    ├─→ Each request includes: Authorization: Bearer <token>
    │
    ├─→ API Gateway validates signature
    │
    └─→ Route to appropriate microservice
```

### 2. Role-Based Access Control (RBAC)

```
User Roles:
├─→ SuperAdmin - Full system access
├─→ Admin - User management, settings
├─→ Doctor - Patient records, prescriptions
├─→ Nurse - Patient vitals, appointments
├─→ Receptionist - Scheduling, front desk
└─→ Patient - View own records

Permission Matrix:
├─→ patients:read, patients:create, patients:update, patients:delete
├─→ appointments:read, appointments:create, appointments:cancel
├─→ prescriptions:read, prescriptions:create, prescriptions:refill
└─→ reports:view, reports:export, reports:schedule
```

### 3. Data Masking & PII Protection

```
Protected Fields:
├─→ SSN: Last 4 digits shown → XXX-XX-1234
├─→ MRN: Hash stored, display with permission check
├─→ DOB: Show only to authorized roles
├─→ Email: Show to patient and their providers
└─→ Phone: Mask until verified

Audit Trail:
└─→ All access to sensitive data logged
    ├─→ Who accessed it
    ├─→ When accessed
    ├─→ From which IP/device
    └─→ Purpose/reason
```

### 4. Network Security

```
┌──────────────┐
│   Internet   │
└──────┬───────┘
       │ HTTPS/TLS 1.3
┌──────▼──────────────────┐
│  WAF (Web App Firewall) │  ← DDoS protection, rate limiting
└──────┬──────────────────┘
       │
┌──────▼──────────────────┐
│   API Gateway (Kong)    │  ← Encryption, authentication check
└──────┬──────────────────┘
       │
┌──────▼──────────────────┐
│   Service Mesh (Istio)  │  ← mTLS between services
└──────┬──────────────────┘
       │
┌──────▼──────────────────┐
│  Individual Services    │  ← Internal firewall rules
└──────┬──────────────────┘
       │
┌──────▼──────────────────┐
│    SQL Server (VPC)     │  ← Encrypted connections only
└─────────────────────────┘
```

---

## 🗄 Database Schema (Simplified)

### Core Tables

```sql
Users
├─ id (PK)
├─ email (unique)
├─ password_hash
├─ first_name
├─ last_name
└─ role_id (FK → Roles)

Patients
├─ id (PK)
├─ mrn (unique, masked)
├─ first_name
├─ last_name
├─ dob (encrypted)
├─ ssn (encrypted)
├─ gender
├─ phone
├─ email
├─ address
└─ allergies (JSON)

Appointments
├─ id (PK)
├─ patient_id (FK)
├─ doctor_id (FK → Users)
├─ appointment_date
├─ appointment_time
├─ status (scheduled, completed, cancelled)
└─ notes

Medical_Records
├─ id (PK)
├─ patient_id (FK)
├─ record_date
├─ record_type (soap, vitals, diagnosis)
├─ content (encrypted for sensitive data)
└─ created_by (FK → Users)

Prescriptions
├─ id (PK)
├─ patient_id (FK)
├─ medication_name
├─ dosage
├─ frequency
├─ issued_date
├─ expiry_date
└─ doctor_id (FK → Users)

Audit_Logs
├─ id (PK)
├─ user_id (FK)
├─ action (create, read, update, delete, export)
├─ resource_type (patient, appointment, prescription)
├─ resource_id
├─ changes (JSON diff)
├─ ip_address
├─ timestamp
└─ status (success, failure)
```

---

## 🚀 Deployment Architecture

### Development

```
Developer Laptop
├─→ Docker Desktop
├─→ docker-compose up
├─→ Frontend: http://localhost:4200
├─→ Backend API: http://localhost:5000
└─→ SQL Server: localhost:1433
```

### Staging

```
Azure Container Registry
├─→ Frontend image: ehr/frontend:staging
├─→ Backend image: ehr/backend:staging
│
Azure Kubernetes Service (AKS)
├─→ Namespace: staging
├─→ Replicas: 2 (frontend), 3 (backend)
├─→ Load Balancer (Layer 4)
├─→ Ingress Controller (Layer 7)
│
Azure SQL Database
├─→ Standard tier
├─→ Geo-replication enabled
└─→ Daily backups
```

### Production

```
Azure Container Registry
├─→ Frontend image: ehr/frontend:latest
├─→ Backend image: ehr/backend:latest
│
Azure Kubernetes Service (AKS) - HA Cluster
├─→ Namespace: production
├─→ Frontend replicas: 4 (min 2 on each AZ)
├─→ Backend replicas: 6 (min 2 per service per AZ)
├─→ Auto-scaling: CPU > 70%, scale up
│
Azure Traffic Manager
├─→ Geographic routing
├─→ Health checks every 30s
│
Azure SQL Database
├─→ Premium tier
├─→ Active Geo-Replication
├─→ Hourly backups
└─→ Point-in-time restore: 35 days

Monitoring
├─→ Application Insights
├─→ Log Analytics
├─→ Alert Rules (error rate > 1%, latency > 500ms)
└─→ Custom dashboards
```

---

## 📈 Scalability Strategy

### Horizontal Scaling

```
Load increases → CPU/Memory threshold breached
                 │
                 ├─→ Auto-scaler triggers
                 │
                 ├─→ New pod spawned
                 │
                 ├─→ Load balancer routes traffic
                 │
                 └─→ Scale-down after 5 min idle
```

### Database Scaling

```
Read-heavy workloads:
├─→ Read replicas (3+)
├─→ Redis cache layer
├─→ Query optimization

Write-heavy workloads:
├─→ Connection pooling
├─→ Index optimization
├─→ Partition by date
└─→ Archive old records
```

### Caching Strategy

```
Frontend:
├─→ HTTP caching (3 months for static assets)
├─→ IndexedDB for offline patient data
└─→ Service Worker cache

Backend:
├─→ Redis (1-hour TTL for user permissions)
├─→ In-memory cache for frequently accessed data
├─→ CDN for images
└─→ Message queue deduplication
```

---

## 🔄 CI/CD Pipeline

```
Developer pushes code
    │
    ├─→ GitHub Actions triggered
    │
    ├─→ Build stage
    │   ├─→ npm install + npm run build (frontend)
    │   ├─→ dotnet build (backend)
    │   └─→ Run unit tests
    │
    ├─→ Test stage
    │   ├─→ npm run test:ci (frontend)
    │   ├─→ dotnet test (backend)
    │   └─→ Coverage > 80% required
    │
    ├─→ Security stage
    │   ├─→ SAST (SonarQube)
    │   ├─→ Dependency scan (Snyk)
    │   └─→ Container scan (Trivy)
    │
    ├─→ Build artifacts
    │   ├─→ Docker build
    │   ├─→ Push to ACR
    │   └─→ Create image tags
    │
    ├─→ Deploy to staging
    │   ├─→ Manual approval required
    │   ├─→ Smoke tests (E2E)
    │   └─→ Performance tests
    │
    └─→ Ready for production deployment
        └─→ Manual approval required
```

---

## 🛡 Disaster Recovery

### Backup Strategy

```
Real-time:
├─→ Database replication (synchronous)
└─→ Write-ahead logs

Hourly:
├─→ Database snapshot
└─→ Backup vault storage

Daily:
├─→ Full database backup
├─→ Retention: 35 days
└─→ Test restore weekly
```

### RTO & RPO

```
RTO (Recovery Time Objective): 1 hour
└─→ Restore from latest backup + apply logs

RPO (Recovery Point Objective): < 5 minutes
└─→ Replication lag + backup frequency
```

### Failover Process

```
Primary region fails
    │
    ├─→ Health check detects (< 30s)
    │
    ├─→ Alert sent to ops team
    │
    ├─→ Manual failover initiated
    │
    ├─→ DNS updated to secondary region
    │
    ├─→ Traffic redirected (< 2 min)
    │
    └─→ Validate data consistency
```

---

## 📊 Performance Targets

| Metric | Target | Monitoring |
|--------|--------|-----------|
| API Response Time (p95) | < 500ms | Application Insights |
| Frontend Load Time | < 3s | Lighthouse, WebVitals |
| Database Query Time (p95) | < 200ms | SQL Profiler |
| Availability | 99.95% (5 nines) | Synthetic monitoring |
| Error Rate | < 0.1% | Log Analytics |
| Cache Hit Ratio | > 80% | Redis metrics |

---

## 🔗 Related Documentation

- [API_SPECIFICATION.md](./API_SPECIFICATION.md) - API endpoints & contracts
- [DEPLOYMENT.md](./DEPLOYMENT.md) - Detailed deployment procedures
- [SECURITY.md](./SECURITY.md) - Security checklist & compliance
- [DATABASE_SCHEMA.md](./DATABASE_SCHEMA.md) - Full schema details

---

**Version**: 1.0.0 | Last Updated: July 2026
