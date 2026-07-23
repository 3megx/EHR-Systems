# Contributing to Modern EHR Platform

Thank you for wanting to contribute! This document provides guidelines for contributing to the project.

---

## 🤝 Code of Conduct

We are committed to providing a welcoming and inclusive environment. Be respectful, professional, and considerate.

---

## 🏃 Quick Start for Contributors

### 1. Fork & Clone

```bash
# Fork on GitHub, then clone your fork
git clone https://github.com/YOUR_USERNAME/modern-ehr-platform.git
cd modern-ehr-platform
```

### 2. Create Feature Branch

```bash
# Use semantic branch naming
git checkout -b feature/patient-search-enhancement
# or
git checkout -b bugfix/appointment-validation
# or
git checkout -b docs/update-api-guide
```

### 3. Set Up Development Environment

```bash
# Install dependencies
npm install
cd frontend && npm install && cd ..
cd backend && dotnet restore && cd ..

# Start local environment
docker-compose up -d

# Create .env file with local settings
cp .env.example .env
```

### 4. Make Your Changes

- Write clean, well-documented code
- Follow project coding standards (see below)
- Add tests for new features
- Update documentation

### 5. Commit with Semantic Messages

```bash
# Good commit messages
git commit -m "feat(patients): add advanced search filters"
git commit -m "fix(appointments): resolve timezone handling in booking"
git commit -m "docs(api): update endpoint examples"
git commit -m "test(prescriptions): add interaction checking tests"

# Bad commit messages
git commit -m "fixes"
git commit -m "updates"
git commit -m "WIP"
```

### 6. Push & Create Pull Request

```bash
git push origin feature/patient-search-enhancement
```

Then create a PR on GitHub with:
- Clear title summarizing the change
- Description of what changed and why
- Link to related issue (if any)
- Screenshots (for UI changes)

---

## 📋 Coding Standards

### Frontend (Angular + TypeScript)

**File Structure**:
```
feature/
├── components/
│   ├── feature.component.ts
│   ├── feature.component.html
│   ├── feature.component.scss
│   └── feature.component.spec.ts
├── models/
│   └── feature.model.ts
├── services/
│   ├── feature.service.ts
│   └── feature.service.spec.ts
├── pages/
│   ├── feature-page.component.ts
│   ├── feature-page.component.html
│   └── feature-page.component.spec.ts
└── feature.routes.ts
```

**TypeScript Style**:
```typescript
// ✅ GOOD
export interface PatientSearchFilter {
  firstName?: string;
  lastName?: string;
  mrn?: string;
  dateOfBirth?: Date;
  status?: PatientStatus;
}

export class PatientService {
  private readonly logger = inject(LoggerService);
  private readonly http = inject(HttpClient);
  
  searchPatients(filter: PatientSearchFilter): Observable<Patient[]> {
    if (!this.isValidFilter(filter)) {
      throw new Error('Invalid search filter');
    }
    
    return this.http.post<Patient[]>('/api/v1/patients/search', filter);
  }
  
  private isValidFilter(filter: PatientSearchFilter): boolean {
    // Validation logic
  }
}

// ❌ BAD
class patient_service {
  searchPatients(f) {
    return http.post('/patients/search', f);
  }
}
```

**Angular Best Practices**:
- Standalone components everywhere
- OnPush change detection
- Lazy loading for features
- Dependency injection via inject()
- Reactive forms (FormBuilder)
- Type safety (strict mode)
- Documentation for public APIs

### Backend (C# / ASP.NET Core)

**File Structure**:
```
Services/
├── PatientService/
│   ├── Controllers/
│   │   └── PatientController.cs
│   ├── Models/
│   │   ├── PatientDto.cs
│   │   └── PatientCreateRequest.cs
│   ├── Services/
│   │   ├── IPatientService.cs
│   │   └── PatientService.cs
│   ├── Repositories/
│   │   ├── IPatientRepository.cs
│   │   └── PatientRepository.cs
│   └── Tests/
│       ├── PatientServiceTests.cs
│       └── PatientControllerTests.cs
```

