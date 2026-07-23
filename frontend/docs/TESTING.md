# Testing Guide

Comprehensive testing documentation for Modern EHR Platform frontend.

---

## 🧪 Testing Overview

### Test Types

| Type | Tool | Scope | Speed | Cost |
|------|------|-------|-------|------|
| **Unit** | Jasmine/Karma | Single function/component | Fast | Low |
| **Integration** | Jasmine/Karma | Multiple components | Medium | Medium |
| **E2E** | Cypress | Entire workflows | Slow | High |

### Coverage Targets

- **Overall**: > 80%
- **Components**: > 85%
- **Services**: > 90%
- **Pipes/Directives**: > 85%

---

## 🧬 Unit Testing

### Setup

```bash
# Run tests in watch mode
npm run test:watch

# Run tests once (CI mode)
npm test

# Generate coverage report
npm run test:coverage
```

### Basic Test Structure

```typescript
describe('PatientSearchComponent', () => {
  let component: PatientSearchComponent;
  let fixture: ComponentFixture<PatientSearchComponent>;
  let patientService: jasmine.SpyObj<PatientService>;
  
  beforeEach(async () => {
    // 1. Create spy object for service
    const patientServiceSpy = jasmine.createSpyObj('PatientService', [
      'searchPatients',
      'getPatientById'
    ]);
    
    // 2. Configure TestBed
    await TestBed.configureTestingModule({
      imports: [PatientSearchComponent],
      providers: [
        { provide: PatientService, useValue: patientServiceSpy }
      ]
    }).compileComponents();
    
    // 3. Create component and get service
    fixture = TestBed.createComponent(PatientSearchComponent);
    component = fixture.componentInstance;
    patientService = TestBed.inject(PatientService) as jasmine.SpyObj<PatientService>;
  });
  
  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
```

### Testing Components

#### Input Bindings

```typescript
it('should display patient name from input', () => {
  // Arrange
  component.patientName = 'John Doe';
  fixture.detectChanges();
  
  // Act
  const name = fixture.nativeElement.querySelector('h2').textContent;
  
  // Assert
  expect(name).toContain('John Doe');
});
```

#### Output Events

```typescript
it('should emit patientSelected when patient clicked', () => {
  // Arrange
  spyOn(component.patientSelected, 'emit');
  
  // Act
  component.selectPatient(mockPatient);
  
  // Assert
  expect(component.patientSelected.emit).toHaveBeenCalledWith(mockPatient);
});
```

#### User Interactions

```typescript
it('should search patients when search button clicked', () => {
  // Arrange
  component.searchQuery = 'John';
  patientService.searchPatients.and.returnValue(of([mockPatient]));
  
  // Act
  const button = fixture.nativeElement.querySelector('button');
  button.click();
  fixture.detectChanges();
  
  // Assert
  expect(patientService.searchPatients).toHaveBeenCalledWith('John');
  expect(component.patients.length).toBe(1);
});
```

#### Form Validation

```typescript
it('should disable submit button when form invalid', () => {
  // Arrange
  const form = component.patientForm;
  form.get('email')?.setValue('invalid-email');
  
  // Act
  fixture.detectChanges();
  
  // Assert
  expect(form.invalid).toBe(true);
  expect(component.isSubmitDisabled).toBe(true);
});
```

### Testing Services

#### HTTP Requests

```typescript
describe('PatientService', () => {
  let service: PatientService;
  let httpMock: HttpTestingController;
  
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PatientService]
    });
    
    service = TestBed.inject(PatientService);
    httpMock = TestBed.inject(HttpTestingController);
  });
  
  afterEach(() => {
    httpMock.verify();  // Ensure no outstanding requests
  });
  
  it('should fetch patients from API', () => {
    // Arrange
    const mockPatients = [
      { id: '1', name: 'John', status: 'active' }
    ];
    
    // Act
    service.getPatients().subscribe(patients => {
      // Assert
      expect(patients).toEqual(mockPatients);
    });
    
    // Assert HTTP request
    const req = httpMock.expectOne('/api/v1/patients');
    expect(req.request.method).toBe('GET');
    req.flush(mockPatients);
  });
  
  it('should handle HTTP errors', () => {
    // Act
    service.getPatients().subscribe({
      next: () => fail('Should have errored'),
      error: (error) => {
        expect(error.status).toBe(500);
      }
    });
    
    // Assert
    const req = httpMock.expectOne('/api/v1/patients');
    req.flush('Server error', { status: 500, statusText: 'Server Error' });
  });
});
```

#### Observable Testing

```typescript
it('should debounce search input', (done) => {
  // Arrange
  const searchTerms = ['J', 'Jo', 'Joh', 'John'];
  
  // Act
  component.searchTerms$.next('J');
  component.searchTerms$.next('Jo');
  component.searchTerms$.next('Joh');
  component.searchTerms$.next('John');
  
  // Wait for debounce to complete
  setTimeout(() => {
    // Assert
    expect(patientService.searchPatients).toHaveBeenCalledTimes(1);
    expect(patientService.searchPatients).toHaveBeenCalledWith('John');
    done();
  }, 500);
});
```

