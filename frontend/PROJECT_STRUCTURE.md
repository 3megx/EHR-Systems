# Modern EHR Platform - Project Structure

## Overview

Enterprise-grade Electronic Health Records (EHR) system frontend built with Angular 18+, Tailwind CSS, and NgRx Signals for state management.

## Complete Directory Structure

```
frontend/
├── src/
│   ├── app/
│   │
│   ├── core/                           # App-wide singletons (no lazy loading)
│   │   ├── auth/                       # Authentication module
│   │   │   └── [auth service files]
│   │   │
│   │   ├── services/                   # Global services
│   │   │   ├── api.service.ts         # Base HTTP service
│   │   │   ├── auth.service.ts        # Authentication service
│   │   │   ├── theme.service.ts       # Dark/light theme management
│   │   │   └── notification.service.ts # Toast notifications
│   │   │
│   │   ├── guards/                     # Route protection
│   │   │   ├── auth.guard.ts          # Authentication guard
│   │   │   └── role.guard.ts          # Role-based access control
│   │   │
│   │   ├── interceptors/               # HTTP interceptors
│   │   │   ├── auth.interceptor.ts    # Token injection
│   │   │   └── error.interceptor.ts   # Global error handling
│   │   │
│   │   ├── models/                     # Global interfaces/types
│   │   │   ├── user.model.ts          # User, Role, Permission
│   │   │   ├── patient.model.ts       # Patient and related models
│   │   │   └── index.ts               # Barrel export
│   │   │
│   │   └── core.config.ts             # Core module configuration
│   │
│   ├── shared/                         # Reusable across all features
│   │   ├── components/
│   │   │   ├── ui/                     # Basic UI components
│   │   │   │   ├── button/
│   │   │   │   │   └── button.component.ts
│   │   │   │   ├── card/
│   │   │   │   │   └── card.component.ts
│   │   │   │   ├── modal/
│   │   │   │   ├── form-field/
│   │   │   │   ├── table/
│   │   │   │   ├── pagination/
│   │   │   │   ├── tabs/
│   │   │   │   └── dropdown/
│   │   │   │
│   │   │   ├── layout/                 # Layout components
│   │   │   │   ├── sidebar/
│   │   │   │   ├── topbar/
│   │   │   │   ├── breadcrumbs/
│   │   │   │   └── patient-header/
│   │   │   │
│   │   │   └── common/                 # Domain-specific shared components
│   │   │       ├── data-table/
│   │   │       ├── chart-wrapper/
│   │   │       ├── file-uploader/
│   │   │       ├── timeline/
│   │   │       ├── vitals-card/
│   │   │       └── lab-results-summary/
│   │   │
│   │   ├── pipes/                      # Custom pipes
│   │   │   ├── safe.pipe.ts           # Sanitization
│   │   │   ├── date-format.pipe.ts
│   │   │   ├── currency.pipe.ts
│   │   │   └── phone-format.pipe.ts
│   │   │
│   │   ├── directives/                 # Custom directives
│   │   │   ├── has-permission.directive.ts
│   │   │   ├── highlight.directive.ts
│   │   │   └── loading.directive.ts
│   │   │
│   │   ├── widgets/                    # Complex reusable widgets
│   │   │   ├── vitals-card/
│   │   │   └── lab-results-summary/
│   │   │
│   │   └── shared.module.ts            # Optional barrel export
│   │
│   ├── features/                       # Business domains (lazy-loaded)
│   │   │
│   │   ├── auth/                       # Authentication feature
│   │   │   ├── pages/
│   │   │   │   ├── login-page/
│   │   │   │   │   └── login-page.component.ts
│   │   │   │   ├── register-page/
│   │   │   │   │   └── register-page.component.ts
│   │   │   │   ├── forgot-password-page/
│   │   │   │   │   └── forgot-password-page.component.ts
│   │   │   │   └── reset-password-page/
│   │   │   │       └── reset-password-page.component.ts
│   │   │   ├── components/
│   │   │   ├── services/
│   │   │   ├── models/
│   │   │   └── auth.routes.ts          # Feature routes (optional)
│   │   │
│   │   ├── dashboard/                  # Dashboard feature
│   │   │   ├── pages/
│   │   │   │   └── dashboard-page/
│   │   │   │       └── dashboard-page.component.ts
│   │   │   ├── components/
│   │   │   ├── services/
│   │   │   ├── store/                  # Feature store (if using NgRx)
│   │   │   └── models/
│   │   │
│   │   ├── patients/                   # Patient management feature
│   │   │   ├── pages/
│   │   │   │   ├── patient-list-page/
│   │   │   │   │   └── patient-list-page.component.ts
│   │   │   │   ├── patient-search-page/
│   │   │   │   │   └── patient-search-page.component.ts
│   │   │   │   ├── patient-detail-page/
│   │   │   │   │   └── patient-detail-page.component.ts
│   │   │   │   └── patient-timeline-page/
│   │   │   │       └── patient-timeline-page.component.ts
│   │   │   ├── components/
│   │   │   │   ├── patient-form/
│   │   │   │   ├── patient-card/
│   │   │   │   ├── allergy-list/
│   │   │   │   └── conditions-list/
│   │   │   ├── store/                  # NgRx state management
│   │   │   │   ├── patient.actions.ts
│   │   │   │   ├── patient.reducer.ts
│   │   │   │   ├── patient.effects.ts
│   │   │   │   └── patient.selectors.ts
│   │   │   ├── services/
│   │   │   │   └── patient.service.ts
│   │   │   ├── models/
│   │   │   │   ├── patient.model.ts
│   │   │   │   └── patient-query.model.ts
│   │   │   └── patients.routes.ts      # Feature routes
│   │   │
│   │   ├── appointments/               # Appointment scheduling
│   │   │   ├── pages/
│   │   │   │   ├── appointment-list-page/
│   │   │   │   ├── appointment-schedule-page/
│   │   │   │   └── appointment-detail-page/
│   │   │   ├── components/
│   │   │   ├── store/
│   │   │   ├── services/
│   │   │   ├── models/
│   │   │   └── appointments.routes.ts
│   │   │
│   │   ├── medical-records/            # Clinical records & SOAP notes
│   │   │   ├── pages/
│   │   │   │   ├── medical-records-page/
│   │   │   │   └── record-detail-page/
│   │   │   ├── components/
│   │   │   ├── store/
│   │   │   ├── services/
│   │   │   ├── models/
│   │   │   └── medical-records.routes.ts
│   │   │
│   │   ├── prescriptions/               # Electronic prescriptions
│   │   │   ├── pages/
│   │   │   │   ├── prescription-list-page/
│   │   │   │   ├── prescription-create-page/
│   │   │   │   └── prescription-detail-page/
│   │   │   ├── components/
│   │   │   ├── store/
│   │   │   ├── services/
│   │   │   ├── models/
│   │   │   └── prescriptions.routes.ts
│   │   │
│   │   ├── lab-results/                # Lab results & imaging
│   │   │   ├── pages/
│   │   │   │   ├── lab-results-page/
│   │   │   │   └── lab-result-detail-page/
│   │   │   ├── components/
│   │   │   ├── store/
│   │   │   ├── services/
│   │   │   ├── models/
│   │   │   └── lab-results.routes.ts
│   │   │
│   │   ├── billing/                    # Billing & insurance
│   │   │   ├── pages/
│   │   │   │   ├── billing-page/
│   │   │   │   └── invoice-list-page/
│   │   │   ├── components/
│   │   │   ├── store/
│   │   │   ├── services/
│   │   │   ├── models/
│   │   │   └── billing.routes.ts
│   │   │
│   │   ├── reports-analytics/          # Population health & compliance
│   │   │   ├── pages/
│   │   │   │   ├── reports-page/
│   │   │   │   ├── population-health-page/
│   │   │   │   └── compliance-page/
│   │   │   ├── components/
│   │   │   ├── store/
│   │   │   ├── services/
│   │   │   ├── models/
│   │   │   └── reports-analytics.routes.ts
│   │   │
│   │   └── admin/                      # System administration
│   │       ├── pages/
│   │       │   ├── admin-dashboard-page/
│   │       │   ├── user-management-page/
│   │       │   ├── role-management-page/
│   │       │   ├── settings-page/
│   │       │   └── audit-logs-page/
│   │       ├── components/
│   │       ├── store/
│   │       ├── services/
│   │       ├── models/
│   │       └── admin.routes.ts
│   │
│   ├── layouts/                        # Shell layouts (not lazy)
│   │   ├── main-layout/
│   │   │   └── main-layout.component.ts
│   │   ├── auth-layout/
│   │   │   └── auth-layout.component.ts
│   │   └── print-layout/
│   │       └── print-layout.component.ts
│   │
│   ├── routes/
│   │   └── app.routes.ts               # Top-level lazy routes
│   │
│   ├── store/
│   │   └── app.reducer.ts              # Root NgRx reducers
│   │
│   ├── app.component.ts                # Root component
│   ├── app.config.ts                   # App configuration
│   │
│   ├── assets/
│   │   ├── i18n/                       # Internationalization
│   │   │   ├── en.json
│   │   │   └── ar.json
│   │   ├── icons/
│   │   ├── images/
│   │   └── styles/
│   │
│   ├── environments/
│   │   ├── environment.ts              # Development config
│   │   └── environment.prod.ts         # Production config
│   │
│   ├── index.html
│   ├── main.ts
│   ├── styles.scss
│   └── test.ts
│
├── Configuration Files
│   ├── angular.json                    # Angular CLI config
│   ├── tsconfig.json                   # TypeScript config
│   ├── tsconfig.app.json
│   ├── tsconfig.spec.json
│   ├── tailwind.config.js              # Tailwind CSS config
│   ├── postcss.config.js
│   ├── karma.conf.js                   # Test runner config
│   ├── package.json
│   ├── .gitignore
│   ├── README.md
│   └── PROJECT_STRUCTURE.md            # This file
```