**C# Style**:
```csharp
// ✅ GOOD
namespace EHRPlatform.Services.PatientService.Services;

public interface IPatientService
{
    Task<PatientDto> GetPatientByIdAsync(Guid id);
    Task<(List<PatientDto>, int totalCount)> SearchPatientsAsync(PatientSearchQuery query);
    Task<PatientDto> CreatePatientAsync(PatientCreateRequest request);
}

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;
    private readonly ILogger<PatientService> _logger;
    
    public PatientService(IPatientRepository repository, ILogger<PatientService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<PatientDto> GetPatientByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Invalid patient ID");
        
        var patient = await _repository.GetByIdAsync(id);
        return patient?.ToDto() ?? throw new NotFoundException("Patient not found");
    }
}

// ❌ BAD
public class PatientService
{
    public PatientDto GetPatient(string id)
    {
        var p = _repo.Get(id);
        return p.ToDto();
    }
}
```

**C# Best Practices**:
- Dependency injection via constructor
- Async/await for I/O operations
- Proper exception handling
- Input validation
- Logging at appropriate levels
- DTOs for API contracts
- Repository pattern for data access
- Unit of work for transactions

---

## 🧪 Testing Requirements

### Frontend Tests

**Minimum Coverage**: 80%

```bash
# Run tests
npm run test:ci --prefix frontend

# Check coverage
npm run coverage --prefix frontend

# Add test for new component
# feature.component.spec.ts should include:
# - Constructor injection
# - Input/Output bindings
# - User interactions
# - Service calls
# - Error scenarios
```

**Test Template**:
```typescript
describe('PatientSearchComponent', () => {
  let component: PatientSearchComponent;
  let fixture: ComponentFixture<PatientSearchComponent>;
  let patientService: jasmine.SpyObj<PatientService>;
  
  beforeEach(async () => {
    const spy = jasmine.createSpyObj('PatientService', ['searchPatients']);
    
    await TestBed.configureTestingModule({
      imports: [PatientSearchComponent],
      providers: [{ provide: PatientService, useValue: spy }]
    }).compileComponents();
    
    patientService = TestBed.inject(PatientService) as jasmine.SpyObj<PatientService>;
    fixture = TestBed.createComponent(PatientSearchComponent);
    component = fixture.componentInstance;
  });
  
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  
  it('should call patientService.searchPatients when search button clicked', () => {
    // Arrange
    const query = 'John';
    patientService.searchPatients.and.returnValue(of([mockPatient]));
    
    // Act
    component.searchQuery = query;
    component.onSearch();
    
    // Assert
    expect(patientService.searchPatients).toHaveBeenCalledWith(query);
  });
});
```

### Backend Tests

**Minimum Coverage**: 75%

```bash
# Run tests
cd backend && dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

**Test Template**:
```csharp
[TestClass]
public class PatientServiceTests
{
    private Mock<IPatientRepository> _mockRepository;
    private Mock<ILogger<PatientService>> _mockLogger;
    private PatientService _service;
    
    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IPatientRepository>();
        _mockLogger = new Mock<ILogger<PatientService>>();
        _service = new PatientService(_mockRepository.Object, _mockLogger.Object);
    }
    
    [TestMethod]
    public async Task GetPatientByIdAsync_WithValidId_ReturnsPatient()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var mockPatient = new Patient { Id = patientId, FirstName = "John" };
        _mockRepository.Setup(r => r.GetByIdAsync(patientId))
            .ReturnsAsync(mockPatient);
        
        // Act
        var result = await _service.GetPatientByIdAsync(patientId);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("John", result.FirstName);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task GetPatientByIdAsync_WithEmptyId_ThrowsException()
    {
        // Act
        await _service.GetPatientByIdAsync(Guid.Empty);
    }
}
```

---

## 📝 Documentation

### Code Comments

```typescript
// ✅ GOOD: Explain WHY, not WHAT
// We retry 3 times because the API occasionally returns 503
// when under load. After 3 retries, we assume permanent failure.
private retryPolicy = retry({ count: 3, delay: 1000 });

