# Database Schema

Complete database design for Modern EHR Platform with all tables, relationships, and constraints.

---

## 📊 Entity Relationship Diagram (ERD)

```
┌─────────────┐         ┌──────────────┐         ┌──────────────┐
│    Users    │◄───────┤   Patients   │────────►│  Appointments│
└─────────────┘         └──────────────┘         └──────────────┘
      │ │                      │                        │
      │ │                      │                        │
      │ └──────┐       ┌───────┴────────┐              │
      │        │       │                │              │
      │   ┌────▼───┐   │   ┌────────────▼──┐      ┌───▼───────┐
      │   │ Roles  │   │   │ Patient_Info  │      │Prescriptions
      │   └────────┘   │   └───────────────┘      └────┬──────┘
      │                │                               │
      │   ┌────────────▼──┐      ┌──────────────┐     │
      │   │ Medical_      │      │    Audit_    │     │
      └──►│ Records       │      │    Logs      │◄────┘
          └───────┬───────┘      └──────────────┘
                  │
                  │
          ┌───────┴─────────┐
          │                 │
      ┌───▼──────┐  ┌──────▼──┐
      │ Allergies│  │Conditions│
      └──────────┘  └──────────┘
```

---

## 📋 Core Tables

### Users

User accounts with authentication & authorization data.

```sql
CREATE TABLE Users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) NOT NULL UNIQUE,
    email_verified BOOLEAN DEFAULT FALSE,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    middle_name VARCHAR(100),
    phone VARCHAR(20),
    phone_verified BOOLEAN DEFAULT FALSE,
    license_number VARCHAR(50),
    specialty VARCHAR(100),
    
    -- Authentication
    last_login TIMESTAMP WITH TIME ZONE,
    last_password_change TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    password_expires_at TIMESTAMP WITH TIME ZONE,
    failed_login_attempts INT DEFAULT 0,
    locked_until TIMESTAMP WITH TIME ZONE,
    
    -- MFA
    mfa_enabled BOOLEAN DEFAULT FALSE,
    mfa_secret_key VARCHAR(255),
    backup_codes TEXT[],
    
    -- Status
    status VARCHAR(50) DEFAULT 'active' CHECK (status IN ('active', 'inactive', 'suspended')),
    role_id UUID NOT NULL,
    
    -- Audit
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by UUID,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_by UUID,
    deleted_at TIMESTAMP WITH TIME ZONE,
    
    FOREIGN KEY (role_id) REFERENCES Roles(id)
);

CREATE INDEX idx_users_email ON Users(email);
CREATE INDEX idx_users_status ON Users(status);
CREATE INDEX idx_users_role_id ON Users(role_id);
```

### Roles

Predefined roles with permissions hierarchy.

```sql
CREATE TABLE Roles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    level INT NOT NULL,  -- 0: SuperAdmin, 1: Admin, 2: Doctor, etc.
    is_system_role BOOLEAN DEFAULT FALSE,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Insert default roles
INSERT INTO Roles (name, description, level, is_system_role) VALUES
    ('SuperAdmin', 'Full system access', 0, TRUE),
    ('Admin', 'User and system management', 1, TRUE),
    ('Doctor', 'Clinical access', 2, TRUE),
    ('Nurse', 'Patient care access', 3, TRUE),
    ('Receptionist', 'Front desk operations', 4, TRUE),
    ('Patient', 'Patient self-service', 5, TRUE);
```

### Permissions

Fine-grained access control.

```sql
CREATE TABLE Permissions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    resource VARCHAR(50) NOT NULL,
    action VARCHAR(50) NOT NULL,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Examples:
-- patients:read, patients:create, patients:update, patients:delete
-- appointments:read, appointments:create, appointments:cancel
-- etc.
```

### RolePermissions

Many-to-many relationship between roles and permissions.

```sql
CREATE TABLE RolePermissions (
    role_id UUID NOT NULL,
    permission_id UUID NOT NULL,
    
    PRIMARY KEY (role_id, permission_id),
    FOREIGN KEY (role_id) REFERENCES Roles(id),
    FOREIGN KEY (permission_id) REFERENCES Permissions(id)
);
```

---

## 👥 Patient Tables

### Patients

Core patient demographic information.

