# 🎯 MASTER PLAN: Domain Folder Reorganization for ALL Services

**Objective**: Standardize Domain structure across ALL microservices following Billing's pattern

**Reference Standard**: `/backend/src/EHRPlatform.Services.Billing/Domain`
- ✅ Organized by TYPE (Entities, Enums, Events)
- ✅ Clean separation of concerns
- ✅ Easy to scale

---

## 📐 THE STANDARD STRUCTURE

Every service MUST have this exact structure:

```
/Domain/Entities/        ← All aggregate roots & entities
/Domain/Enums/           ← All enumerations
/Domain/Events/          ← All domain events
/Domain/ValueObjects/    ← Value objects (optional but recommended)
/Domain/Specifications/  ← Query specifications (optional)
/Domain/Exceptions/      ← Domain exceptions (optional)
```

---

## 🚀 QUICK REFERENCE: What Goes Where

| File Type | Folder | Example |
|-----------|--------|---------|
| Entities | Entities/ | `Patient.cs`, `Invoice.cs`, `Appointment.cs` |
| Enums | Enums/ | `PatientStatus.cs`, `InvoiceStatus.cs` |
| Domain Events | Events/ | `PatientCreatedEvent.cs`, `PaymentReceivedEvent.cs` |
| Value Objects | ValueObjects/ | `MRN.cs`, `Money.cs`, `BloodPressure.cs` |
| Specifications | Specifications/ | `ActivePatientsSpec.cs` |
| Domain Exceptions | Exceptions/ | `InvalidStateException.cs` |

---

## ✅ SERVICE STATUS & ACTION ITEMS

### 1. BILLING SERVICE
**Status**: ✅ ALREADY CORRECT - No changes needed
**Structure**:
- `/Domain/Entities/` ✅
- `/Domain/Enums/` ✅
- `/Domain/Events/` ✅

---

### 2. PATIENT SERVICE
**Status**: ❌ NEEDS REORGANIZATION

**Current**:
```
/Domain
└── Patient.cs (only file)
```

**Target**:
```
/Domain/Entities/
├── Patient.cs              (MOVE)
├── EmergencyContact.cs     (NEW)
├── InsuranceInfo.cs        (NEW)
├── Allergy.cs              (NEW)
└── MedicalAllergy.cs       (NEW)

/Domain/Enums/
├── PatientStatus.cs        (NEW)
├── BloodType.cs            (NEW)
└── MaritalStatus.cs        (NEW)

/Domain/Events/
├── PatientRegisteredEvent.cs      (NEW)
├── PatientUpdatedEvent.cs         (NEW)
├── PatientDeactivatedEvent.cs     (NEW)
└── PatientDeletedEvent.cs         (NEW)

/Domain/ValueObjects/
├── MRN.cs                  (NEW - Medical Record Number, immutable)
├── DateOfBirth.cs          (NEW)
└── ContactInfo.cs          (NEW)
```

**Steps**:
1. [ ] Create `/Domain/Entities` folder
2. [ ] Move `Patient.cs` → `/Domain/Entities/Patient.cs`
3. [ ] Create new entity files (EmergencyContact, InsuranceInfo, Allergy)
4. [ ] Create `/Domain/Enums` folder with status/type enums
5. [ ] Create `/Domain/Events` folder with all domain events
6. [ ] Create `/Domain/ValueObjects` folder with immutable objects
7. [ ] Update namespaces in all moved files
8. [ ] Find & replace imports in Features:
   - `using EHRPlatform.Services.Patient.Domain;` → `using EHRPlatform.Services.Patient.Domain.Entities;`
   - Update other imports similarly
9. [ ] Build and verify no errors
10. [ ] Commit changes

**Estimated Time**: 1 hour

---

### 3. APPOINTMENT SERVICE
**Status**: ❌ NEEDS REORGANIZATION

