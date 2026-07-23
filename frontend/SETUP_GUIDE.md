# Modern EHR Platform - Setup & Development Guide

## Prerequisites

- **Node.js**: 18.x or 20.x LTS
- **npm**: 9.x or higher
- **Git**: Latest version
- **Angular CLI**: 18.x (installed globally)

## Installation

### 1. Install Dependencies

```bash
cd frontend
npm install
```

This installs:
- Angular 18+ framework
- Tailwind CSS for styling
- NgRx for state management
- PrimeNG for advanced UI components
- TypeScript 5.3+
- And other necessary packages

### 2. Verify Installation

```bash
ng version
```

Should output Angular 18+.

## Development Setup

### 1. Start Development Server

```bash
npm start
```

Or using Angular CLI directly:

```bash
ng serve
```

The application will be available at: **http://localhost:4200**

### 2. Development Features

- **Hot Reload**: Changes automatically reload in browser
- **Source Maps**: Debug TypeScript directly in browser
- **Console**: Angular logs and errors visible

### 3. Configure Backend API

Edit `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://YOUR_API_URL:3000/api',
  wsUrl: 'ws://YOUR_API_URL:3000',
};
```

## Project Structure Overview

```
frontend/
├── src/app/
│   ├── core/              # Singleton services, guards, interceptors
│   ├── shared/            # Reusable components, pipes, directives
│   ├── features/          # Business domains (lazy-loaded)
│   │   ├── auth/          # Authentication
│   │   ├── patients/      # Patient management
│   │   ├── dashboard/     # Dashboard
│   │   └── ...            # Other features
│   ├── layouts/           # Main, Auth, Print layouts
│   ├── routes/            # Routing configuration
│   └── store/             # NgRx state management
├── src/assets/            # Static files, i18n, styles
├── src/environments/      # Environment configurations
└── angular.json           # Angular CLI configuration
```

## Code Organization

### File Naming Conventions

- **Components**: `*.component.ts` (standalone = true)
- **Services**: `*.service.ts` (providedIn: 'root')
- **Models**: `*.model.ts` (interfaces and types)
- **Store**: `*.actions.ts`, `*.reducer.ts`, `*.effects.ts`, `*.selectors.ts`
- **Pipes**: `*.pipe.ts`
- **Directives**: `*.directive.ts`

### Example Structure for a Feature

```
features/patients/
├── pages/
│   ├── patient-list-page/
│   ├── patient-detail-page/
│   └── patient-timeline-page/
├── components/
│   ├── patient-form/
│   └── patient-card/
├── services/
│   └── patient.service.ts
├── store/
│   ├── patient.actions.ts
│   ├── patient.reducer.ts
│   ├── patient.effects.ts
│   └── patient.selectors.ts
├── models/
│   └── patient.model.ts
└── patients.routes.ts
```

## Component Creation

### Create a New Component

```bash
ng generate component features/patients/components/patient-form --skip-tests
```

Or manually create:

```typescript
import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-patient-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `<form></form>`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PatientFormComponent {
  @Input() patient: any;
  @Output() submitted = new EventEmitter<any>();
}
```

### Key Patterns

1. **Always use `standalone: true`**
2. **Always use `ChangeDetectionStrategy.OnPush`**
3. **Use Reactive Forms** for complex forms
4. **Import dependencies** explicitly in `imports`

## Services & Dependency Injection

### Create a Service

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',  // Auto-provided in root injector
})
export class PatientService {
  constructor(private http: HttpClient) {}

  getPatients(): Observable<any[]> {
    return this.http.get<any[]>('/api/patients');
  }
}
```

### Use in Component

```typescript
@Component({...})
export class PatientListComponent {
  patients$ = this.patientService.getPatients();

  constructor(private patientService: PatientService) {}
}
```

## State Management with NgRx

### Using NgRx Signals (Recommended for simple state)

```typescript
import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';

export const PatientStore = signalStore(
  { providedIn: 'root' },
  withState<PatientState>({ patients: [], loading: false }),
  withMethods((store) => ({
    setPatients: (patients: Patient[]) =>
      patchState(store, { patients }),
    setLoading: (loading: boolean) =>
      patchState(store, { loading }),
  }))
);
```

### Using NgRx Store (For complex state)

1. **Define Actions** (`patient.actions.ts`):
```typescript
export const loadPatients = createAction(
  '[Patient] Load Patients'
);
export const loadPatientsSuccess = createAction(
  '[Patient] Load Patients Success',
  props<{ patients: Patient[] }>()
);
```

2. **Create Reducer** (`patient.reducer.ts`):
```typescript
const initialState: PatientState = { patients: [], loading: false };

export const patientReducer = createReducer(
  initialState,
  on(loadPatients, (state) => ({ ...state, loading: true })),
  on(loadPatientsSuccess, (state, { patients }) => ({
    ...state,
    patients,
    loading: false,
  }))
);
```

3. **Create Effects** (`patient.effects.ts`):
```typescript
loadPatients$ = createEffect(() =>
  this.actions$.pipe(
    ofType(loadPatients),
    switchMap(() => this.patientService.getPatients()),
    map((patients) => loadPatientsSuccess({ patients }))
  )
);
```

4. **Create Selectors** (`patient.selectors.ts`):
```typescript
export const selectPatients = createSelector(
  selectPatientState,
  (state) => state.patients
);
```

## Routing

### Define Routes

```typescript
// features/patients/patients.routes.ts
export const patientsRoutes: Routes = [
  { path: '', component: PatientListComponent },
  { path: ':id', component: PatientDetailComponent },
];
```

### Use in Main Routes

```typescript
// app.routes.ts
{
  path: 'patients',
  canActivate: [authGuard],
  children: loadChildren(() => 
    import('./features/patients/patients.routes').then(m => m.patientsRoutes)
  )
}
```

## Testing

### Run Tests

```bash
npm test              # Run tests once
npm run test:watch   # Run tests in watch mode
npm run test:coverage # Generate coverage report
```

### Write Unit Tests

```typescript
import { TestBed } from '@angular/core/testing';
import { PatientService } from './patient.service';