// ❌ BAD: Obvious from code
// Retry 3 times
private retryPolicy = retry({ count: 3 });
```

### Commit Messages

Format: `<type>(<scope>): <subject>`

Types:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `style`: Code style changes (formatting)
- `refactor`: Code refactoring
- `perf`: Performance improvement
- `test`: Adding/updating tests
- `chore`: Maintenance tasks

Examples:
```
feat(patients): add bulk import from CSV
fix(appointments): resolve double-booking issue
docs(setup): clarify Docker setup steps
test(prescriptions): add interaction checker tests
```

### Pull Request Description

```markdown
## Description
Brief explanation of what this PR does.

## Related Issue
Closes #123

## Changes Made
- Change 1
- Change 2
- Change 3

## Screenshots (if UI changes)
[Include screenshots]

## Testing
- [ ] Unit tests added
- [ ] E2E tests added
- [ ] Manual testing completed

## Checklist
- [ ] Code follows project style
- [ ] Documentation updated
- [ ] No breaking changes
- [ ] Database migrations (if needed)
```

---

## 🔍 Code Review Process

### For Contributors

1. **Before submitting PR**:
   - Run `npm run lint` (frontend)
   - Run `npm test` (frontend)
   - Run `dotnet test` (backend)
   - Update documentation

2. **After submitting**:
   - Wait for CI/CD checks to pass
   - Wait for code review (2+ reviewers for main)
   - Respond to review comments promptly
   - Don't force-push (preserve conversation history)

### For Reviewers

Check for:
- ✅ Code follows standards & style guide
- ✅ Tests added/updated
- ✅ Documentation updated
- ✅ No breaking changes
- ✅ Security best practices followed
- ✅ Performance implications considered
- ✅ Error handling adequate
- ✅ HIPAA compliance maintained

---

## 🐛 Bug Reports

Use GitHub Issues with this template:

```markdown
## Describe the Bug
Clear description of what happened.

## Steps to Reproduce
1. Step 1
2. Step 2
3. Step 3

## Expected Behavior
What should happen.

## Actual Behavior
What actually happened.

## Environment
- OS: Windows / Mac / Linux
- Browser: Chrome / Firefox / Safari
- Version: 1.0.0

## Screenshots
[If applicable]

## Additional Context
[Any other relevant info]
```

---

## ✨ Feature Requests

Use GitHub Issues with this template:

```markdown
## Description
What feature should be added and why?

## Use Case
Who would benefit and how?

## Proposed Solution
How should it work?

## Alternatives Considered
Any other approaches?
```

---

## 📚 Style Guides

### Git Workflow

```
main (production)
  └─ staging (pre-production)
      ├─ feature branches (develop here)
      └─ bugfix branches
```

**Process**:
1. Create feature branch from `staging`
2. Create PR to `staging`
3. After approval & testing: merge to `main`

### Naming Conventions

**Branches**:
```
feature/short-description
bugfix/bug-name
docs/doc-name
refactor/refactor-description
```

**Files**:
```
TypeScript: feature.component.ts (camelCase)
C#: PatientService.cs (PascalCase)
SQL: 001_initial_schema.sql (snake_case)
```

### Environment Variables

```bash
# .env (local development)
DATABASE_URL=Server=localhost;Database=ehr_dev;...
API_KEY=dev_key_xyz
LOG_LEVEL=debug

# Never commit .env to Git
# Use .env.example as template
```

---

## 🚀 Deployment Checklist

Before merging to main:

- [ ] All tests pass (unit + E2E)
- [ ] Code coverage >= threshold
- [ ] Security scan passed
- [ ] No breaking changes (or documented)
- [ ] Database migrations tested
- [ ] API docs updated
- [ ] Changelog updated
- [ ] Version bumped (if release)
- [ ] Performance reviewed
- [ ] Security review completed

---

## 📞 Getting Help

- **Questions?** Open a GitHub Discussion
- **Bug?** File an Issue with template
- **Security concern?** Email security@moderneHRplatform.com
- **Chat?** Join our Slack channel (if available)

---

## 📄 License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

**Thank you for contributing! 🎉**

Last Updated: July 2026