```sql
CREATE TABLE Patients (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    mrn VARCHAR(50) NOT NULL UNIQUE,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    middle_name VARCHAR(100),
    dob DATE NOT NULL ENCRYPTED,  -- Encrypted at rest
    gender VARCHAR(20),
    email VARCHAR(255),
    phone VARCHAR(20),
    ssn VARCHAR(11) NOT NULL UNIQUE ENCRYPTED,  -- Encrypted SSN
    
    -- Address
    street_address VARCHAR(255),
    city VARCHAR(100),
    state VARCHAR(50),
    zip_code VARCHAR(10),
    country VARCHAR(100),
    
    -- Emergency Contact
    emergency_contact_name VARCHAR(100),
    emergency_contact_phone VARCHAR(20),
    emergency_contact_relationship VARCHAR(50),
    
    -- Insurance
    insurance_provider VARCHAR(100),
    insurance_member_id VARCHAR(50),
    insurance_group_number VARCHAR(50),
    insurance_effective_date DATE,
    
    -- Status
    status VARCHAR(50) DEFAULT 'active' CHECK (status IN ('active', 'inactive', 'deceased')),
    
    -- Audit
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by UUID NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_by UUID,
    deleted_at TIMESTAMP WITH TIME ZONE,
    
    FOREIGN KEY (created_by) REFERENCES Users(id),
    FOREIGN KEY (updated_by) REFERENCES Users(id)
);

CREATE INDEX idx_patients_mrn ON Patients(mrn);
CREATE INDEX idx_patients_email ON Patients(email);
CREATE INDEX idx_patients_status ON Patients(status);
CREATE FULL TEXT INDEX idx_patients_search ON Patients(first_name, last_name);
```

### PatientAllergies

Patient allergy records.

```sql
CREATE TABLE PatientAllergies (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id UUID NOT NULL,
    allergen VARCHAR(255) NOT NULL,
    severity VARCHAR(50) NOT NULL CHECK (severity IN ('mild', 'moderate', 'severe')),
    reaction TEXT,
    onset_date DATE,
    notes TEXT,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by UUID NOT NULL,
    
    FOREIGN KEY (patient_id) REFERENCES Patients(id) ON DELETE CASCADE,
    FOREIGN KEY (created_by) REFERENCES Users(id)
);

CREATE INDEX idx_allergies_patient ON PatientAllergies(patient_id);
```

### PatientConditions

Chronic conditions & diagnoses.

```sql
CREATE TABLE PatientConditions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id UUID NOT NULL,
    icd10_code VARCHAR(10) NOT NULL,
    condition_name VARCHAR(255) NOT NULL,
    description TEXT,
    status VARCHAR(50) DEFAULT 'active' CHECK (status IN ('active', 'resolved', 'inactive')),
    onset_date DATE,
    resolved_date DATE,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by UUID NOT NULL,
    
    FOREIGN KEY (patient_id) REFERENCES Patients(id) ON DELETE CASCADE,
    FOREIGN KEY (created_by) REFERENCES Users(id)
);

CREATE INDEX idx_conditions_patient ON PatientConditions(patient_id);
CREATE INDEX idx_conditions_icd10 ON PatientConditions(icd10_code);
```

---

## 📅 Appointment Tables

### Appointments

Appointment scheduling records.

```sql
CREATE TABLE Appointments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id UUID NOT NULL,
    provider_id UUID NOT NULL,
    appointment_date DATE NOT NULL,
    appointment_time TIME NOT NULL,
    duration_minutes INT NOT NULL DEFAULT 30,
    appointment_type VARCHAR(50) NOT NULL,  -- follow-up, new-patient, etc.
    
    -- Status
    status VARCHAR(50) DEFAULT 'scheduled' CHECK (status IN (
        'scheduled', 'checked-in', 'in-progress', 'completed', 'no-show', 'cancelled'
    )),
    cancellation_reason TEXT,
    cancellation_time TIMESTAMP WITH TIME ZONE,
    
    -- Details
    location VARCHAR(255),
    room_number VARCHAR(20),
    notes TEXT,
    
    -- Reminders
    reminder_email_sent BOOLEAN DEFAULT FALSE,
    reminder_sms_sent BOOLEAN DEFAULT FALSE,
    reminder_sent_at TIMESTAMP WITH TIME ZONE,
    
    -- Audit
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by UUID NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_by UUID,
    
    FOREIGN KEY (patient_id) REFERENCES Patients(id),
    FOREIGN KEY (provider_id) REFERENCES Users(id),
    FOREIGN KEY (created_by) REFERENCES Users(id)
);

CREATE INDEX idx_appointments_patient ON Appointments(patient_id);
CREATE INDEX idx_appointments_provider ON Appointments(provider_id);
CREATE INDEX idx_appointments_date ON Appointments(appointment_date);
CREATE INDEX idx_appointments_status ON Appointments(status);
```

