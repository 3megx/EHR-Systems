# API Specification

Complete reference for all REST API endpoints and SignalR real-time connections.

---

## 📋 Base Information

**Base URL**: `https://api.moderneHRplatform.com/api/v1`  
**Authentication**: JWT Bearer Token  
**Response Format**: JSON  
**Rate Limit**: 1000 requests/hour per user  

### Example Request

```bash
curl -X GET https://api.moderneHRplatform.com/api/v1/patients/123 \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..." \
  -H "Content-Type: application/json"
```

---

## 🔐 Authentication Endpoints

### POST /auth/login

**Description**: Authenticate user and receive JWT tokens

**Request**:
```json
{
  "email": "doctor@hospital.com",
  "password": "SecurePassword123!"
}
```

**Response** (200):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiresIn": 3600,
  "user": {
    "id": "usr_001",
    "email": "doctor@hospital.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": "doctor",
    "permissions": ["patients:read", "prescriptions:create"]
  }
}
```

**Errors**:
- `401 Unauthorized`: Invalid credentials
- `429 Too Many Requests`: Too many login attempts

---

### POST /auth/refresh

**Description**: Refresh access token using refresh token

**Request**:
```json
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIs..."
}
```

**Response** (200):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiresIn": 3600
}
```

---

### POST /auth/logout

**Description**: Invalidate user session

**Response** (204): No Content

---

## 👥 Patient Endpoints

### GET /patients

**Description**: List all patients with pagination

**Query Parameters**:
```
?page=1
&pageSize=20
&search=John
&sortBy=lastName
&sortOrder=asc
```

**Response** (200):
```json
{
  "data": [
    {
      "id": "pat_001",
      "mrn": "MRN-2024-001",
      "firstName": "John",
      "lastName": "Smith",
      "dob": "1975-05-15",
      "gender": "M",
      "email": "john@example.com",
      "phone": "(555) 123-4567",
      "status": "active",
      "allergies": ["Penicillin", "Aspirin"],
      "conditions": ["Hypertension", "Type 2 Diabetes"],
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalRecords": 145,
    "totalPages": 8
  }
}
```

---

### GET /patients/{id}

**Description**: Get patient details by ID

**Response** (200):
```json
{
  "id": "pat_001",
  "mrn": "MRN-2024-001",
  "firstName": "John",
  "lastName": "Smith",
  "dob": "1975-05-15",
  "gender": "M",
  "email": "john@example.com",
  "phone": "(555) 123-4567",
  "ssn": "XXX-XX-1234",
  "address": "123 Main St, City, State 12345",
  "emergencyContact": {
    "name": "Jane Smith",
    "phone": "(555) 123-4568",
    "relationship": "Spouse"
  },
  "allergies": [
    {
      "id": "allg_001",
      "allergen": "Penicillin",
      "severity": "severe",
      "reaction": "Anaphylaxis"
    }
  ],
  "conditions": [
    {
      "id": "cond_001",
      "icd10Code": "I10",
      "condition": "Essential Hypertension",
      "status": "active",
      "onsetDate": "2010-03-20"
    }
  ],
  "insurance": {
    "provider": "Blue Cross Blue Shield",
    "memberId": "BCB123456789",
    "groupNumber": "GROUP123",
    "effectiveDate": "2024-01-01"
  },
  "status": "active",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-07-20T15:45:00Z"
}
```

---

### POST /patients

**Description**: Create new patient

**Request**:
```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "dob": "1985-07-22",
  "gender": "F",
  "email": "jane@example.com",
  "phone": "(555) 987-6543",
  "address": "456 Oak Ave, City, State 54321"
}
```

**Response** (201):
```json
{
  "id": "pat_002",
  "mrn": "MRN-2024-002",
  "firstName": "Jane",
  "lastName": "Doe",
  "status": "active",
  "createdAt": "2024-07-20T16:00:00Z"
}
```

---

### PUT /patients/{id}

**Description**: Update patient demographics

**Request**:
```json
{
  "phone": "(555) 987-6543",
  "address": "789 Pine Rd, City, State 98765"
}
```

