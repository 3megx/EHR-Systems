# Domain Folder Structure Standardization

**Objective**: Consolidate domain models and ensure consistent structure across all microservices

**Status**: Analysis complete, standardization needed

---

## 🔍 Current State Analysis

### Problem Identified
Each service has domain models in different places:
- Some use `/Domain` at service root (Billing, Appointment, Patient, Clinical)
- Some have domain logic nested inside `/Features/[Feature]/Domain`
- Inconsistency in entity organization (Entities, Enums, Events, ValueObjects)

### Current Structures

**Billing Service** (INCONSISTENT - needs fix):
```
/Domain
├── Entities/         ← Should be consolidated
├── Enums/
└── Events/
/Features
├── Invoicing/       ← Has its own domain? Check deeper
├── Payments/
├── Claims/
└── Reports/
```

**Appointment Service** (PARTIALLY ORGANIZED):
```
/Domain
├── Appointment.cs
├── AppointmentDomainEvents.cs
├── AppointmentReminder.cs
└── ProviderAvailability.cs
/Features/
└── (feature folders)
```

**Patient Service** (SIMPLE):
```
/Domain
└── Patient.cs
/Features/
└── (feature folders)
```

**Clinical Service** (MINIMAL):
```
/Domain
/Features
└── ClinicalNotes/
```

---

## ✅ Standardized Domain Structure (TO IMPLEMENT)

### Primary Organization: Domain-First Pattern

All services MUST follow this exact structure:

```
/Domain                              ← Root domain folder (service-wide)
│
├── Entities/                        ← Aggregate roots and entities
│   ├── [EntityName].cs
│   ├── [AnotherEntity].cs
│   └── [ValueObject].cs
│
├── Enums/                           ← Enumerations
│   ├── [EntityName]Status.cs
│   ├── [EntityName]Type.cs
│   └── Common[Enum].cs
│
├── ValueObjects/                    ← Value objects (immutable)
│   ├── [ValueObject].cs
│   └── [AnotherValueObject].cs
│
├── Events/                          ← Domain events
│   ├── [EntityName]CreatedEvent.cs
│   ├── [EntityName]UpdatedEvent.cs
│   └── [EntityName]DeletedEvent.cs
│
├── Specifications/                  ← Query specifications (optional)
│   ├── Active[Entity]Specification.cs
│   └── [Entity]ByIdSpecification.cs
│
└── Exceptions/                      ← Domain exceptions
    ├── InvalidOperationException.cs
    └── DomainRuleException.cs
```

---

## 🔗 How Features Reference Domain

### Feature Structure (UNCHANGED):
```
/Features
├── [Feature]/
│   ├── Commands/
│   │   └── [Command].cs
│   ├── Handlers/
│   │   └── [CommandHandler].cs
│   ├── Queries/
│   │   └── [Query].cs
│   ├── Dtos/
│   │   └── [Dto].cs
│   └── Validators/
│       └── [Validator].cs
```

### Key Rule
**Features IMPORT from Domain, NOT the reverse**
- ✅ GOOD: `Features/Invoicing/Commands` imports `Domain/Entities/Invoice`
- ❌ BAD: `Domain/Entities/Invoice` references `Features/Invoicing/Dtos`

---

## 📋 Consolidation Plan

### For Each Service

**Step 1: Audit Current State**
- [ ] List all files currently in `/Domain`
- [ ] Check if any `/Features/[Feature]/Domain` exists
- [ ] Identify duplicate entity definitions

**Step 2: Move to Standard Structure**
- [ ] Create `/Domain/Entities`
- [ ] Create `/Domain/Enums`
- [ ] Create `/Domain/ValueObjects` (if needed)
- [ ] Create `/Domain/Events`
- [ ] Move all entities to `/Domain/Entities`
- [ ] Move all enums to `/Domain/Enums`
- [ ] Move all domain events to `/Domain/Events`

**Step 3: Update Imports**
- [ ] Update all Feature files to reference new Domain paths
- [ ] Remove any nested `/Features/[Feature]/Domain` folders
- [ ] Update namespaces consistently

