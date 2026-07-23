# EHR Platform - Mapping Architecture Review & Refactoring Guide

**Date**: July 23, 2026  
**Focus**: Single Responsibility Principle, SOLID Architecture, Dedicated Mapping Layers  
**Current Status**: VIOLATIONS FOUND - Multiple inline mappings, duplicate MapToDto methods, mixed concerns

---

## 🚨 Critical Issues Found

### Backend Issues (C# / ASP.NET Core)

#### 1. **Billing Service - Severe SOLID Violations**
**Location**: `backend/src/EHRPlatform.Services.Billing/`

**Problems**:
- ❌ `MapToDto()` method duplicated across 3 handlers (SRP violation)
- ❌ Complex nested transformations inline (LineItemDto, PaymentDto, ClaimDto)
- ❌ Tight coupling between domain and DTO layers
- ❌ No testable mapping logic (mixed with business logic)
- ❌ Difficult to modify mapping rules (scattered across files)

**Current Pattern** (WRONG):
```csharp
// In BillingCommandHandler.cs
private InvoiceResponseDto MapToDto(Invoice invoice)
{
    var dto = invoice.Adapt<InvoiceResponseDto>();
    dto.BalanceDue = invoice.BalanceDue;
    dto.LineItems = invoice.LineItems.Select(l => new LineItemDto {
        Id = l.Id,
        Description = l.Description,
        CPTCode = l.CPTCode,
        // ... more fields
    }).ToList();
    // ... duplicate in GetPatientInvoicesQueryHandler
    // ... duplicate in GetPatientOutstandingBalanceQueryHandler
}
```

#### 2. **Appointment Service - Inconsistent Mapping Strategy**
**Location**: `backend/src/EHRPlatform.Services.Appointment/`

**Problems**:
- ❌ Mix of simple `.Adapt()` calls and complex inline mappings
- ❌ Availability slot mapping logic embedded in query handlers
- ❌ No centralized type conversion strategy

#### 3. **Common Library - Missing Mapster Configuration**
**Location**: `backend/src/EHRPlatform.Common/`

**Problems**:
- ❌ Global Mapster registration but no service-specific mapping profiles
- ❌ No IRegister implementations for type converters
- ❌ Missing mapping configuration per microservice context

### Frontend Issues (Angular / TypeScript)

#### 1. **Patient Service - Inline Data Transformation**
**Location**: `frontend/src/app/features/patients/services/`

**Problems**:
- ❌ `.map()` transformations embedded in service methods
- ❌ CSV generation mixing data transformation with formatting
- ❌ No dedicated adapter/converter layer
- ❌ Data shape conversions hardcoded in service

#### 2. **Frontend Patterns - Missing Adapter Layer**
**Location**: `frontend/src/app/features/*/services/`

**Problems**:
- ❌ No DTO interfaces separate from domain models
- ❌ No dedicated adapter/converter classes
- ❌ RxJS pipe operators handling transformations (scattered logic)
- ❌ Mock data directly transformed without mapping layer

---

## ✅ Recommended Architecture

### Backend: Mapster-Based Mapping Layer

**Folder Structure** (Per Microservice):
```
EHRPlatform.Services.{ServiceName}/
├── Features/
│   └── {Feature}/
│       ├── Commands/
│       ├── Queries/
│       ├── Handlers/
│       └── Mappers/              ← NEW: Dedicated mapping layer
│           ├── {Feature}Mapper.cs
│           └── {Feature}MappingProfile.cs
├── Mappings/                      ← NEW: Service-level mapping config
│   ├── ServiceMappingProfile.cs
│   └── ServiceMapper.cs
└── Program.cs
```

**Implementation Pattern** (CORRECT):