**Current**:
```
/Domain
├── Appointment.cs
├── AppointmentDomainEvents.cs   (WRONG LOCATION)
├── AppointmentReminder.cs
└── ProviderAvailability.cs
```

**Target**:
```
/Domain/Entities/
├── Appointment.cs                 (MOVE)
├── AppointmentReminder.cs         (MOVE)
├── ProviderAvailability.cs        (MOVE)
├── TimeSlot.cs                    (NEW)
└── AvailabilitySlot.cs            (NEW)

/Domain/Enums/
├── AppointmentStatus.cs           (NEW)
├── ReminderType.cs                (NEW)
├── ReminderStatus.cs              (NEW)
└── AvailabilityStatus.cs          (NEW)

/Domain/Events/
├── AppointmentScheduledEvent.cs          (Extract from AppointmentDomainEvents.cs)
├── AppointmentConfirmedEvent.cs          (Extract)
├── AppointmentRescheduledEvent.cs        (Extract)
├── AppointmentCancelledEvent.cs          (Extract)
├── AppointmentCompletedEvent.cs          (Extract)
├── ReminderScheduledEvent.cs             (Extract)
├── ReminderSentEvent.cs                  (Extract)
└── AvailabilityUpdatedEvent.cs           (NEW)
```

**Steps**:
1. [ ] Create `/Domain/Entities` folder
2. [ ] Move `Appointment.cs`, `AppointmentReminder.cs`, `ProviderAvailability.cs` to `/Domain/Entities/`
3. [ ] Create new entity files (TimeSlot, AvailabilitySlot)
4. [ ] Create `/Domain/Enums` folder with status/type enums
5. [ ] Create `/Domain/Events` folder
6. [ ] Extract individual event classes from `AppointmentDomainEvents.cs` into separate files
7. [ ] Delete `AppointmentDomainEvents.cs`
8. [ ] Update all namespaces
9. [ ] Update all imports in Features
10. [ ] Build and verify
11. [ ] Commit changes

**Estimated Time**: 1.5 hours

---

### 4. CLINICAL SERVICE
**Status**: ❌ NEEDS REORGANIZATION

**Current**:
```
/Domain
└── ClinicalNote.cs (only file)
```

**Target**:
```
/Domain/Entities/
├── ClinicalNote.cs                (MOVE & ENHANCE)
├── SOAPNote.cs                    (NEW - enhance ClinicalNote)
├── VitalSigns.cs                  (NEW)
├── Diagnosis.cs                   (NEW)
├── LabOrder.cs                    (NEW)
└── MedicationOrder.cs             (NEW)

/Domain/Enums/
├── ClinicalNoteStatus.cs          (NEW)
├── SOAPNoteType.cs                (NEW)
├── DiagnosisStatus.cs             (NEW)
├── LabOrderStatus.cs              (NEW)
└── ProcedureType.cs               (NEW)

/Domain/Events/
├── ClinicalNoteCreatedEvent.cs    (NEW)
├── ClinicalNoteUpdatedEvent.cs    (NEW)
├── VitalSignsRecordedEvent.cs     (NEW)
├── DiagnosisAddedEvent.cs         (NEW)
├── LabOrderCreatedEvent.cs        (NEW)
└── MedicationOrderedEvent.cs      (NEW)

/Domain/ValueObjects/
├── TemperatureReading.cs          (NEW)
├── BloodPressure.cs               (NEW)
├── HeartRate.cs                   (NEW)
├── OxygenSaturation.cs            (NEW)
└── RespiratoryRate.cs             (NEW)
```

**Steps**:
1. [ ] Create `/Domain/Entities` folder
2. [ ] Move `ClinicalNote.cs` → `/Domain/Entities/ClinicalNote.cs`
3. [ ] Create additional entity files (SOAPNote, VitalSigns, Diagnosis, LabOrder, etc.)
4. [ ] Create `/Domain/Enums` folder
5. [ ] Create `/Domain/Events` folder
6. [ ] Create `/Domain/ValueObjects` folder with measurement value objects
7. [ ] Update namespaces
8. [ ] Update all imports in Features
9. [ ] Build and verify
10. [ ] Commit changes

