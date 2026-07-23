# Build Large-Scale Angular Application for EHR (Electronic Health Records) System

**Project Name**: Modern EHR Platform  
**Tech Stack**: Angular 18+ (standalone components, signals), Tailwind CSS, TypeScript, NgRx (or Signals + NgRx SignalStore for state), RxJS, i18n (EN + AR + RTL support).

**Goal**: Create a **highly scalable, secure, and maintainable** frontend for a full-featured Electronic Health Records system using the enterprise feature-based architecture.

## Core Requirements

### 1. Large-Scale Project Structure (MANDATORY)

Use this exact folder structure:

```
src/app/
├── core/                          # App-wide singletons
│   ├── auth/                      # Auth service, guards, interceptor
│   ├── services/                  # Global services (API base, logging, notification, websocket)
│   ├── guards/
│   ├── interceptors/
│   ├── models/                    # Global interfaces (User, Role, PatientBase, etc.)
│   └── core.config.ts

├── shared/                        # Reusable across all features
│   ├── components/
│   │   ├── ui/                    # Button, Table, Modal, Form controls, Card, etc.
│   │   ├── layout/                # Sidebar, Topbar, Patient Header, Breadcrumbs
│   │   └── common/                # DataTable, ChartWrapper, FileUploader, Timeline
│   ├── pipes/
│   ├── directives/                # Permission, Highlight, etc.
│   ├── widgets/                   # Vitals Card, Lab Results Summary
│   └── shared.module.ts           # (optional barrel)

├── features/                      # Business domains (lazy-loaded)
│   ├── auth/
│   ├── dashboard/
│   ├── patients/
│   │   ├── components/ (list, detail, search, timeline)
│   │   ├── pages/
│   │   ├── store/                 # NgRx or SignalStore
│   │   ├── services/
│   │   ├── models/
│   │   └── patients.routes.ts
│   ├── appointments/
│   ├── medical-records/
│   ├── prescriptions/
│   ├── lab-results/
│   ├── billing/
│   ├── reports-analytics/
│   ├── admin/                     # RBAC, users, settings
│   └── ... (add more as needed)

├── layouts/                       # Shell layouts
│   ├── main-layout/
│   ├── auth-layout/
│   └── print-layout/              # For reports/PDF

├── routes/
│   └── app.routes.ts              # Top-level lazy routes

├── app.component.ts
└── app.config.ts
```

### 2. EHR-Specific Features & Best Practices

**Must-Have Modules** (Implement with high quality):
- **Patients** — Search, demographics, history, timeline, allergies, chronic conditions.
- **Appointments** — Scheduling, calendar (use FullCalendar or PrimeNG), reminders.
- **Medical Records** — SOAP notes, vitals, diagnoses (ICD-10), procedures (CPT).
- **Prescriptions** — eRx, medication history, interactions check.
- **Lab & Imaging** — Results, trends, PDF/viewer integration.
- **Billing & Insurance** — Claims, payments.
- **Reports & Analytics** — Population health, compliance reports.
- **Real-time** — Live vitals, chat/consult notes (SignalR/WebSocket).
- **Security & Compliance** — HIPAA-ready patterns (audit logs, consent, role-based access, data masking).

**Key Technical Practices**:
- Standalone components everywhere + OnPush change detection.
- Signals for local state + NgRx for complex global state.
- Strong typing (interfaces for every entity).
- Responsive + Accessible (WCAG AA) design with Tailwind.
- i18n with Angular built-in (EN + Arabic + RTL).
- Lazy loading for all features.
- Error handling, loading states, toast notifications.
- Dark mode support.
- PWA capabilities if possible.
- Mock data / fake backend initially (switch to real API later).

### 3. UI/UX Focus for EHR
- Clean, professional medical UI (blues/greens, high contrast).
- Patient header sticky with key info.
- Timeline view for medical history.
- Fast search across patients/records.
- Keyboard shortcuts for clinicians.
- Mobile-friendly for tablets/wards.

### 4. Development Workflow
1. Initialize Angular app with Tailwind + standalone setup.
2. Create the full folder structure.
3. Implement **Shared** UI components first.
4. Implement **Core** services (API, Auth).
5. Build **Patients** feature completely as reference (with store, routing, components).
6. Apply the same high-quality pattern to other features.
7. Ensure all code is clean, documented, and follows Angular style guide.