**Step 4: Verify**
- [ ] Build each service individually
- [ ] No circular references
- [ ] No unused imports
- [ ] Consistent naming conventions

---

## 🎯 Service-by-Service Consolidation

### 1. **Billing Service**
**Current Issue**: Mixed organization
**Action Required**:
```
/Domain
├── Entities/
│   ├── Invoice.cs
│   ├── Payment.cs
│   ├── Claim.cs
│   ├── BillingTransaction.cs
│   └── InsuranceCoverage.cs
├── Enums/
│   ├── InvoiceStatus.cs
│   ├── PaymentMethod.cs
│   ├── ClaimStatus.cs
│   └── PaymentStatus.cs
├── Events/
│   ├── InvoiceCreatedEvent.cs
│   ├── PaymentProcessedEvent.cs
│   └── ClaimSubmittedEvent.cs
└── ValueObjects/
    ├── Money.cs
    ├── BillingPeriod.cs
    └── ClaimDetails.cs
```

**Features** (unchanged):
- `/Features/Invoicing/` → imports Domain/Entities/Invoice
- `/Features/Payments/` → imports Domain/Entities/Payment
- `/Features/Claims/` → imports Domain/Entities/Claim
- `/Features/Reports/` → uses multiple entities

---

### 2. **Appointment Service**
**Current**: Good start, needs organization
**Action Required**:
```
/Domain/Entities/
├── Appointment.cs
├── ProviderAvailability.cs
├── AppointmentReminder.cs
└── TimeSlot.cs

/Domain/Enums/
├── AppointmentStatus.cs
├── ReminderType.cs
└── AvailabilityStatus.cs

/Domain/Events/
├── AppointmentCreatedEvent.cs
├── AppointmentRescheduledEvent.cs
├── AppointmentCancelledEvent.cs
└── ReminderSentEvent.cs
```

---

### 3. **Patient Service**
**Current**: Minimal
**Action Required**:
```
/Domain/Entities/
├── Patient.cs
├── EmergencyContact.cs
├── InsuranceInfo.cs
├── MedicalHistory.cs
└── Allergy.cs

/Domain/Enums/
├── PatientStatus.cs
├── BloodType.cs
└── MaritalStatus.cs

/Domain/Events/
├── PatientRegisteredEvent.cs
├── PatientUpdatedEvent.cs
└── PatientDeactivatedEvent.cs

/Domain/ValueObjects/
├── MRN.cs (Medical Record Number)
├── DateOfBirth.cs
└── ContactInfo.cs
```

---

### 4. **Clinical Service**
**Current**: Needs structure
**Action Required**:
```
/Domain/Entities/
├── SOAPNote.cs
├── VitalSigns.cs
├── Diagnosis.cs
├── MedicationPrescription.cs
└── LabOrder.cs

/Domain/Enums/
├── SOAPNoteType.cs
├── LabOrderStatus.cs
└── DiagnosisStatus.cs

/Domain/Events/
├── SOAPNoteCreatedEvent.cs
├── VitalSignsRecordedEvent.cs
└── DiagnosisAddedEvent.cs

/Domain/ValueObjects/
├── TemperatureReading.cs
├── BloodPressure.cs
└── HeartRate.cs
```

---

### 5. **Identity Service**
**Action Required**:
```
/Domain/Entities/
├── User.cs
├── Role.cs
├── Permission.cs
└── RefreshToken.cs

/Domain/Enums/
├── UserStatus.cs
├── RoleType.cs
└── PermissionLevel.cs

/Domain/Events/
├── UserCreatedEvent.cs
├── UserLoginEvent.cs
├── PasswordChangedEvent.cs
└── RoleAssignedEvent.cs

/Domain/ValueObjects/
├── Email.cs
├── HashedPassword.cs
└── PhoneNumber.cs
```

---

