# Backend Mapping Refactoring - Implementation Templates

## Template 1: Mapster IRegister Profile

**File**: `Services.{ServiceName}/Mappings/{Entity}MappingProfile.cs`

```csharp
using Mapster;
using EHRPlatform.Services.{ServiceName}.Features.{Feature}.Domain;
using EHRPlatform.Services.{ServiceName}.Features.{Feature}.Queries;

namespace EHRPlatform.Services.{ServiceName}.Mappings;

/// <summary>
/// Mapster registration profile for {Entity} entity mappings.
/// Handles conversion between domain models and DTOs.
/// </summary>
public class {Entity}MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Domain to DTO mappings
        config.NewConfig<{Entity}, {Entity}ResponseDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Property1, src => src.Property1)
            // Add more mappings as needed
            ;

        config.NewConfig<{Entity}, {Entity}ListDto>();
        
        // Nested entity mappings (if applicable)
        config.NewConfig<NestedEntity, NestedEntityDto>();

        // Custom type converters (if needed)
        config.NewConfig<ComplexType, SimplifiedType>()
            .MapWith(src => new SimplifiedType 
            { 
                Value = src.ComplexLogic() 
            });
    }
}
```

## Template 2: Service-Level Mapper

**File**: `Services.{ServiceName}/Mappings/{Entity}Mapper.cs`

```csharp
using Mapster;
using EHRPlatform.Services.{ServiceName}.Features.{Feature}.Domain;
using EHRPlatform.Services.{ServiceName}.Features.{Feature}.Queries;

namespace EHRPlatform.Services.{ServiceName}.Mappings;

/// <summary>
/// Mapper for {Entity} domain model conversions.
/// Provides single responsibility mapping methods with optional post-processing.
/// </summary>
public class {Entity}Mapper
{
    public {Entity}ResponseDto MapToResponseDto({Entity} entity)
    {
        return entity.Adapt<{Entity}ResponseDto>();
    }

    public {Entity}ListDto MapToListDto(
        ICollection<{Entity}> entities,
        int total,
        int pageNumber,
        int pageSize)
    {
        return new {Entity}ListDto
        {
            Items = entities.Adapt<List<{Entity}ResponseDto>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public List<{Entity}ResponseDto> MapToResponseDtoList(ICollection<{Entity}> entities)
    {
        return entities.Adapt<List<{Entity}ResponseDto>>();
    }

    /// <summary>
    /// Custom mapping with post-processing logic.
    /// Use when domain object needs enrichment before returning to client.
    /// </summary>
    public {Entity}DetailedResponseDto MapToDetailedResponseDto({Entity} entity, object? enrichmentData = null)
    {
        var dto = entity.Adapt<{Entity}DetailedResponseDto>();

        // Post-process if enrichment data provided
        if (enrichmentData != null)
        {
            // Apply enrichment logic
        }

        return dto;
    }
}
```

## Template 3: Handler Using Mapper

**File**: `Services.{ServiceName}/Features/{Feature}/Queries/{Entity}QueryHandler.cs`

```csharp
using MediatR;
using EHRPlatform.Common.Data;
using EHRPlatform.Common.CQRS;
using EHRPlatform.Services.{ServiceName}.Features.{Feature}.Domain;
using EHRPlatform.Services.{ServiceName}.Mappings;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.{ServiceName}.Features.{Feature}.Queries;

public class Get{Entity}Query : IQuery<{Entity}ResponseDto>
{
    public string Id { get; set; }
    public Get{Entity}Query(string id) => Id = id;
}

/// <summary>
/// Query handler with injected mapper.
/// Focuses ONLY on business logic, not data transformation.
/// </summary>
public class Get{Entity}QueryHandler : IQueryHandler<Get{Entity}Query, {Entity}ResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly {Entity}Mapper _mapper;  ← Injected mapper
    private readonly ILogger<Get{Entity}QueryHandler> _logger;

    public Get{Entity}QueryHandler(
        IUnitOfWork unitOfWork,
        {Entity}Mapper mapper,
        ILogger<Get{Entity}QueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<{Entity}ResponseDto> Handle(
        Get{Entity}Query request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching {Entity} with ID {Id}", typeof({Entity}).Name, request.Id);

        var repo = _unitOfWork.Repository<{Entity}>();
        var entity = await repo.FirstOrDefaultAsync(
            q => q.Where(x => x.Id == request.Id),
            cancellationToken);

        if (entity == null)
            throw new InvalidOperationException($"{typeof({Entity}).Name} {request.Id} not found");

        // Use mapper - business logic clean, mapping delegated
        return _mapper.MapToResponseDto(entity);
    }
}
```