**Start by** showing:
- The created folder structure.
- Updated `angular.json`, `package.json`, Tailwind config.
- `app.routes.ts` example with lazy loading.
- First few shared components and the Patients feature skeleton.

Focus on **enterprise quality**: performance, accessibility, maintainability, security patterns. Make it production-ready from day one.

---

## Technical Stack Details

### Frameworks & Libraries
- **Angular**: 18.0+ (standalone components)
- **Tailwind CSS**: 3.4+
- **NgRx**: 17+ (for global state)
- **RxJS**: 7.8+
- **TypeScript**: 5.2+
- **Angular i18n**: Built-in for EN/AR + RTL
- **FullCalendar** or **PrimeNG**: For calendar/scheduling
- **Chart.js/ng2-charts**: For analytics dashboards
- **ng-zorro or PrimeNG**: Prebuilt components (optional)

### Key Dependencies (package.json)
```json
{
  "dependencies": {
    "@angular/common": "^18.0.0",
    "@angular/compiler": "^18.0.0",
    "@angular/core": "^18.0.0",
    "@angular/forms": "^18.0.0",
    "@angular/platform-browser": "^18.0.0",
    "@angular/platform-browser-dynamic": "^18.0.0",
    "@angular/router": "^18.0.0",
    "@ngrx/store": "^17.0.0",
    "@ngrx/effects": "^17.0.0",
    "@ngrx/store-devtools": "^17.0.0",
    "rxjs": "^7.8.0",
    "tailwindcss": "^3.4.0",
    "postcss": "^8.4.0",
    "autoprefixer": "^10.4.0",
    "@fullcalendar/angular": "^6.1.0",
    "@fullcalendar/daygrid": "^6.1.0",
    "@fullcalendar/timegrid": "^6.1.0",
    "@fullcalendar/interaction": "^6.1.0",
    "chart.js": "^4.4.0",
    "ng2-charts": "^4.1.0",
    "signalr": "latest"
  },
  "devDependencies": {
    "@angular/cli": "^18.0.0",
    "@angular/compiler-cli": "^18.0.0",
    "typescript": "^5.2.0",
    "@types/node": "^20.0.0",
    "prettier": "^3.0.0",
    "eslint": "^8.50.0",
    "@typescript-eslint/eslint-plugin": "^6.0.0"
  }
}
```

### Configuration Standards

**tsconfig.json** (strict mode):
```json
{
  "compilerOptions": {
    "strict": true,
    "noImplicitAny": true,
    "strictNullChecks": true,
    "strictFunctionTypes": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noImplicitReturns": true
  }
}
```

**Tailwind config**: Include custom healthcare color palette, breakpoints for medical UI.

**ESLint + Prettier**: Enforce consistent code style across the team.

---

## Core Architecture Patterns

### 1. Standalone Components (MANDATORY)
Every component must be standalone:
```typescript
@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [CommonModule, FormsModule, NgIf, NgFor, ...]
  // No module declarations
})
```

### 2. Signals for Local State
Use Angular Signals for reactive local component state:
```typescript
patientCount = signal(0);
isLoading = signal(false);
selectedPatient = signal<Patient | null>(null);
```

### 3. NgRx for Global State
For complex cross-feature state (auth, global settings, notifications):
```
store/
├── auth/
│   ├── auth.state.ts
│   ├── auth.actions.ts
│   ├── auth.reducer.ts
│   └── auth.effects.ts
├── patients/
│   └── ...
```

### 4. Services (API + Domain Logic)
```
services/
├── api/
│   ├── api.service.ts          # Base HTTP client
│   ├── patient.api.service.ts
│   └── appointment.api.service.ts
├── domain/
│   ├── patient.service.ts      # Business logic
│   ├── auth.service.ts
│   └── notification.service.ts
└── utils/
    ├── logger.service.ts
    └── error-handler.service.ts
```

### 5. Guards & Interceptors
- **Auth Guard**: Protect routes, check JWT validity
- **Role Guard**: RBAC enforcement
- **Consent Guard**: HIPAA consent checking
- **Auth Interceptor**: Auto-inject JWT tokens
- **Error Interceptor**: Global error handling + retry logic
- **Logging Interceptor**: Request/response logging (audit compliance)

