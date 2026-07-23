# Frontend Adapter Pattern - Implementation Templates

## Folder Structure (Per Feature)

```
frontend/src/app/features/{feature}/
├── models/
│   ├── domain/
│   │   ├── {entity}.model.ts
│   │   └── index.ts
│   ├── dto/
│   │   ├── {entity}.dto.ts
│   │   └── index.ts
│   ├── adapters/
│   │   ├── {entity}.adapter.ts
│   │   ├── {entity}.adapter.spec.ts
│   │   └── index.ts
│   └── index.ts
├── pages/
├── components/
├── services/
│   ├── {feature}.service.ts
│   └── {feature}.service.spec.ts
└── {feature}.routes.ts
```

---

## Template 1: DTO (API Contract)

**File**: `features/{feature}/models/dto/{entity}.dto.ts`

```typescript
/**
 * Data Transfer Objects (DTOs)
 * Represent API response/request contracts - exactly as received from backend
 */

export interface {Entity}Dto {
  id: string;
  property1: string;
  property2: number;
  dateField: string;  // ISO string from API
  nestedObject: {
    fieldA: string;
    fieldB: number;
  };
}

export interface {Entity}ListDto {
  items: {Entity}Dto[];
  total: number;
  pageNumber: number;
  pageSize: number;
}

export interface Create{Entity}Dto {
  property1: string;
  property2: number;
  // Omit server-generated fields (id, createdAt, etc.)
}

export interface Update{Entity}Dto {
  property1?: string;
  property2?: number;
}
```

## Template 2: Domain Model

**File**: `features/{feature}/models/domain/{entity}.model.ts`

```typescript
/**
 * Domain Models
 * Component-ready models with transformed/computed properties
 * May include calculated fields, formatted dates, enums, etc.
 */

export enum {Entity}Status {
  Active = 'ACTIVE',
  Inactive = 'INACTIVE',
  Archived = 'ARCHIVED'
}

export interface {Entity} {
  id: string;
  property1: string;
  property2: number;
  dateField: Date;          // Converted to Date object
  status: {Entity}Status;   // Enum for type safety
  formattedDate: string;    // Pre-formatted for display
  computedField: string;    // Calculated/derived value
  nestedObject: {
    fieldA: string;
    fieldB: number;
    displayName: string;    // Mapped/computed
  };
}

export interface {Entity}List {
  items: {Entity}[];
  total: number;
  pageNumber: number;
  pageSize: number;
  hasMorePages: boolean;    // Computed field
}

export interface Create{Entity}Request {
  property1: string;
  property2: number;
}

export interface Update{Entity}Request {
  property1?: string;
  property2?: number;
}
```

## Template 3: Adapter

**File**: `features/{feature}/models/adapters/{entity}.adapter.ts`

```typescript
import { Injectable } from '@angular/core';
import { {Entity}Dto, {Entity}ListDto } from '../dto';
import { {Entity}, {Entity}List, {Entity}Status } from '../domain';

/**
 * {Entity} Adapter
 * 
 * Single Responsibility: Convert between API DTOs and domain models.
 * Handles:
 * - Date transformations (string ↔ Date)
 * - Enum mappings
 * - Computed/derived fields
 * - Nested object conversions
 * - Data enrichment/flattening
 */
@Injectable({ providedIn: 'root' })
export class {Entity}Adapter {
  
  /**
   * Convert API DTO to domain model
   * @param dto Data Transfer Object from API
   * @returns Domain-ready model with transformed properties
   */
  fromDto(dto: {Entity}Dto): {Entity} {
    const dateField = new Date(dto.dateField);
    
    return {
      id: dto.id,
      property1: dto.property1,
      property2: dto.property2,
      dateField,
      status: this.mapToStatus(dto.statusCode),
      formattedDate: this.formatDate(dateField),
      computedField: this.computeField(dto.property1, dto.property2),
      nestedObject: {
        fieldA: dto.nestedObject.fieldA,
        fieldB: dto.nestedObject.fieldB,
        displayName: `${dto.nestedObject.fieldA} - ${dto.nestedObject.fieldB}`
      }
    };
  }

  /**
   * Convert domain model to API DTO for submission
   */
  toDto(model: {Entity}): {Entity}Dto {
    return {
      id: model.id,
      property1: model.property1,
      property2: model.property2,
      dateField: model.dateField.toISOString(),
      nestedObject: {
        fieldA: model.nestedObject.fieldA,
        fieldB: model.nestedObject.fieldB
      }
    };
  }

  /**
   * Convert array of DTOs to domain models
   */
  fromDtoList(dtos: {Entity}Dto[]): {Entity}[] {
    return dtos.map(dto => this.fromDto(dto));
  }

  /**
   * Convert paginated DTO response
   */
  fromListDto(listDto: {Entity}ListDto): {Entity}List {
    return {
      items: this.fromDtoList(listDto.items),
      total: listDto.total,
      pageNumber: listDto.pageNumber,
      pageSize: listDto.pageSize,
      hasMorePages: listDto.pageNumber * listDto.pageSize < listDto.total
    };
  }

  /**
   * Transform for creation request (omit server fields)
   */
  toCreateRequest(model: Omit<{Entity}, 'id' | 'formattedDate' | 'computedField'>): {Entity}Dto {
    return {
      id: '', // Server will generate
      property1: model.property1,
      property2: model.property2,
      dateField: model.dateField.toISOString(),
      nestedObject: model.nestedObject
    };
  }

  // ===== Private Mapping Methods =====

  private mapToStatus(statusCode: number): {Entity}Status {
    const statusMap: Record<number, {Entity}Status> = {
      1: {Entity}Status.Active,
      2: {Entity}Status.Inactive,
      3: {Entity}Status.Archived
    };
    return statusMap[statusCode] || {Entity}Status.Inactive;
  }

  private formatDate(date: Date): string {
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  private computeField(prop1: string, prop2: number): string {
    return `${prop1}:${prop2}`;
  }
}
```