---

## 📝 Medical Records Tables

### MedicalRecords

Clinical notes (SOAP format, vitals, diagnoses).

```sql
CREATE TABLE MedicalRecords (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id UUID NOT NULL,
    appointment_id UUID,
    record_date TIMESTAMP WITH TIME ZONE NOT NULL,
    record_type VARCHAR(50) NOT NULL CHECK (record_type IN (
        'soap', 'vitals', 'procedure', 'lab_result', 'imaging_result'
    )),
    
    -- SOAP Note
    subjective TEXT ENCRYPTED,
    objective TEXT ENCRYPTED,
    assessment TEXT ENCRYPTED,
    plan TEXT ENCRYPTED,
    
    -- Status
    status VARCHAR(50) DEFAULT 'draft' CHECK (status IN ('draft', 'complete', 'signed', 'archived')),
    signed_at TIMESTAMP WITH TIME ZONE,
    
    -- Audit
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by UUID NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_by UUID,
    
    FOREIGN KEY (patient_id) REFERENCES Patients(id),
    FOREIGN KEY (appointment_id) REFERENCES Appointments(id),
    FOREIGN KEY (created_by) REFERENCES Users(id)
);

CREATE INDEX idx_records_patient ON MedicalRecords(patient_id);
CREATE INDEX idx_records_date ON MedicalRecords(record_date);
```

### VitalSigns

Patient vital measurements.

```sql
CREATE TABLE VitalSigns (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id UUID NOT NULL,
    record_date TIMESTAMP WITH TIME ZONE NOT NULL,
    
    -- Measurements
    blood_pressure_systolic INT,
    blood_pressure_diastolic INT,
    heart_rate INT,
    respiratory_rate INT,
    temperature_f DECIMAL(5, 1),
    oxygen_saturation INT,  -- 0-100
    weight_lbs DECIMAL(6, 1),
    height_inches DECIMAL(5, 1),
    
    -- Calculations
    bmi DECIMAL(5, 1),  -- Auto-calculated
    
    recorded_by UUID NOT NULL,
    notes TEXT,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (patient_id) REFERENCES Patients(id),
    FOREIGN KEY (recorded_by) REFERENCES Users(id)
);

CREATE INDEX idx_vitals_patient ON VitalSigns(patient_id);
CREATE INDEX idx_vitals_date ON VitalSigns(record_date);
```

---

## 💊 Prescription Tables

### Prescriptions

Medication prescriptions (eRx).

```sql
CREATE TABLE Prescriptions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id UUID NOT NULL,
    provider_id UUID NOT NULL,
    
    -- Medication Details
    medication_name VARCHAR(255) NOT NULL,
    dosage VARCHAR(100) NOT NULL,
    unit VARCHAR(50),
    frequency VARCHAR(100) NOT NULL,
    route VARCHAR(50) NOT NULL,  -- Oral, IV, IM, etc.
    quantity INT,
    
    -- Prescription Details
    issued_date DATE NOT NULL,
    expiry_date DATE NOT NULL,
    refills_allowed INT DEFAULT 0,
    refills_remaining INT DEFAULT 0,
    
    -- Clinical
    indication TEXT,
    side_effects TEXT,
    special_instructions TEXT,
    
    -- Status
    status VARCHAR(50) DEFAULT 'active' CHECK (status IN (
        'active', 'completed', 'cancelled', 'expired', 'suspended'
    )),
    cancellation_reason TEXT,
    cancelled_at TIMESTAMP WITH TIME ZONE,
    cancelled_by UUID,
    
    -- Audit
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    created_by UUID NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_by UUID,
    
    FOREIGN KEY (patient_id) REFERENCES Patients(id),
    FOREIGN KEY (provider_id) REFERENCES Users(id),
    FOREIGN KEY (created_by) REFERENCES Users(id)
);

CREATE INDEX idx_prescriptions_patient ON Prescriptions(patient_id);
CREATE INDEX idx_prescriptions_provider ON Prescriptions(provider_id);
CREATE INDEX idx_prescriptions_status ON Prescriptions(status);
```