### 6. Error Handling
Custom exception hierarchy for EHR-specific errors:
```typescript
- AuthenticationError
- AuthorizationError
- ValidationError
- HIPAAComplianceError
- DataNotFoundError
- ConflictError
```

### 7. i18n (Internationalization)
Support EN (English) + AR (Arabic) with RTL layout:
- `src/locale/messages.en.json`
- `src/locale/messages.ar.json`
- LTR by default, RTL for Arabic

---

## Feature-Specific Guidelines

### Patients Feature
**Responsibilities**: Patient search, CRUD, demographics, history timeline.

**Structure**:
```
patients/
├── pages/
│   ├── patient-list-page/
│   ├── patient-detail-page/
│   └── patient-create-page/
├── components/
│   ├── patient-search/
│   ├── patient-timeline/
│   ├── patient-header/
│   ├── allergies-section/
│   └── chronic-conditions-section/
├── store/
│   ├── patient.state.ts
│   ├── patient.actions.ts
│   ├── patient.reducer.ts
│   └── patient.effects.ts
├── services/
│   ├── patient.service.ts
│   └── patient.api.service.ts
├── models/
│   ├── patient.model.ts
│   └── patient-search.model.ts
└── patients.routes.ts
```

**Key Features**:
- MRN (Medical Record Number) search
- Advanced filtering (age, gender, insurance, conditions)
- Timeline view of visits/records
- Allergies & drug interactions check
- Demographic editing with audit trail
- Soft deletes + recovery

### Appointments Feature
**Responsibilities**: Scheduling, availability, reminders, calendar view.

**Structure**:
```
appointments/
├── pages/
│   ├── appointment-list-page/
│   ├── appointment-calendar-page/
│   └── appointment-create-page/
├── components/
│   ├── appointment-calendar/ (FullCalendar wrapper)
│   ├── appointment-form/
│   ├── availability-picker/
│   └── appointment-timeline/
├── store/
│   └── appointment.state.ts, actions, reducer, effects
├── services/
│   ├── appointment.service.ts
│   └── appointment.api.service.ts
├── models/
│   └── appointment.model.ts
└── appointments.routes.ts
```

**Key Features**:
- Calendar view with drag-drop rescheduling
- Provider availability slots
- Appointment reminders (email/SMS integration via backend)
- Cancellation + rebooking
- No-show tracking

### Medical Records Feature
**Responsibilities**: SOAP notes, vitals, diagnoses, procedures.

**Structure**:
```
medical-records/
├── pages/
│   ├── records-list-page/
│   ├── soap-note-page/
│   └── vitals-page/
├── components/
│   ├── soap-note-editor/
│   ├── vitals-chart/
│   ├── diagnosis-selector/ (ICD-10)
│   └── procedure-selector/ (CPT)
├── services/
│   └── medical-record.service.ts
├── models/
│   └── medical-record.model.ts
└── medical-records.routes.ts
```

**Key Features**:
- Rich text SOAP notes (with spell check)
- Vital signs trending charts
- ICD-10 diagnosis code picker
- CPT procedure codes
- Searchable history

### Prescriptions Feature
**Responsibilities**: eRx, medication history, interactions.

**Structure**:
```
prescriptions/
├── pages/
│   ├── prescription-list-page/
│   └── prescription-create-page/
├── components/
│   ├── medication-picker/
│   ├── interactions-checker/
│   ├── refill-request/
│   └── pharmacy-selector/
├── services/
│   └── prescription.service.ts
├── models/
│   └── prescription.model.ts
└── prescriptions.routes.ts
```

**Key Features**:
- Medication search with NPM/RxNorm codes
- Drug-drug interaction checking
- Refill management
- Pharmacy routing
- Signature workflow

### Lab & Imaging Feature
**Responsibilities**: Lab results, imaging orders, trending.

**Structure**:
```
lab-results/
├── pages/
│   ├── lab-results-list-page/
│   └── lab-details-page/
├── components/
│   ├── lab-trending-chart/
│   ├── result-details/
│   └── imaging-viewer/
├── services/
│   └── lab.service.ts
├── models/
│   └── lab-result.model.ts
└── lab-results.routes.ts
```

**Key Features**:
- Results with reference ranges
- Trending over time
- Imaging viewer (PDF/DICOM)
- Abnormal flag alerts
- Export/PDF generation