## Architecture Highlights

### 1. **Core Module** (`/core`)
- **Singleton services** - Auth, API, Theme, Notifications
- **Route guards** - Auth and Role-based protection
- **HTTP interceptors** - Token injection, error handling
- **Global models** - User, Role, Patient base structures

### 2. **Shared Module** (`/shared`)
- **UI Components** - Button, Card, Modal, Table, Form controls
- **Layout Components** - Sidebar, Topbar, Breadcrumbs
- **Pipes** - Date formatting, Phone formatting, Currency, Safe HTML
- **Directives** - Permission checking, Highlighting, Loading states
- **Widgets** - Complex reusable components (Vitals, Lab Results)

### 3. **Features** (`/features`)
Each feature is **completely isolated** and **lazy-loaded**:
- **Pages** - Container components for routes
- **Components** - Feature-specific UI components
- **Services** - Feature-specific business logic
- **Store** - NgRx state management (optional per feature)
- **Models** - Feature-specific interfaces

### 4. **State Management**
- **NgRx Signals** - For feature state management
- **NgRx Store** - For complex global state
- **Signals API** - For local component state
- **Router Store** - For route-related state

### 5. **Routing**
- **Lazy loading** - All features loaded on-demand
- **Preloading strategy** - PreloadAllModules for optimal UX
- **Route guards** - Auth and role-based protection
- **Error handling** - Global error interceptor