### 6. **Prescription Service**
**Action Required**:
```
/Domain/Entities/
├── Prescription.cs
├── PrescriptionLine.cs
├── RefillRequest.cs
└── PharmacyGuidance.cs

/Domain/Enums/
├── PrescriptionStatus.cs
├── RefillStatus.cs
└── MedicationType.cs

/Domain/Events/
├── PrescriptionIssuedEvent.cs
├── PrescriptionFilledEvent.cs
└── RefillRequestedEvent.cs

/Domain/ValueObjects/
├── Dosage.cs
├── Medication.cs
└── PrescriptionDirections.cs
```

---

### 7. **Notification Service**
**Action Required**:
```
/Domain/Entities/
├── Notification.cs
├── NotificationTemplate.cs
├── NotificationPreference.cs
└── NotificationLog.cs

/Domain/Enums/
├── NotificationType.cs
├── NotificationChannel.cs
├── DeliveryStatus.cs
└── PreferenceType.cs

/Domain/Events/
├── NotificationSentEvent.cs
├── NotificationFailedEvent.cs
└── PreferenceUpdatedEvent.cs
```

---

### 8. **Analytics Service**
**Action Required**:
```
/Domain/Entities/
├── AnalyticsMetric.cs
├── Dashboard.cs
├── Report.cs
└── DataSnapshot.cs

/Domain/Enums/
├── MetricType.cs
├── ReportType.cs
└── TimeGranularity.cs

/Domain/Events/
├── ReportGeneratedEvent.cs
└── MetricCalculatedEvent.cs
```

---

## 🔧 Implementation Checklist

### Phase 1: Audit (1-2 hours)
- [ ] Document current state of each service
- [ ] Identify all domain entities in each service
- [ ] List files that need to move
- [ ] Check for duplicate entity definitions

### Phase 2: Reorganize (2-3 hours)
- [ ] Create standard folder structure in each service
- [ ] Move entities to `/Domain/Entities`
- [ ] Move enums to `/Domain/Enums`
- [ ] Move events to `/Domain/Events`
- [ ] Move value objects to `/Domain/ValueObjects`
- [ ] Delete old nested `/Features/[Feature]/Domain` folders if they exist

### Phase 3: Update References (2-3 hours)
- [ ] Update all using statements in Feature files
- [ ] Update all namespace declarations
- [ ] Run build on each service
- [ ] Fix any compilation errors

### Phase 4: Verify (1 hour)
- [ ] Build all services
- [ ] No circular references
- [ ] Consistent naming (PascalCase, singular for entities)
- [ ] Git commit each service

---

## 📐 Naming Conventions

### Entities
```csharp
// ✅ GOOD
public class Patient { }
public class PatientAllergy { }
public class EmergencyContact { }

// ❌ BAD
public class PatientEntity { }
public class Patients { }
```

### Value Objects
```csharp
// ✅ GOOD
public class MedicalRecordNumber { }
public class EmailAddress { }
public class BloodPressure { }

// ❌ BAD
public class MRN { }
public class Email { }
```

### Enums
```csharp
// ✅ GOOD
public enum PatientStatus { Active, Inactive, Deleted }
public enum BloodType { A, B, O, AB }

// ❌ BAD
public enum Status { }
public enum Patient_Status { }
```

### Domain Events
```csharp
// ✅ GOOD
public class PatientCreatedEvent : IntegrationEvent { }
public class InvoicePaidEvent : IntegrationEvent { }

// ❌ BAD
public class PatientEvent { }
public class OnPatientCreated { }
```

---

## 🚀 Expected Benefits

After Consolidation:
- ✅ Consistent structure across all services
- ✅ Easy to find domain models
- ✅ Clear separation of concerns
- ✅ No nested domain duplication
- ✅ Easier onboarding for new developers
- ✅ Better for mass refactoring

---

## 📝 Next Steps

1. **Approve this standardization**
2. **Implement for each service in order**:
   - Start with Billing (most complex)
   - Then Appointment, Patient, Clinical
   - Then Identity, Prescription, Notification, Analytics
3. **Verify each service builds after consolidation**
4. **Commit each service separately**

---

**Ready to implement this standardization across all services?**