### PrescriptionRefills

Refill request history.

```sql
CREATE TABLE PrescriptionRefills (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    prescription_id UUID NOT NULL,
    patient_id UUID NOT NULL,
    quantity INT NOT NULL,
    status VARCHAR(50) DEFAULT 'pending' CHECK (status IN (
        'pending', 'approved', 'denied', 'filled'
    )),
    request_date TIMESTAMP WITH TIME ZONE NOT NULL,
    decision_date TIMESTAMP WITH TIME ZONE,
    decided_by UUID,
    reason_denied TEXT,
    
    FOREIGN KEY (prescription_id) REFERENCES Prescriptions(id),
    FOREIGN KEY (patient_id) REFERENCES Patients(id),
    FOREIGN KEY (decided_by) REFERENCES Users(id)
);
```

---

## 💰 Billing Tables

### BillingClaims

Insurance claims for services rendered.

```sql
CREATE TABLE BillingClaims (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id UUID NOT NULL,
    claim_number VARCHAR(50) NOT NULL UNIQUE,
    
    service_date DATE NOT NULL,
    service_start_time TIME,
    service_end_time TIME,
    
    -- Amounts
    total_service_amount DECIMAL(12, 2) NOT NULL,
    insurance_paid_amount DECIMAL(12, 2),
    patient_responsibility DECIMAL(12, 2),
    
    -- Insurance
    insurance_provider VARCHAR(100),
    insurance_member_id VARCHAR(50),
    
    -- Status
    status VARCHAR(50) DEFAULT 'pending' CHECK (status IN (
        'pending', 'submitted', 'received', 'approved', 'denied', 'paid'
    )),
    
    -- Dates
    submitted_date TIMESTAMP WITH TIME ZONE,
    decision_date TIMESTAMP WITH TIME ZONE,
    paid_date TIMESTAMP WITH TIME ZONE,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (patient_id) REFERENCES Patients(id)
);

CREATE INDEX idx_claims_patient ON BillingClaims(patient_id);
CREATE INDEX idx_claims_status ON BillingClaims(status);
```

### ClaimLineItems

Individual services in a claim.

```sql
CREATE TABLE ClaimLineItems (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    claim_id UUID NOT NULL,
    
    cpt_code VARCHAR(10) NOT NULL,
    description VARCHAR(255),
    units INT NOT NULL DEFAULT 1,
    unit_price DECIMAL(12, 2) NOT NULL,
    total_price DECIMAL(12, 2) NOT NULL,
    
    FOREIGN KEY (claim_id) REFERENCES BillingClaims(id) ON DELETE CASCADE
);
```

---

## 📊 Audit Logging Tables

### AuditLogs

Comprehensive action tracking for compliance.

```sql
CREATE TABLE AuditLogs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- User Info
    user_id UUID NOT NULL,
    user_email VARCHAR(255),
    user_role VARCHAR(100),
    
    -- Action
    action VARCHAR(100) NOT NULL,
    resource_type VARCHAR(50) NOT NULL,
    resource_id VARCHAR(100),
    resource_name VARCHAR(255),
    
    -- Details
    status VARCHAR(50) DEFAULT 'success' CHECK (status IN ('success', 'failure', 'denied')),
    ip_address VARCHAR(45),
    user_agent TEXT,
    device_type VARCHAR(50),
    location VARCHAR(255),
    
    -- Changes
    changes JSONB,  -- Diff of before/after data
    
    -- Error info
    error_message TEXT,
    
    FOREIGN KEY (user_id) REFERENCES Users(id)
);

CREATE INDEX idx_audit_timestamp ON AuditLogs(timestamp DESC);
CREATE INDEX idx_audit_user ON AuditLogs(user_id);
CREATE INDEX idx_audit_resource ON AuditLogs(resource_type, resource_id);
CREATE INDEX idx_audit_action ON AuditLogs(action);
```

---

## 🔐 Session & Token Tables

### Sessions

Active user sessions.