```csharp
// Mappers/InvoiceMappingProfile.cs - Mapster IRegister
public class InvoiceMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Invoice, InvoiceResponseDto>()
            .Map(dest => dest.LineItems, src => src.LineItems)
            .Map(dest => dest.Payments, src => src.Payments)
            .Map(dest => dest.Claims, src => src.InsuranceClaims);

        config.NewConfig<LineItem, LineItemDto>();
        config.NewConfig<Payment, PaymentDto>();
        config.NewConfig<InsuranceClaim, ClaimDto>();
    }
}

// Mappers/InvoiceMapper.cs - Single Responsibility Mapper
public class InvoiceMapper
{
    public InvoiceResponseDto MapToResponseDto(Invoice invoice)
    {
        return invoice.Adapt<InvoiceResponseDto>();
    }

    public InvoiceListDto MapToListDto(List<Invoice> invoices, int total, int pageNumber, int pageSize)
    {
        return new InvoiceListDto
        {
            Items = invoices.Adapt<List<InvoiceResponseDto>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}

// In Handlers - Clean separation
public class GetPatientInvoicesQueryHandler : IQueryHandler<GetPatientInvoicesQuery, InvoiceListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly InvoiceMapper _mapper;  ← Injected mapper

    public GetPatientInvoicesQueryHandler(IUnitOfWork unitOfWork, InvoiceMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<InvoiceListDto> Handle(GetPatientInvoicesQuery request, CancellationToken cancellationToken)
    {
        // Business logic only - NO mapping
        var invoices = await _unitOfWork.Repository<Invoice>()
            .ToListAsync(/* query */);

        // Delegate to mapper
        return _mapper.MapToListDto(invoices, total, request.PageNumber, request.PageSize);
    }
}
```

### Frontend: Dedicated Adapter Layer

**Folder Structure** (Per Feature):
```
frontend/src/app/features/{feature}/
├── pages/
├── components/
├── services/
├── models/
│   ├── domain/               ← NEW: Domain models
│   │   └── patient.model.ts
│   ├── dto/                  ← NEW: API DTOs
│   │   └── patient.dto.ts
│   └── adapters/             ← NEW: Conversion layer
│       └── patient.adapter.ts
├── store/
└── {feature}.routes.ts
```

**Implementation Pattern** (CORRECT):

```typescript
// models/dto/patient.dto.ts
export interface PatientDto {
  id: string;
  firstName: string;
  lastName: string;
  mrn: string;
  dateOfBirth: string;  // ISO string from API
}

// models/domain/patient.model.ts
export interface Patient {
  id: string;
  firstName: string;
  lastName: string;
  mrn: string;
  dateOfBirth: Date;  // Converted to Date
  fullName: string;
  age: number;
}

// models/adapters/patient.adapter.ts - Single Responsibility Adapter
@Injectable({ providedIn: 'root' })
export class PatientAdapter {
  /**
   * Convert API DTO to domain model
   */
  fromDto(dto: PatientDto): Patient {
    const dob = new Date(dto.dateOfBirth);
    return {
      id: dto.id,
      firstName: dto.firstName,
      lastName: dto.lastName,
      mrn: dto.mrn,
      dateOfBirth: dob,
      fullName: `${dto.firstName} ${dto.lastName}`,
      age: this.calculateAge(dob)
    };
  }

  /**
   * Convert domain model to API DTO
   */
  toDto(model: Patient): PatientDto {
    return {
      id: model.id,
      firstName: model.firstName,
      lastName: model.lastName,
      mrn: model.mrn,
      dateOfBirth: model.dateOfBirth.toISOString()
    };
  }

  /**
   * Convert multiple DTOs
   */
  fromDtoList(dtos: PatientDto[]): Patient[] {
    return dtos.map(dto => this.fromDto(dto));
  }

  private calculateAge(dob: Date): number {
    const today = new Date();
    let age = today.getFullYear() - dob.getFullYear();
    const monthDiff = today.getMonth() - dob.getMonth();
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < dob.getDate())) {
      age--;
    }
    return age;
  }
}

// services/patient.service.ts - Clean service
@Injectable({ providedIn: 'root' })
export class PatientService {
  constructor(
    private http: HttpClient,
    private adapter: PatientAdapter  ← Injected adapter
  ) {}

  getPatientById(id: string): Observable<Patient> {
    return this.http.get<PatientDto>(`/api/patients/${id}`).pipe(
      map(dto => this.adapter.fromDto(dto))  ← Use adapter in pipe
    );
  }

  searchPatients(query: string): Observable<Patient[]> {
    return this.http.get<PatientDto[]>('/api/patients/search', {
      params: { q: query }
    }).pipe(
      map(dtos => this.adapter.fromDtoList(dtos))  ← Use adapter for lists
    );
  }

  createPatient(patient: Patient): Observable<Patient> {
    const dto = this.adapter.toDto(patient);
    return this.http.post<PatientDto>('/api/patients', dto).pipe(
      map(responseDto => this.adapter.fromDto(responseDto))
    );
  }
}
```