**Response** (200): Updated patient object

---

### DELETE /patients/{id}

**Description**: Soft-delete patient (mark as inactive)

**Response** (204): No Content

---

### GET /patients/{id}/medical-history

**Description**: Get patient's medical timeline

**Query Parameters**:
```
?recordType=all  # all, soap, vitals, diagnosis
&startDate=2024-01-01
&endDate=2024-07-31
&limit=50
```

**Response** (200):
```json
{
  "data": [
    {
      "id": "rec_001",
      "type": "vital_signs",
      "date": "2024-07-20T14:30:00Z",
      "content": {
        "bloodPressure": "120/80",
        "heartRate": 72,
        "temperature": 98.6,
        "respiratoryRate": 16,
        "oxygenSaturation": 98
      },
      "recordedBy": "Nurse Sarah Johnson",
      "facility": "Downtown Hospital"
    },
    {
      "id": "rec_002",
      "type": "soap_note",
      "date": "2024-07-15T10:00:00Z",
      "content": {
        "subjective": "Patient reports slight headache...",
        "objective": "BP: 120/80, HR: 72",
        "assessment": "Tension headache",
        "plan": "Rest, hydration, follow-up in 1 week"
      },
      "createdBy": "Dr. Michael Chen"
    }
  ]
}
```

---

## 📅 Appointment Endpoints

### GET /appointments

**Description**: List appointments with filtering

**Query Parameters**:
```
?status=scheduled  # scheduled, completed, cancelled
&date=2024-08-01
&providerId=usr_005
&patientId=pat_001
```

**Response** (200):
```json
{
  "data": [
    {
      "id": "apt_001",
      "patientId": "pat_001",
      "patientName": "John Smith",
      "doctorId": "usr_005",
      "doctorName": "Dr. Sarah Lee",
      "specialty": "Cardiology",
      "datetime": "2024-08-05T14:00:00Z",
      "duration": 30,
      "status": "scheduled",
      "type": "follow-up",
      "location": "Clinic B, Room 201",
      "notes": "Monthly check-up",
      "reminders": {
        "email": true,
        "sms": true
      }
    }
  ]
}
```

---

### POST /appointments

**Description**: Create appointment

**Request**:
```json
{
  "patientId": "pat_001",
  "doctorId": "usr_005",
  "datetime": "2024-08-05T14:00:00Z",
  "duration": 30,
  "type": "follow-up",
  "notes": "Monthly check-up",
  "reminders": {
    "email": true,
    "sms": true,
    "daysBeforeAppointment": 1
  }
}
```

**Response** (201): Created appointment object

**Validations**:
- Doctor must be available at requested time
- Patient must not have conflicting appointments
- Cannot book > 90 days in advance
- Duration must be 15, 30, 45, or 60 minutes

---

### GET /appointments/{id}

**Description**: Get appointment details

**Response** (200): Appointment object with full details

---

### PUT /appointments/{id}

**Description**: Update appointment

**Request**:
```json
{
  "datetime": "2024-08-05T15:00:00Z",
  "notes": "Rescheduled per patient request"
}
```

**Response** (200): Updated appointment

---

### POST /appointments/{id}/cancel

**Description**: Cancel appointment

**Request**:
```json
{
  "reason": "Patient requested cancellation",
  "notifyPatient": true
}
```

**Response** (200): Cancellation confirmation

---

### GET /appointments/availability

**Description**: Get doctor's available time slots

**Query Parameters**:
```
?doctorId=usr_005
&date=2024-08-05
&duration=30
```

**Response** (200):
```json
{
  "doctorId": "usr_005",
  "date": "2024-08-05",
  "availableSlots": [
    "09:00:00",
    "09:30:00",
    "10:00:00",
    "14:00:00",
    "14:30:00",
    "15:00:00"
  ]
}
```

---

## 💊 Prescription Endpoints

### GET /prescriptions

**Description**: List prescriptions

**Query Parameters**:
```
?status=active  # active, completed, cancelled
&patientId=pat_001
```