### Reports & Analytics Feature
**Responsibilities**: Dashboards, population health, compliance reports.

**Structure**:
```
reports-analytics/
├── pages/
│   ├── dashboard-page/
│   ├── population-health-page/
│   └── compliance-reports-page/
├── components/
│   ├── kpi-card/
│   ├── chart-wrapper/
│   └── report-generator/
├── services/
│   └── analytics.service.ts
└── reports-analytics.routes.ts
```

**Key Features**:
- KPI dashboards (patient volume, provider stats)
- Population health metrics
- Compliance report generation
- Export to PDF/Excel

---

## Security & Compliance Patterns

### 1. HIPAA-Ready Architecture
- **Audit Logging**: Track all PHI access via interceptor
- **Role-Based Access Control (RBAC)**: Guards enforce permission checks
- **Consent Management**: Verify patient consent before data display
- **Data Masking**: Mask SSN, DOB on public displays
- **Encryption in Transit**: HTTPS only, TLS 1.3+

### 2. Authentication & Authorization
- JWT stored in secure HTTP-only cookies (not localStorage)
- Token refresh via refresh endpoint
- Logout clears all sensitive data
- MFA support (TOTP/SMS)

### 3. Error Handling for Compliance
- Never expose system/database errors to UI
- Log all errors server-side for audit
- Show user-friendly messages
- Rate limiting to prevent brute force

---

## Development & Deployment

### Local Development
```bash
ng serve --open
```

### Production Build
```bash
ng build --configuration production
```

### Testing (Jest + Testing Library)
```bash
npm test                  # Unit tests
npm run test:integration # Integration tests
npm run test:e2e         # E2E tests
```

### Docker Deployment
```dockerfile
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build -- --configuration production

FROM nginx:alpine
COPY --from=build /app/dist/ehr /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

---

## Code Quality Standards

### TypeScript Strict Mode
- No `any` types
- Explicit return types
- Null safety checks

### Component Guidelines
- Single Responsibility Principle (SRP)
- Max 300 lines per component
- Inputs are read-only
- OnPush change detection by default
- Unsubscribe from observables (use `takeUntilDestroyed`)

### Naming Conventions
- Classes: PascalCase
- Variables/functions: camelCase
- Constants: UPPER_SNAKE_CASE
- Private members: prefix with `_`
- Signals: suffix with `$` (e.g., `isLoading$`)

### Documentation
- JSDoc comments for public methods
- Type descriptions in interfaces
- Examples for complex functions
- README.md in each feature folder

---

## Success Criteria for Production-Ready Frontend

✅ **Functionality**
- All EHR features work end-to-end
- Responsive on desktop, tablet, mobile
- Real-time updates via WebSocket

✅ **Performance**
- Lighthouse score >90
- Initial load <3s
- LCP < 2.5s
- Lazy loading for all routes
- No memory leaks

✅ **Accessibility**
- WCAG AA compliant
- Keyboard navigation throughout
- Screen reader support
- Color contrast >4.5:1

✅ **Security**
- No console errors/warnings
- CSP headers configured
- XSS protection
- CSRF protection
- Secure API token handling

✅ **Testing**
- >80% code coverage
- All critical paths tested
- Integration tests for features
- E2E tests for user journeys

✅ **Maintainability**
- Clean, documented code
- Consistent patterns
- No technical debt
- Easy onboarding for new developers

---

## Implementation Priority

1. **Phase 1**: Project setup + Shared UI components
2. **Phase 2**: Core services + Auth feature
3. **Phase 3**: Patients feature (reference implementation)
4. **Phase 4**: Remaining features (appointments, medical records, etc.)
5. **Phase 5**: Dashboards + Reports
6. **Phase 6**: Testing + Performance optimization
7. **Phase 7**: Deployment + Documentation

---

## Begin Now

Start with:
1. Create the complete folder structure
2. Set up Tailwind + Angular configuration
3. Implement shared UI components (Button, Card, Table, Modal, Form controls)
4. Implement core services (API, Auth, Notification)
5. Build the Patients feature as reference implementation

This is an enterprise-grade EHR frontend. Every line of code should be production-ready. Focus on clarity, security, performance, and accessibility.

**Let's build it right.**