## Template 4: Service Using Adapter

**File**: `features/{feature}/services/{feature}.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';

import { environment } from '@env/environment';
import { {Entity}Dto, {Entity}ListDto } from '../models/dto';
import { {Entity}, {Entity}List } from '../models/domain';
import { {Entity}Adapter } from '../models/adapters';

/**
 * {Feature} Service
 * 
 * Responsibilities:
 * - API communication
 * - Data fetching/submission
 * - Using adapter for transformations
 * 
 * Does NOT contain:
 * - Mapping logic (→ adapter)
 * - Business logic (→ components/store)
 * - State management (→ store)
 */
@Injectable({ providedIn: 'root' })
export class {Feature}Service {
  private apiUrl = `${environment.apiUrl}/{entities}`;

  constructor(
    private http: HttpClient,
    private adapter: {Entity}Adapter
  ) {}

  /**
   * Get single {entity}
   */
  get{Entity}(id: string): Observable<{Entity}> {
    return this.http.get<{Entity}Dto>(`${this.apiUrl}/${id}`).pipe(
      map(dto => this.adapter.fromDto(dto))
    );
  }

  /**
   * Get paginated list
   */
  get{Entity}List(
    pageNumber: number,
    pageSize: number,
    filters?: Record<string, any>
  ): Observable<{Entity}List> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (filters) {
      Object.entries(filters).forEach(([key, value]) => {
        params = params.set(key, value);
      });
    }

    return this.http.get<{Entity}ListDto>(this.apiUrl, { params }).pipe(
      map(listDto => this.adapter.fromListDto(listDto))
    );
  }

  /**
   * Search
   */
  search(query: string): Observable<{Entity}[]> {
    return this.http.get<{Entity}Dto[]>(`${this.apiUrl}/search`, {
      params: new HttpParams().set('q', query)
    }).pipe(
      map(dtos => this.adapter.fromDtoList(dtos))
    );
  }

  /**
   * Create
   */
  create(model: Omit<{Entity}, 'id' | 'formattedDate' | 'computedField'>): Observable<{Entity}> {
    const dto = this.adapter.toCreateRequest(model);
    return this.http.post<{Entity}Dto>(this.apiUrl, dto).pipe(
      map(responseDto => this.adapter.fromDto(responseDto)),
      tap(created => console.log('Created:', created))
    );
  }

  /**
   * Update
   */
  update(id: string, model: Partial<{Entity}>): Observable<{Entity}> {
    const dto = this.adapter.toDto(model as {Entity});
    return this.http.put<{Entity}Dto>(`${this.apiUrl}/${id}`, dto).pipe(
      map(responseDto => this.adapter.fromDto(responseDto))
    );
  }

  /**
   * Delete
   */
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

## Template 5: Unit Tests

**File**: `features/{feature}/models/adapters/{entity}.adapter.spec.ts`

```typescript
import { TestBed } from '@angular/core/testing';
import { {Entity}Adapter } from './{entity}.adapter';
import { {Entity}Dto, {Entity}ListDto } from '../dto';
import { {Entity}Status } from '../domain';