**Response** (200):
```json
{
  "data": [
    {
      "id": "prx_001",
      "patientId": "pat_001",
      "patientName": "John Smith",
      "medicationName": "Lisinopril",
      "dosage": "10mg",
      "frequency": "Once daily",
      "route": "Oral",
      "quantity": 30,
      "unit": "tablets",
      "refills": 3,
      "issuedDate": "2024-07-20T10:00:00Z",
      "expiryDate": "2024-10-20T23:59:59Z",
      "status": "active",
      "doctorId": "usr_005",
      "doctorName": "Dr. Sarah Lee",
      "indication": "Hypertension",
      "sideEffects": "Dizziness, dry cough",
      "interactions": []
    }
  ]
}
```

---

### POST /prescriptions

**Description**: Create prescription

**Request**:
```json
{
  "patientId": "pat_001",
  "medicationName": "Metformin",
  "dosage": "500mg",
  "frequency": "Twice daily",
  "route": "Oral",
  "quantity": 60,
  "unit": "tablets",
  "refills": 6,
  "indication": "Type 2 Diabetes",
  "notes": "Take with food"
}
```

**Response** (201): Created prescription

**Validations**:
- Check for drug interactions
- Verify patient allergies
- Validate dosage ranges
- Check for duplicate active prescriptions

---

### POST /prescriptions/{id}/refill

**Description**: Request prescription refill

**Request**:
```json
{
  "quantity": 30,
  "reason": "Routine refill"
}
```

**Response** (201): New prescription object

---

### GET /prescriptions/interactions

**Description**: Check drug interactions

**Query Parameters**:
```
?medications=Lisinopril,Ibuprofen,Aspirin
```

**Response** (200):
```json
{
  "medications": ["Lisinopril", "Ibuprofen", "Aspirin"],
  "interactions": [
    {
      "drug1": "Ibuprofen",
      "drug2": "Aspirin",
      "severity": "high",
      "description": "Increased risk of GI bleeding"
    }
  ]
}
```

---

## 📊 Medical Records Endpoints

### GET /medical-records

**Description**: List medical records

**Query Parameters**:
```
?patientId=pat_001
&type=soap  # soap, vitals, diagnosis, procedure
&startDate=2024-01-01
&endDate=2024-07-31
```

**Response** (200): Array of medical records

---

### POST /medical-records

**Description**: Create medical record (SOAP note)

**Request**:
```json
{
  "patientId": "pat_001",
  "type": "soap",
  "recordDate": "2024-07-20T14:30:00Z",
  "subjective": "Patient presents with complaint of chest pain...",
  "objective": "Vital Signs: BP 130/85, HR 88, Temp 98.6°F\nPhysical Exam: Clear lungs, regular heart rhythm",
  "assessment": "Probable anxiety-related chest pain. Rule out cardiac cause.",
  "plan": "Order EKG, refer to cardiology if persistent",
  "diagnoses": [
    {
      "icd10Code": "R07.9",
      "description": "Chest pain, unspecified"
    }
  ]
}
```

**Response** (201): Created record

---

### POST /medical-records/vitals

**Description**: Record patient vital signs

**Request**:
```json
{
  "patientId": "pat_001",
  "recordDate": "2024-07-20T14:30:00Z",
  "bloodPressure": "120/80",
  "heartRate": 72,
  "temperature": 98.6,
  "respiratoryRate": 16,
  "oxygenSaturation": 98,
  "weight": 185,
  "height": 70,
  "notes": "Routine vitals check"
}
```

**Response** (201): Vital signs record

---

## 💰 Billing Endpoints

### GET /billing/claims

**Description**: List billing claims

**Query Parameters**:
```
?status=pending  # pending, submitted, denied, paid
&patientId=pat_001
&startDate=2024-01-01
```