## Template 4: DI Registration

**File**: `Services.{ServiceName}/Program.cs`

```csharp
// In ConfigureServices or Program.cs

// Register Mapster with mapping profile
services.AddMapster();
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.AssignableTo(typeof(IRegister)))
    .As(typeof(IRegister))
    .WithTransientLifetime());

// Register all mappers
services.AddScoped<{Entity}Mapper>();
services.AddScoped<{AnotherEntity}Mapper>();
// ... register all mappers

// Or use reflection to auto-register all mappers
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes
        .Where(c => c.Name.EndsWith("Mapper") && !c.IsAbstract))
    .AsSelfWithInterfaces()
    .WithTransientLifetime());
```

## Template 5: Unit Test for Mapper

**File**: `Services.{ServiceName}.Tests/Mappings/{Entity}MapperTests.cs`

```csharp
using Xunit;
using Mapster;
using EHRPlatform.Services.{ServiceName}.Mappings;
using EHRPlatform.Services.{ServiceName}.Features.{Feature}.Domain;

namespace EHRPlatform.Services.{ServiceName}.Tests.Mappings;

public class {Entity}MapperTests
{
    private readonly {Entity}Mapper _mapper;

    public {Entity}MapperTests()
    {
        // Configure Mapster for testing
        TypeAdapterConfig.GlobalSettings.Compile();
        _mapper = new {Entity}Mapper();
    }

    [Fact]
    public void MapToResponseDto_WithValidEntity_ReturnsMappedDto()
    {
        // Arrange
        var entity = new {Entity}
        {
            Id = Guid.NewGuid(),
            Property1 = "value1",
            Property2 = 42
        };

        // Act
        var result = _mapper.MapToResponseDto(entity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Property1, result.Property1);
    }

    [Fact]
    public void MapToListDto_WithMultipleEntities_ReturnsPaginatedDto()
    {
        // Arrange
        var entities = new List<{Entity}>
        {
            new { Id = Guid.NewGuid(), Property1 = "value1" },
            new { Id = Guid.NewGuid(), Property1 = "value2" }
        };
        int total = 100;
        int pageNumber = 1;
        int pageSize = 2;

        // Act
        var result = _mapper.MapToListDto(entities, total, pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(total, result.Total);
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
    }

    [Fact]
    public void MapToResponseDtoList_WithValidEntities_ReturnsMappedList()
    {
        // Arrange
        var entities = new List<{Entity}>
        {
            new { Id = Guid.NewGuid(), Property1 = "value1" },
            new { Id = Guid.NewGuid(), Property1 = "value2" }
        };

        // Act
        var result = _mapper.MapToResponseDtoList(entities);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
}
```

---

## Implementation Checklist

- [ ] Create `{Service}/Mappings/` folder
- [ ] Create `{Entity}MappingProfile.cs` (IRegister)
- [ ] Create `{Entity}Mapper.cs` service
- [ ] Register mapper in DI container
- [ ] Inject mapper into all handlers
- [ ] Replace inline `MapToDto` calls with `_mapper.MapTo*()`
- [ ] Create unit tests for mapper
- [ ] Delete inline mapping methods
- [ ] Update handler documentation
- [ ] Verify all tests pass
- [ ] Code review & merge

---

## Mapster Common Patterns

```csharp
// Custom mapping with conditions
config.NewConfig<Source, Destination>()
    .Map(dest => dest.Status, src => src.StatusCode == 1 ? "Active" : "Inactive");

// Flattening nested objects
config.NewConfig<Address, AddressDto>()
    .Map(dest => dest.FullAddress, src => $"{src.Street}, {src.City}");

// Reverse mapping
config.NewConfig<Entity, EntityDto>()
    .ReverseMap();  // Enables both directions

// Custom type converter
config.NewConfig<DateTime, string>()
    .MapWith(src => src.ToString("yyyy-MM-dd"));

// Ignore properties
config.NewConfig<Source, Destination>()
    .Ignore(dest => dest.CreatedAt);
```

---

## Migration Checklist (Per Service)

1. **Billing Service** (CRITICAL)
   - [ ] Remove duplicate MapToDto methods
   - [ ] Create InvoiceMappingProfile
   - [ ] Create InvoiceMapper
   - [ ] Update all handlers

2. **Appointment Service** (HIGH)
   - [ ] Create AppointmentMappingProfile
   - [ ] Create AppointmentMapper
   - [ ] Centralize slot mapping

3. **All Other Services**
   - [ ] Follow same pattern
   - [ ] One feature = one mapper