---

## 📋 Refactoring Checklist

### Phase 1: Backend Mapper Layer (Priority: CRITICAL)

- [ ] Create Mapster IRegister profiles for each service
- [ ] Extract MapToDto methods into dedicated Mapper classes
- [ ] Create service-level mapping configuration
- [ ] Register mappers in DI container
- [ ] Update all handlers to use injected mappers
- [ ] Remove inline mapping logic from handlers
- [ ] Add unit tests for mappers
- [ ] Update documentation

### Phase 2: Frontend Adapter Layer (Priority: HIGH)

- [ ] Create folder structure (models/domain, models/dto, models/adapters)
- [ ] Implement adapters for each feature domain model
- [ ] Create DTO interfaces matching API contracts
- [ ] Extract inline transformations to adapters
- [ ] Inject adapters into services
- [ ] Update RxJS pipes to use adapters
- [ ] Add unit tests for adapters
- [ ] Update service unit tests

### Phase 3: Validation & Testing (Priority: HIGH)

- [ ] Write mapper unit tests (backend)
- [ ] Write adapter unit tests (frontend)
- [ ] Add mapping configuration tests
- [ ] Integration tests for complete flow
- [ ] Performance tests for bulk mappings
- [ ] Documentation updates

---

## 🎯 SOLID Principles Applied

### Single Responsibility Principle (SRP)
- ✅ Each mapper handles ONE entity type and ONE direction (domain→DTO or DTO→domain)
- ✅ Handlers focus on business logic, not mapping
- ✅ Adapters handle value transformations only

### Open/Closed Principle (OCP)
- ✅ New mappings added by extending mapper classes
- ✅ New adapters for new models without modifying existing code
- ✅ Mapster profiles are open for extension

### Liskov Substitution Principle (LSP)
- ✅ All mappers implement consistent interface pattern
- ✅ Adapters provide consistent `fromDto()` and `toDto()` methods

### Interface Segregation Principle (ISP)
- ✅ Mappers injected only where needed
- ✅ Adapters provide specific conversion methods

### Dependency Inversion Principle (DIP)
- ✅ Handlers depend on abstraction (Mapper interface)
- ✅ Services depend on injected adapters
- ✅ Configuration via DI container

---

## 📚 Implementation Files Needed

### Backend Mapper Templates

```
Common/Mapping/
├── IMappingService.cs
├── MappingServiceBase.cs
└── MappingConfiguration.cs

Services.{Name}/Mappings/
├── {Service}MappingProfile.cs
├── {Entity}Mapper.cs
└── Converters/
    └── Custom{Type}Converter.cs
```

### Frontend Adapter Templates

```
features/{feature}/models/
├── domain/
│   └── {entity}.model.ts
├── dto/
│   └── {entity}.dto.ts
└── adapters/
    └── {entity}.adapter.ts
```

---

## 🔄 Migration Plan

### Week 1: Backend Mappers
- Day 1-2: Create mapping infrastructure
- Day 3-4: Refactor Billing service
- Day 5: Refactor Appointment service

### Week 2: Frontend Adapters
- Day 1-2: Create adapter infrastructure
- Day 3-5: Implement adapters for all features

### Week 3: Testing & Documentation
- Complete unit test coverage
- Integration test validation
- Update all documentation

---

## ✨ Benefits

| Area | Current | After Refactoring |
|------|---------|-------------------|
| **Testability** | Mapping mixed with logic | Isolated, unit-testable mappers |
| **Maintainability** | Scattered mapping code | Centralized, single location |
| **Reusability** | Duplicate MapToDto | Shared mapper instances |
| **Clarity** | Handlers do too much | Clear separation of concerns |
| **Performance** | No tracking | Can profile/optimize mappers |
| **Scalability** | Hard to add types | Easy to extend with new mappers |

---

## 📖 References

- **Mapster Documentation**: https://mapperly.riok.codes/ (or use Mapster package)
- **SOLID Principles**: Clean Code by Robert C. Martin
- **Angular Adapters**: Architectural Patterns in Angular
- **DDD Mapping**: Domain-Driven Design patterns for mapping layers