## Design Patterns Used

### 1. **Smart/Dumb Components**
```typescript
// Dumb (presentational) component - receives @Input, emits @Output
@Component({
  selector: 'app-patient-card',
  inputs: ['patient'],
  outputs: ['delete'],
})
export class PatientCardComponent {}

// Smart (container) component - handles logic
@Component({
  selector: 'app-patient-list',
})
export class PatientListComponent {
  patients$ = this.store.select(selectPatients);
}
```

### 2. **OnPush Change Detection**
All components use `ChangeDetectionStrategy.OnPush` for performance.

### 3. **Reactive Forms**
FormBuilder and FormGroup for complex forms with validation.

### 4. **RxJS Operators**
- `takeUntil()` - Automatic unsubscription
- `switchMap()` - Flattening observables
- `map()` - Data transformation
- `shareReplay()` - Result caching

### 5. **Feature Module Pattern**
Each feature is self-contained with routes defined locally:
```typescript
// features/patients/patients.routes.ts
export const patientsRoutes: Routes = [
  { path: '', component: PatientListComponent },
  { path: ':id', component: PatientDetailComponent },
];
```

## Security Best Practices

1. **Authentication** - JWT with refresh token rotation
2. **Authorization** - Role-based access control (RBAC)
3. **Interceptors** - Auto token injection and error handling
4. **Guards** - Route protection with role checks
5. **Sanitization** - Angular built-in XSS protection
6. **HIPAA Compliance** - Audit logging ready
7. **Data Masking** - Sensitive data handling

## Performance Optimizations

1. **Lazy Loading** - Features loaded on-demand
2. **OnPush Detection** - Reduced change detection cycles
3. **Preloading Strategy** - Smart bundle preloading
4. **Tree Shaking** - Dead code elimination
5. **Code Splitting** - Optimal bundle sizes
6. **Virtual Scrolling** - Large list optimization
7. **Image Optimization** - Responsive images

## i18n & Localization

- **English** (en) - Default language
- **Arabic** (ar) - RTL language support
- Translation files in `src/assets/i18n/`
- Built-in Angular i18n integration

## Accessibility (WCAG AA)

- Semantic HTML
- ARIA labels
- Keyboard navigation
- Focus management
- Color contrast compliance
- Screen reader support

## Development Workflow

```bash
# Install dependencies
npm install

# Start dev server
npm start

# Run tests
npm test

# Build production
npm run build:prod

# Analyze bundle
npm run analyze
```

## Next Steps

1. Implement shared UI components
2. Build Patient feature completely
3. Create store setup for state management
4. Implement real API integration
5. Add comprehensive testing
6. Set up CI/CD pipeline
7. Performance optimization
8. Security audit

---

**Project**: Modern EHR Platform  
**Version**: 0.0.1  
**Framework**: Angular 18+  
**Last Updated**: July 2024