**Estimated Time**: 1.5 hours

---

### 5. IDENTITY SERVICE
**Status**: ❌ NEEDS REORGANIZATION

**Current**:
```
/Domain
└── User.cs (only file)
```

**Target**:
```
/Domain/Entities/
├── User.cs                        (MOVE)
├── Role.cs                        (NEW)
├── Permission.cs                  (NEW)
├── RefreshToken.cs                (NEW)
└── UserRole.cs                    (NEW - junction entity)

/Domain/Enums/
├── UserStatus.cs                  (NEW)
├── RoleType.cs                    (NEW)
├── PermissionLevel.cs             (NEW)
└── TokenType.cs                   (NEW)

/Domain/Events/
├── UserCreatedEvent.cs            (NEW)
├── UserActivatedEvent.cs          (NEW)
├── UserDeactivatedEvent.cs        (NEW)
├── UserLoginEvent.cs              (NEW)
├── PasswordChangedEvent.cs        (NEW)
├── RoleAssignedEvent.cs           (NEW)
├── PermissionGrantedEvent.cs      (NEW)
└── TokenRefreshedEvent.cs         (NEW)

/Domain/ValueObjects/
├── Email.cs                       (NEW - immutable, validation)
├── HashedPassword.cs              (NEW)
├── PhoneNumber.cs                 (NEW)
└── TwoFactorCode.cs               (NEW)

/Domain/Exceptions/
├── InvalidCredentialsException.cs (NEW)
├── UserNotActivatedException.cs   (NEW)
└── TokenExpiredException.cs       (NEW)
```

**Steps**:
1. [ ] Create `/Domain/Entities` folder
2. [ ] Move `User.cs` → `/Domain/Entities/User.cs`
3. [ ] Create additional entity files (Role, Permission, RefreshToken, etc.)
4. [ ] Create `/Domain/Enums` folder
5. [ ] Create `/Domain/Events` folder
6. [ ] Create `/Domain/ValueObjects` folder
7. [ ] Create `/Domain/Exceptions` folder for domain exceptions
8. [ ] Update namespaces
9. [ ] Update imports
10. [ ] Build and verify
11. [ ] Commit changes

**Estimated Time**: 2 hours

---

### 6. PRESCRIPTION SERVICE
**Status**: ❌ LIKELY NEEDS REORGANIZATION

**Target Structure**:
```
/Domain/Entities/
├── Prescription.cs
├── PrescriptionLine.cs
├── Refill.cs
├── Medication.cs
└── DrugInteraction.cs

/Domain/Enums/
├── PrescriptionStatus.cs
├── RefillStatus.cs
├── MedicationType.cs
└── FrequencyType.cs

/Domain/Events/
├── PrescriptionIssuedEvent.cs
├── PrescriptionFilledEvent.cs
├── RefillRequestedEvent.cs
└── InteractionDetectedEvent.cs

/Domain/ValueObjects/
├── Dosage.cs
├── MedicationName.cs
└── PrescriptionDirections.cs
```

**Estimated Time**: 1.5 hours

---

### 7. NOTIFICATION SERVICE
**Status**: ❌ LIKELY NEEDS REORGANIZATION

**Target Structure**:
```
/Domain/Entities/
├── Notification.cs
├── NotificationTemplate.cs
├── NotificationPreference.cs
└── DeliveryLog.cs

/Domain/Enums/
├── NotificationType.cs
├── NotificationChannel.cs
├── DeliveryStatus.cs
└── TemplateType.cs

/Domain/Events/
├── NotificationSentEvent.cs
├── NotificationFailedEvent.cs
├── DeliveryAttemptEvent.cs
└── PreferenceUpdatedEvent.cs

/Domain/ValueObjects/
├── RecipientAddress.cs
└── NotificationContent.cs
```