**Response** (200):
```json
{
  "data": [
    {
      "id": "clm_001",
      "patientId": "pat_001",
      "patientName": "John Smith",
      "claimNumber": "CLM-2024-001",
      "serviceDate": "2024-07-15",
      "totalAmount": 500.00,
      "insurance": "Blue Cross Blue Shield",
      "status": "submitted",
      "submittedDate": "2024-07-18T10:00:00Z",
      "services": [
        {
          "cptCode": "99213",
          "description": "Office visit - established patient",
          "units": 1,
          "unitPrice": 150.00,
          "totalPrice": 150.00
        }
      ]
    }
  ]
}
```

---

### GET /billing/invoices

**Description**: List patient invoices

**Response** (200): Array of invoice objects

---

## 📈 Analytics Endpoints

### GET /analytics/dashboard

**Description**: Get dashboard analytics

**Query Parameters**:
```
?startDate=2024-01-01
&endDate=2024-07-31
```

**Response** (200):
```json
{
  "patientMetrics": {
    "totalPatients": 1250,
    "activePatients": 1100,
    "newPatientsThisMonth": 45,
    "patientsByGender": {
      "M": 620,
      "F": 630
    }
  },
  "appointmentMetrics": {
    "totalAppointments": 3500,
    "completedAppointments": 3420,
    "cancelledAppointments": 80,
    "noShowAppointments": 50,
    "averageWaitTime": 15
  },
  "prescriptionMetrics": {
    "totalPrescriptions": 2100,
    "activePrescriptions": 1850,
    "refillRequests": 250
  },
  "topConditions": [
    {
      "condition": "Hypertension",
      "count": 380,
      "percentage": 30.4
    }
  ]
}
```

---

### GET /analytics/reports

**Description**: Generate compliance & population health reports

**Query Parameters**:
```
?reportType=quality_metrics  # quality_metrics, population_health, financial
&startDate=2024-01-01
&endDate=2024-07-31
&format=pdf  # pdf, csv, json
```

**Response** (200): Report data or file

---

## 🔔 SignalR Real-Time Endpoints

### WebSocket Connection

**Endpoint**: `wss://api.moderneHRplatform.com/hubs/notifications`

**Authentication**: JWT token in query string or header

### Events

#### 1. Patient Vitals Update

**Emit**:
```json
{
  "method": "VitalSignsUpdated",
  "args": {
    "patientId": "pat_001",
    "bloodPressure": "125/82",
    "heartRate": 75,
    "timestamp": "2024-07-20T14:35:00Z"
  }
}
```

#### 2. Appointment Reminder

**Emit**:
```json
{
  "method": "AppointmentReminder",
  "args": {
    "patientId": "pat_001",
    "appointmentId": "apt_001",
    "datetime": "2024-08-05T14:00:00Z",
    "minutesUntilAppointment": 60
  }
}
```

#### 3. Prescription Refill Request

**Emit**:
```json
{
  "method": "PrescriptionRefillRequest",
  "args": {
    "patientId": "pat_001",
    "prescriptionId": "prx_001",
    "medicationName": "Lisinopril",
    "timestamp": "2024-07-20T14:40:00Z"
  }
}
```

---

## ⚠️ Error Responses

### Standard Error Format

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid request parameters",
    "details": [
      {
        "field": "email",
        "message": "Email is required"
      }
    ],
    "timestamp": "2024-07-20T14:45:00Z",
    "requestId": "req_abc123"
  }
}
```

### HTTP Status Codes

| Code | Meaning | Example |
|------|---------|---------|
| 200 | Success | Patient retrieved |
| 201 | Created | Appointment scheduled |
| 204 | No Content | Successful deletion |
| 400 | Bad Request | Invalid parameters |
| 401 | Unauthorized | Invalid/expired token |
| 403 | Forbidden | Insufficient permissions |
| 404 | Not Found | Patient not found |
| 409 | Conflict | Duplicate MRN |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Server Error | Internal error |

---

## 📚 SDKs & Client Libraries

- **JavaScript/TypeScript**: `@moderneHR/sdk` (npm)
- **C#/.NET**: `ModernEHR.Sdk` (NuGet)
- **Python**: `modern_ehr` (pip)
- **Java**: `com.moderneHR:sdk` (Maven)

---

**Version**: 1.0.0 | Last Updated: July 2026