describe('{Entity}Adapter', () => {
  let adapter: {Entity}Adapter;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [{Entity}Adapter]
    });
    adapter = TestBed.inject({Entity}Adapter);
  });

  describe('fromDto', () => {
    it('should convert DTO to domain model', () => {
      // Arrange
      const dto: {Entity}Dto = {
        id: '123',
        property1: 'test',
        property2: 42,
        dateField: '2024-01-15T10:00:00Z',
        nestedObject: { fieldA: 'a', fieldB: 1 }
      };

      // Act
      const result = adapter.fromDto(dto);

      // Assert
      expect(result.id).toBe('123');
      expect(result.property1).toBe('test');
      expect(result.property2).toBe(42);
      expect(result.dateField).toBeInstanceOf(Date);
      expect(result.status).toBeDefined();
    });

    it('should compute derived fields', () => {
      // Arrange
      const dto: {Entity}Dto = {
        id: '123',
        property1: 'test',
        property2: 42,
        dateField: '2024-01-15T10:00:00Z',
        nestedObject: { fieldA: 'a', fieldB: 1 }
      };

      // Act
      const result = adapter.fromDto(dto);

      // Assert
      expect(result.computedField).toBe('test:42');
      expect(result.formattedDate).toBeDefined();
    });
  });

  describe('toDto', () => {
    it('should convert domain model back to DTO', () => {
      // Arrange
      const model: {Entity} = {
        id: '123',
        property1: 'test',
        property2: 42,
        dateField: new Date('2024-01-15'),
        status: {Entity}Status.Active,
        formattedDate: 'Jan 15, 2024',
        computedField: 'test:42',
        nestedObject: { fieldA: 'a', fieldB: 1, displayName: 'a - 1' }
      };

      // Act
      const result = adapter.toDto(model);

      // Assert
      expect(result.id).toBe('123');
      expect(result.dateField).toMatch(/^\d{4}-\d{2}-\d{2}/);
    });
  });

  describe('fromListDto', () => {
    it('should convert paginated DTO list', () => {
      // Arrange
      const listDto: {Entity}ListDto = {
        items: [
          {
            id: '1',
            property1: 'test1',
            property2: 10,
            dateField: '2024-01-15T10:00:00Z',
            nestedObject: { fieldA: 'a', fieldB: 1 }
          }
        ],
        total: 100,
        pageNumber: 1,
        pageSize: 10
      };

      // Act
      const result = adapter.fromListDto(listDto);

      // Assert
      expect(result.items.length).toBe(1);
      expect(result.total).toBe(100);
      expect(result.hasMorePages).toBe(true);
    });
  });
});
```

## Template 6: Module Barrel Export

**File**: `features/{feature}/models/index.ts`

```typescript
// Domain models
export * from './domain/{entity}.model';

// DTOs
export * from './dto/{entity}.dto';

// Adapters
export * from './adapters/{entity}.adapter';
```

---

## Implementation Checklist

- [ ] Create `models/domain/` folder
- [ ] Create `models/dto/` folder
- [ ] Create `models/adapters/` folder
- [ ] Write all DTO interfaces
- [ ] Write all domain model interfaces
- [ ] Implement adapter with all methods
- [ ] Write adapter unit tests
- [ ] Update service to use adapter
- [ ] Update service tests
- [ ] Create barrel exports
- [ ] Update component imports
- [ ] Verify all tests pass

---

## Common Adapter Patterns

```typescript
// Enum mapping
private mapToStatus(apiCode: string): Status {
  const map: Record<string, Status> = {
    'A': Status.Active,
    'I': Status.Inactive
  };
  return map[apiCode] || Status.Inactive;
}

// Date transformation
private fromApiDate(dateStr: string): Date {
  return new Date(dateStr);
}

// Nested object flattening
private flattenAddress(dto: AddressDto): FlatAddress {
  return {
    fullAddress: `${dto.street}, ${dto.city}, ${dto.state}`
  };
}

// Array transformation
private transformItems(items: ItemDto[]): Item[] {
  return items
    .filter(item => item.active)
    .map(item => this.transformItem(item));
}

// Computed field
private calculateAge(birthDate: Date): number {
  return Math.floor((new Date().getTime() - birthDate.getTime()) / 
    (365.25 * 24 * 60 * 60 * 1000));
}
```