```sql
CREATE TABLE Sessions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    
    -- Token info
    refresh_token_hash VARCHAR(255) NOT NULL UNIQUE,
    ip_address VARCHAR(45),
    user_agent TEXT,
    device_name VARCHAR(255),
    
    -- Expiry
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    last_activity_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Status
    is_active BOOLEAN DEFAULT TRUE,
    revoked_at TIMESTAMP WITH TIME ZONE,
    
    FOREIGN KEY (user_id) REFERENCES Users(id)
);

CREATE INDEX idx_sessions_user ON Sessions(user_id);
CREATE INDEX idx_sessions_expires ON Sessions(expires_at);
```

---

## 📈 Constraints & Integrity

### Check Constraints

```sql
-- Age validation for patients
ALTER TABLE Patients
ADD CONSTRAINT chk_patient_age
CHECK (EXTRACT(YEAR FROM AGE(dob)) >= 0 AND EXTRACT(YEAR FROM AGE(dob)) <= 150);

-- Vital signs ranges
ALTER TABLE VitalSigns
ADD CONSTRAINT chk_heart_rate
CHECK (heart_rate BETWEEN 30 AND 300);

ALTER TABLE VitalSigns
ADD CONSTRAINT chk_o2_sat
CHECK (oxygen_saturation BETWEEN 0 AND 100);

-- Insurance dates
ALTER TABLE Patients
ADD CONSTRAINT chk_insurance_dates
CHECK (insurance_effective_date <= CURRENT_DATE);

-- Appointment duration
ALTER TABLE Appointments
ADD CONSTRAINT chk_duration
CHECK (duration_minutes IN (15, 30, 45, 60, 90, 120));
```

### Referential Integrity

- ON DELETE CASCADE: Allergies, Conditions, Prescription Refills
- ON DELETE RESTRICT: Patients, Users, Appointments, Prescriptions
- ON DELETE SET NULL: Update records when user deleted

---

## 🔄 Stored Procedures & Views

### Useful Views

```sql
-- Active patients
CREATE VIEW vw_active_patients AS
SELECT * FROM Patients
WHERE status = 'active' AND deleted_at IS NULL;

-- Upcoming appointments
CREATE VIEW vw_upcoming_appointments AS
SELECT 
    a.id, a.patient_id, p.first_name, p.last_name,
    a.appointment_date, a.appointment_time,
    u.first_name as provider_name
FROM Appointments a
JOIN Patients p ON a.patient_id = p.id
JOIN Users u ON a.provider_id = u.id
WHERE a.appointment_date >= CURRENT_DATE
  AND a.status IN ('scheduled', 'checked-in')
ORDER BY a.appointment_date, a.appointment_time;

-- Patient medical timeline
CREATE VIEW vw_patient_timeline AS
SELECT 
    patient_id,
    record_date,
    'appointment' as event_type,
    'Appointment' as description
FROM Appointments
WHERE status IN ('completed', 'checked-in', 'in-progress')
UNION ALL
SELECT 
    patient_id,
    record_date,
    'medical_record' as event_type,
    CONCAT(record_type, ' - ', COALESCE(assessment, 'No assessment'))
FROM MedicalRecords
ORDER BY patient_id, record_date DESC;
```

---

## 📦 Migrations

Database migrations are versioned and tracked:

```
migrations/
├── 001_initial_schema.sql          (Users, Roles, Permissions)
├── 002_patients_module.sql         (Patients, Allergies, Conditions)
├── 003_appointments_module.sql     (Appointments)
├── 004_medical_records.sql         (Medical Records, Vitals)
├── 005_prescriptions_module.sql    (Prescriptions, Refills)
├── 006_billing_module.sql          (Claims, Line Items)
├── 007_audit_logging.sql           (Audit Logs)
├── 008_sessions_tokens.sql         (Sessions)
└── 009_indexes_optimization.sql    (Performance indexes)
```

Each migration includes:
- Forward script (migration up)
- Rollback script (migration down)
- Data validation checks
- Performance impact analysis

---

## 🛡️ Backup Strategy

```sql
-- Full backup (daily at 02:00 UTC)
BACKUP DATABASE ehr_platform
TO DISK = '/backups/full_$(TIMESTAMP).bak'
WITH COMPRESSION, STATS = 1;

-- Transaction log backup (every 15 minutes)
BACKUP LOG ehr_platform
TO DISK = '/backups/tlog_$(TIMESTAMP).bak'
WITH COMPRESSION;

-- Retention: 35 days (per HIPAA)
```

---

**Version**: 1.0.0 | Last Updated: July 2026