### Testing Pipes

```typescript
describe('DateFormatPipe', () => {
  let pipe: DateFormatPipe;
  
  beforeEach(() => {
    pipe = new DateFormatPipe();
  });
  
  it('should format date correctly', () => {
    // Arrange
    const date = new Date('2024-07-20');
    
    // Act
    const result = pipe.transform(date, 'short');
    
    // Assert
    expect(result).toContain('07/20/2024');
  });
  
  it('should handle null input', () => {
    // Act
    const result = pipe.transform(null);
    
    // Assert
    expect(result).toBe('');
  });
});
```

### Testing Directives

```typescript
describe('HasPermissionDirective', () => {
  let component: TestComponent;
  let fixture: ComponentFixture<TestComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  
  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['hasPermission']);
    
    await TestBed.configureTestingModule({
      imports: [HasPermissionDirective, TestComponent],
      providers: [{ provide: AuthService, useValue: authServiceSpy }]
    }).compileComponents();
    
    fixture = TestBed.createComponent(TestComponent);
    component = fixture.componentInstance;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
  });
  
  it('should show element when user has permission', () => {
    // Arrange
    authService.hasPermission.and.returnValue(true);
    component.permission = 'patients:read';
    
    // Act
    fixture.detectChanges();
    
    // Assert
    const element = fixture.nativeElement.querySelector('[appHasPermission]');
    expect(element).toBeTruthy();
  });
  
  it('should hide element when user lacks permission', () => {
    // Arrange
    authService.hasPermission.and.returnValue(false);
    component.permission = 'admin:delete';
    
    // Act
    fixture.detectChanges();
    
    // Assert
    const element = fixture.nativeElement.querySelector('[appHasPermission]');
    expect(element).toBeFalsy();
  });
});
```

---

## 🔗 Integration Testing

Integration tests verify that multiple components/services work together.

```typescript
describe('Patient Management Integration', () => {
  let patientService: PatientService;
  let authService: AuthService;
  let httpMock: HttpTestingController;
  
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PatientService, AuthService]
    });
    
    patientService = TestBed.inject(PatientService);
    authService = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });
  
  it('should fetch patients after authentication', () => {
    // Arrange
    spyOn(authService, 'login').and.returnValue(of({ token: 'jwt123' }));
    
    // Act
    authService.login('user@test.com', 'password').subscribe(() => {
      patientService.getPatients().subscribe(patients => {
        // Assert
        expect(patients.length).toBeGreaterThan(0);
      });
    });
    
    // Handle auth request
    const authReq = httpMock.expectOne('/api/v1/auth/login');
    authReq.flush({ token: 'jwt123' });
    
    // Handle patients request
    const patientsReq = httpMock.expectOne('/api/v1/patients');
    expect(patientsReq.request.headers.get('Authorization')).toBe('Bearer jwt123');
    patientsReq.flush([{ id: '1', name: 'John' }]);
  });
});
```

---

## 🎯 E2E Testing with Cypress

### Setup

```bash
# Install Cypress
npm install --save-dev cypress

# Open Cypress Test Runner
npm run e2e:open

# Run headless
npm run e2e:headless
```

### Test Structure

```typescript
describe('Patient Management E2E', () => {
  beforeEach(() => {
    // Visit login page
    cy.visit('http://localhost:4200');
  });
  
  it('should login and view patient list', () => {
    // Login
    cy.get('input[type="email"]').type('doctor@hospital.com');
    cy.get('input[type="password"]').type('Password123!');
    cy.get('button:contains("Login")').click();
    
    // Verify redirected to dashboard
    cy.url().should('include', '/dashboard');
    cy.get('h1').should('contain', 'Dashboard');
    
    // Navigate to patients
    cy.get('a:contains("Patients")').click();
    cy.url().should('include', '/patients');
    
    // Verify patient list displayed
    cy.get('app-table').should('be.visible');
    cy.get('tr').should('have.length.greaterThan', 1);
  });
});
```

### Common Cypress Commands

```typescript
// Navigation
cy.visit('/patients');
cy.go('back');
cy.url().should('include', '/patients');

// Element interaction
cy.get('button').click();
cy.get('input').type('text');
cy.get('select').select('option');
cy.get('a').contains('Link text').click();

// Assertions
cy.get('h1').should('contain', 'Patients');
cy.get('button').should('be.disabled');
cy.get('element').should('not.exist');
cy.get('element').should('be.visible');

// Waiting
cy.get('button').click();
cy.get('.loading-spinner').should('not.exist');  // Wait for load
cy.wait('@apiRequest');  // Wait for API call

// Debugging
cy.pause();  // Pause execution
cy.debug();  // Log element info
cy.log('Message');  // Log to console
```

### Testing Login Flow