describe('PatientService', () => {
  let service: PatientService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PatientService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
```

## Building

### Development Build

```bash
npm run build
```

Output: `dist/ehr-platform/`

### Production Build

```bash
npm run build:prod
```

Optimizations:
- Minification
- Tree-shaking
- Code splitting
- AOT compilation
- CSS/JS optimization

### Analyze Bundle

```bash
npm run analyze
```

View bundle size visualization.

## Linting & Code Quality

### Run Linter

```bash
npm run lint        # Check for issues
npm run lint:fix    # Auto-fix issues
```

### Prettier Configuration

Create `.prettierrc`:

```json
{
  "printWidth": 100,
  "tabWidth": 2,
  "useTabs": false,
  "semi": true,
  "singleQuote": true,
  "trailingComma": "es5",
  "arrowParens": "avoid"
}
```

## Environment Configuration

### Development (`.env.local`)

```env
API_URL=http://localhost:3000/api
WS_URL=ws://localhost:3000
DEBUG=true
```

### Production

```env
API_URL=https://api.ehr-platform.com/api
WS_URL=wss://api.ehr-platform.com
DEBUG=false
```

Load in `app.config.ts`:

```typescript
import { environment } from '@env/environment';
```

## Debugging

### Chrome DevTools

1. Open Chrome DevTools (F12)
2. Go to **Sources** tab
3. Source maps enabled for TypeScript debugging
4. Set breakpoints in `.ts` files

### Angular DevTools

Install [Angular DevTools Chrome Extension](https://chrome.google.com/webstore)

Features:
- Component tree inspection
- Property binding inspection
- Change detection timing
- NgRx store inspection

### Logging

```typescript
import { environment } from '@env/environment';

if (!environment.production) {
  console.log('Development mode enabled');
}
```

## Performance Tips

1. **Use OnPush Change Detection**
   ```typescript
   changeDetection: ChangeDetectionStrategy.OnPush
   ```

2. **Unsubscribe from Observables**
   ```typescript
   private destroy$ = new Subject<void>();
   
   ngOnDestroy() {
     this.destroy$.next();
     this.destroy$.complete();
   }
   
   // In subscriptions:
   .pipe(takeUntil(this.destroy$))
   .subscribe(...)
   ```

3. **Use trackBy in *ngFor**
   ```typescript
   trackByPatientId(index: number, patient: Patient): string {
     return patient.id;
   }
   
   // In template:
   <div *ngFor="let patient of patients; trackBy: trackByPatientId">
   ```

4. **Lazy Load Modules**
   - All features are already lazy-loaded

5. **Defer Loading with @defer**
   ```typescript
   @defer (when isVisible) {
     <app-heavy-component />
   } @placeholder {
     <p>Loading...</p>
   }
   ```

## Accessibility Checklist

- ✅ Semantic HTML
- ✅ ARIA labels on interactive elements
- ✅ Keyboard navigation (Tab, Enter, Escape)
- ✅ Focus indicators
- ✅ Color contrast ratio (WCAG AA)
- ✅ Form labels and validation messages
- ✅ Image alt text
- ✅ Screen reader testing

## Internationalization (i18n)

### Translation Files

```json
// src/assets/i18n/en.json
{
  "patients": {
    "list": "Patients",
    "add": "Add Patient"
  }
}

// src/assets/i18n/ar.json
{
  "patients": {
    "list": "المرضى",
    "add": "إضافة مريض"
  }
}
```

### Use Translations

```typescript
constructor(private translateService: TranslateService) {}

title$ = this.translateService.get('patients.list');
```

## Deployment

### Deploy to Production

1. **Build**
   ```bash
   npm run build:prod
   ```

2. **Configure Server**
   - Serve from `dist/ehr-platform/`
   - Route all requests to `index.html` (SPA)

3. **Environment Configuration**
   Update `environment.prod.ts` with production API URL

4. **SSL/HTTPS**
   Always use HTTPS in production

5. **CORS Configuration**
   Configure backend to accept requests from your domain

## Troubleshooting

### Port Already in Use

```bash
ng serve --port 4201
```

### Clear Cache

```bash
rm -rf node_modules package-lock.json
npm install
```

### TypeScript Errors

```bash
ng build --aot --strict
```

### Module Not Found

Check import paths match `tsconfig.json` path aliases:

```json
{
  "paths": {
    "@core/*": ["src/app/core/*"],
    "@shared/*": ["src/app/shared/*"],
    "@features/*": ["src/app/features/*"]
  }
}
```

## Resources

- [Angular Documentation](https://angular.io)
- [NgRx Documentation](https://ngrx.io)
- [Tailwind CSS](https://tailwindcss.com)
- [RxJS Operators](https://rxjs.dev/api)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)

## Support

For questions and issues:
1. Check the documentation
2. Review existing code patterns
3. Consult team members
4. Create GitHub issue

---

**Happy Coding! 🚀**