**Estimated Time**: 1.5 hours

---

### 8. ANALYTICS SERVICE
**Status**: ❌ LIKELY NEEDS REORGANIZATION

**Target Structure**:
```
/Domain/Entities/
├── AnalyticsMetric.cs
├── Dashboard.cs
├── Report.cs
└── DataSnapshot.cs

/Domain/Enums/
├── MetricType.cs
├── ReportType.cs
├── TimeGranularity.cs
└── AggregationType.cs

/Domain/Events/
├── ReportGeneratedEvent.cs
├── MetricCalculatedEvent.cs
└── DashboardUpdatedEvent.cs
```

**Estimated Time**: 1 hour

---

### 9. AUDIT SERVICE
**Status**: ❌ LIKELY NEEDS REORGANIZATION

**Target Structure**:
```
/Domain/Entities/
├── AuditLog.cs
├── AuditEntry.cs
└── ChangeHistory.cs

/Domain/Enums/
├── AuditAction.cs
├── EntityType.cs
└── ChangeType.cs

/Domain/Events/
└── AuditLogCreatedEvent.cs
```

**Estimated Time**: 45 minutes

---

### 10. API GATEWAY
**Status**: ⚠️ May not have Domain or minimal domain

**Action**: Check if it needs a Domain folder or if it's just routing

---

## 📊 TOTAL EFFORT ESTIMATE

| Service | Time |
|---------|------|
| Billing | ✅ DONE |
| Patient | 1 hr |
| Appointment | 1.5 hrs |
| Clinical | 1.5 hrs |
| Identity | 2 hrs |
| Prescription | 1.5 hrs |
| Notification | 1.5 hrs |
| Analytics | 1 hr |
| Audit | 45 min |
| **TOTAL** | **~12 hours** |

---

## 🔄 Execution Order (Recommended)

1. **Patient Service** (simplest, 1 hour)
2. **Appointment Service** (extract events, 1.5 hours)
3. **Clinical Service** (value objects, 1.5 hours)
4. **Identity Service** (complex, 2 hours)
5. **Prescription Service** (1.5 hours)
6. **Notification Service** (1.5 hours)
7. **Analytics Service** (1 hour)
8. **Audit Service** (45 minutes)

---

## ✅ IMPLEMENTATION CHECKLIST TEMPLATE

For each service, use this checklist:

```
SERVICE: [Name]

PREPARATION:
- [ ] Create backup/review current Domain structure
- [ ] List all files to move
- [ ] List all new files to create
- [ ] Identify import dependencies

IMPLEMENTATION:
- [ ] Create /Domain/Entities folder
- [ ] Move/create all entity files
- [ ] Create /Domain/Enums folder
- [ ] Create all enum files
- [ ] Create /Domain/Events folder
- [ ] Create all domain event files
- [ ] Create /Domain/ValueObjects folder (if needed)
- [ ] Create value object files
- [ ] Create /Domain/Exceptions folder (if needed)
- [ ] Create exception files

IMPORT UPDATES:
- [ ] Update namespaces in moved files
- [ ] Find all usages in Features/
- [ ] Update imports: Domain → Domain.Entities
- [ ] Update imports: Domain → Domain.Enums
- [ ] Update imports: Domain → Domain.Events
- [ ] Check Controllers for imports
- [ ] Check Application layer imports

VERIFICATION:
- [ ] Build succeeds (0 errors)
- [ ] Build succeeds (0 warnings related to imports)
- [ ] Review git diff to ensure correct moves
- [ ] Commit with clear message

TIME SPENT: _____ hours
```

---

## 🎯 NEXT STEPS

**Ready to start reorganization?**

**Recommended**: Start with Patient Service (simplest case)

Would you like me to:
1. Start reorganizing Patient Service?
2. Start with a different service?
3. Create automated scripts to help with the reorganization?