```typescript
describe('Authentication E2E', () => {
  it('should login successfully with valid credentials', () => {
    cy.visit('/login');
    cy.get('input[name="email"]').type('doctor@hospital.com');
    cy.get('input[name="password"]').type('Password123!');
    cy.get('button[type="submit"]').click();
    
    cy.url().should('include', '/dashboard');
    cy.get('[data-testid="user-menu"]').should('contain', 'John Doe');
  });
  
  it('should show error with invalid credentials', () => {
    cy.visit('/login');
    cy.get('input[name="email"]').type('doctor@hospital.com');
    cy.get('input[name="password"]').type('WrongPassword');
    cy.get('button[type="submit"]').click();
    
    cy.get('[role="alert"]').should('contain', 'Invalid credentials');
    cy.url().should('include', '/login');
  });
});
```

### Testing Patient Search

```typescript
describe('Patient Search E2E', () => {
  beforeEach(() => {
    cy.login('doctor@hospital.com', 'Password123!');
    cy.visit('/patients');
  });
  
  it('should search patients by name', () => {
    // Intercept API call
    cy.intercept('POST', '/api/v1/patients/search').as('searchRequest');
    
    // Search
    cy.get('input[placeholder="Search patients"]').type('John');
    cy.get('button:contains("Search")').click();
    
    // Wait for API and verify results
    cy.wait('@searchRequest');
    cy.get('table tbody tr').should('have.length', 1);
    cy.get('table').should('contain', 'John Smith');
  });
});
```

### Custom Cypress Commands

Create `cypress/support/commands.ts`:

```typescript
// Login command
Cypress.Commands.add('login', (email: string, password: string) => {
  cy.visit('/login');
  cy.get('input[name="email"]').type(email);
  cy.get('input[name="password"]').type(password);
  cy.get('button[type="submit"]').click();
  cy.url().should('include', '/dashboard');
});

// Create patient command
Cypress.Commands.add('createPatient', (patientData: any) => {
  cy.visit('/patients/new');
  cy.get('input[name="firstName"]').type(patientData.firstName);
  cy.get('input[name="lastName"]').type(patientData.lastName);
  cy.get('input[name="dob"]').type(patientData.dob);
  cy.get('button:contains("Create")').click();
  cy.url().should('include', '/patients/');
});
```

Use in tests:

```typescript
it('should create new patient', () => {
  cy.login('doctor@hospital.com', 'Password123!');
  cy.createPatient({
    firstName: 'Jane',
    lastName: 'Smith',
    dob: '01/15/1990'
  });
  cy.get('h1').should('contain', 'Jane Smith');
});
```

---

## 📊 Coverage Reports

### Generate Coverage

```bash
npm run test:coverage
```

Coverage files generated in `coverage/` directory:
- `index.html` - Visual report
- `lcov.info` - Machine-readable format

### View Report

```bash
# Open in browser
open coverage/index.html

# Or use VS Code extension
# Install: Coverage Gutters
# Shows coverage inline in editor
```

### Coverage Thresholds

```json
// karma.conf.js
coverageReporter: {
  dir: require('path').join(__dirname, './coverage'),
  subdir: '.',
  reporters: [
    { type: 'html' },
    { type: 'text-summary' },
    { type: 'lcovonly' }
  ],
  check: {
    global: {
      statements: 80,
      branches: 75,
      functions: 80,
      lines: 80
    }
  }
}
```

---

## ✅ Testing Checklist

Before committing code:

- [ ] All unit tests pass
- [ ] Coverage > 80%
- [ ] No console errors in tests
- [ ] Integration tests pass
- [ ] E2E tests pass for new features
- [ ] No skipped tests (`xit`, `xdescribe`)
- [ ] No `fit` or `fdescribe` left in code
- [ ] Error scenarios tested
- [ ] Edge cases covered
- [ ] Performance acceptable

---

## 🐛 Common Testing Issues

### Issue: Test Timeout

```typescript
// Solution: Increase timeout for async operations
it('should fetch data', (done) => {
  service.fetchData().subscribe(() => {
    expect(true).toBe(true);
    done();
  });
}, 5000);  // 5 second timeout
```

### Issue: Change Detection Not Working

```typescript
// Solution: Call detectChanges()
fixture.componentInstance.property = newValue;
fixture.detectChanges();  // Trigger change detection
expect(fixture.nativeElement.textContent).toContain('newValue');
```

### Issue: Async Operations

```typescript
// Solution: Use fakeAsync and tick
it('should debounce input', fakeAsync(() => {
  component.searchQuery = 'test';
  tick(300);  // Simulate passage of time
  expect(component.results).toBeDefined();
}));
```

---

## 📚 Resources

- [Jasmine Docs](https://jasmine.github.io)
- [Karma Docs](https://karma-runner.github.io)
- [Cypress Docs](https://docs.cypress.io)
- [Angular Testing Guide](https://angular.io/guide/testing)

---

**Version**: 1.0.0 | Last Updated: July 2026
