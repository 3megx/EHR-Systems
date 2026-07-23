# Component Library

Complete documentation for all shared UI, layout, and domain components in Modern EHR Platform.

---

## 📚 Component Index

### Core UI Components
- [Button](#button)
- [Form Field](#form-field)
- [Modal](#modal)
- [Table](#table)
- [Pagination](#pagination)
- [Tabs](#tabs)
- [Dropdown](#dropdown)

### Layout Components
- [Sidebar](#sidebar)
- [Topbar](#topbar)
- [Breadcrumbs](#breadcrumbs)
- [Patient Header](#patient-header)

### Domain Components
- [File Uploader](#file-uploader)
- [Timeline](#timeline)
- [Vitals Card](#vitals-card)
- [Lab Results Summary](#lab-results-summary)

---

## 🎨 Core UI Components

### Button

Reusable button component with multiple variants and sizes.

**Location**: `src/app/shared/components/ui/button.component.ts`

**Usage**:
```typescript
import { ButtonComponent } from '@shared/components';

@Component({
  selector: 'app-example',
  template: `
    <app-button 
      variant="primary"
      size="lg"
      (clicked)="onButtonClick()">
      Click Me
    </app-button>
  `,
  standalone: true,
  imports: [ButtonComponent]
})
export class ExampleComponent {
  onButtonClick() {
    console.log('Button clicked');
  }
}
```

**API**:
```typescript
@Input() variant: 'primary' | 'secondary' | 'danger' | 'success' = 'primary';
@Input() size: 'sm' | 'md' | 'lg' = 'md';
@Input() disabled: boolean = false;
@Input() loading: boolean = false;
@Input() fullWidth: boolean = false;
@Input() type: 'button' | 'submit' | 'reset' = 'button';

@Output() clicked = new EventEmitter<void>();
```

**Variants**:
```html
<!-- Primary (Blue) -->
<app-button variant="primary">Primary</app-button>

<!-- Secondary (Gray) -->
<app-button variant="secondary">Secondary</app-button>

<!-- Danger (Red) -->
<app-button variant="danger">Delete</app-button>

<!-- Success (Green) -->
<app-button variant="success">Confirm</app-button>

<!-- Sizes -->
<app-button size="sm">Small</app-button>
<app-button size="md">Medium</app-button>
<app-button size="lg">Large</app-button>

<!-- States -->
<app-button [disabled]="true">Disabled</app-button>
<app-button [loading]="true">Loading...</app-button>
<app-button [fullWidth]="true">Full Width</app-button>
```

---

### Form Field

Wrapper component for form inputs with validation and error display.

**Location**: `src/app/shared/components/ui/form-field.component.ts`

**Usage**:
```typescript
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { FormFieldComponent } from '@shared/components';

@Component({
  selector: 'app-login-form',
  template: `
    <form [formGroup]="form">
      <app-form-field 
        label="Email"
        [error]="getFieldError('email')">
        <input 
          type="email"
          formControlName="email"
          placeholder="Enter your email"/>
      </app-form-field>
      
      <app-form-field 
        label="Password"
        [error]="getFieldError('password')">
        <input 
          type="password"
          formControlName="password"
          placeholder="Enter password"/>
      </app-form-field>
    </form>
  `,
  standalone: true,
  imports: [ReactiveFormsModule, FormFieldComponent]
})
export class LoginFormComponent {
  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });
  
  constructor(private fb: FormBuilder) {}
  
  getFieldError(fieldName: string): string | null {
    const field = this.form.get(fieldName);
    if (field?.hasError('required')) return 'This field is required';
    if (field?.hasError('email')) return 'Invalid email format';
    return null;
  }
}
```

**API**:
```typescript
@Input() label: string;
@Input() required: boolean = false;
@Input() error: string | null = null;
@Input() hint: string;
@Input() disabled: boolean = false;
```

---

### Modal

Dialog component for alerts, confirmations, and custom content.

**Location**: `src/app/shared/components/ui/modal.component.ts`

**Usage**:
```typescript
import { ModalComponent } from '@shared/components';
import { ModalService } from '@shared/services';

@Component({
  selector: 'app-patient-list',
  template: `
    <button (click)="openDeleteModal()">Delete Patient</button>
    <app-modal 
      [isOpen]="isModalOpen"
      title="Confirm Deletion"
      (onClose)="isModalOpen = false">
      <p>Are you sure you want to delete this patient?</p>
      <template #footer>
        <app-button 
          variant="danger"
          (clicked)="confirmDelete()">
          Delete
        </app-button>
        <app-button 
          variant="secondary"
          (clicked)="isModalOpen = false">
          Cancel
        </app-button>
      </template>
    </app-modal>
  `,
  standalone: true,
  imports: [ModalComponent, ButtonComponent]
})
export class PatientListComponent {
  isModalOpen = false;
  
  openDeleteModal() {
    this.isModalOpen = true;
  }
  
  confirmDelete() {
    // Delete logic
    this.isModalOpen = false;
  }
}
```

**API**:
```typescript
@Input() isOpen: boolean = false;
@Input() title: string;
@Input() size: 'sm' | 'md' | 'lg' = 'md';
@Input() closeOnBackdropClick: boolean = true;

@Output() onClose = new EventEmitter<void>();
```

---

### Table

Data table component with pagination, sorting, and filtering.

**Location**: `src/app/shared/components/ui/table.component.ts`

**Usage**:
```typescript
import { TableComponent } from '@shared/components';

interface PatientRow {
  id: string;
  name: string;
  mrn: string;
  status: string;
}

@Component({
  selector: 'app-patients-table',
  template: `
    <app-table 
      [columns]="columns"
      [data]="patients"
      [totalRecords]="totalPatients"
      [loading]="isLoading"
      (onPageChange)="onPageChange($event)"
      (onSort)="onSort($event)">
    </app-table>
  `,
  standalone: true,
  imports: [TableComponent]
})
export class PatientsTableComponent {
  patients: PatientRow[] = [];
  totalPatients = 0;
  isLoading = false;
  
  columns = [
    { field: 'name', header: 'Patient Name', sortable: true },
    { field: 'mrn', header: 'MRN', sortable: true },
    { field: 'status', header: 'Status', sortable: false }
  ];
  
  constructor(private patientService: PatientService) {}
  
  ngOnInit() {
    this.loadPatients();
  }
  
  loadPatients(page = 1, pageSize = 10) {
    this.isLoading = true;
    this.patientService.getPatients(page, pageSize).subscribe({
      next: (response) => {
        this.patients = response.data;
        this.totalPatients = response.totalRecords;
        this.isLoading = false;
      }
    });
  }
  
  onPageChange(page: number) {
    this.loadPatients(page);
  }
  
  onSort(event: any) {
    console.log('Sort by:', event.field, event.order);
  }
}
```

**API**:
```typescript
@Input() columns: TableColumn[];
@Input() data: any[];
@Input() totalRecords: number;
@Input() loading: boolean = false;
@Input() pageSize: number = 10;

@Output() onPageChange = new EventEmitter<number>();
@Output() onSort = new EventEmitter<SortEvent>();
```

---

### Pagination

Pagination controls for data navigation.

**Location**: `src/app/shared/components/ui/pagination.component.ts`

**Usage**:
```html
<app-pagination 
  [currentPage]="page"
  [pageSize]="pageSize"
  [totalRecords]="total"
  (pageChange)="onPageChange($event)">
</app-pagination>
```

---

### Tabs

Tabbed interface component.

**Location**: `src/app/shared/components/ui/tabs.component.ts`

**Usage**:
```html
<app-tabs>
  <app-tab label="Overview">
    <div>Tab 1 Content</div>
  </app-tab>
  <app-tab label="Details">
    <div>Tab 2 Content</div>
  </app-tab>
  <app-tab label="History">
    <div>Tab 3 Content</div>
  </app-tab>
</app-tabs>
```

---

### Dropdown

Select/dropdown component with search and grouping.

**Location**: `src/app/shared/components/ui/dropdown.component.ts`

**Usage**:
```typescript
<app-dropdown 
  [options]="roleOptions"
  [(ngModel)]="selectedRole"
  placeholder="Select Role"
  searchable="true">
</app-dropdown>
```

---

## 🏗️ Layout Components

### Sidebar

Navigation sidebar component.

**Location**: `src/app/shared/components/layout/sidebar.component.ts`

**Features**:
- Collapsible navigation
- Role-based menu items (uses `HasPermissionDirective`)
- Active route highlighting
- Dark mode support

**Usage**:
```html
<app-sidebar [collapsed]="isSidebarCollapsed">
  <!-- Navigation items are auto-populated based on user role -->
</app-sidebar>
```

---

### Topbar

Top navigation bar with user menu.

**Location**: `src/app/shared/components/layout/topbar.component.ts`

**Features**:
- Search bar
- Notifications
- User profile menu
- Dark mode toggle
- Language selector (i18n)

---

### Breadcrumbs

Navigation breadcrumbs component.

**Location**: `src/app/shared/components/layout/breadcrumbs.component.ts`

**Usage**:
```typescript
@Component({
  template: `
    <app-breadcrumbs 
      [items]="breadcrumbs">
    </app-breadcrumbs>
  `,
  imports: [BreadcrumbsComponent]
})
export class PatientDetailComponent implements OnInit {
  breadcrumbs = [
    { label: 'Home', route: '/dashboard' },
    { label: 'Patients', route: '/patients' },
    { label: 'John Smith', route: null }  // Current page
  ];
}
```

---

### Patient Header

Sticky header with patient key information.

**Location**: `src/app/shared/components/layout/patient-header.component.ts`

**Features**:
- Patient name and MRN
- Status indicator
- Quick actions (appointments, prescriptions)
- Allergies alert
- Always visible on patient pages

---

## 📊 Domain Components

### File Uploader

Component for file uploads with drag-and-drop.

**Location**: `src/app/shared/components/domain/file-uploader.component.ts`

**Usage**:
```typescript
@Component({
  template: `
    <app-file-uploader 
      [acceptedFileTypes]="['.pdf', '.jpg', '.png']"
      [maxFileSize]="10485760"
      (fileSelected)="onFileSelected($event)">
    </app-file-uploader>
  `,
  imports: [FileUploaderComponent]
})
export class MedicalRecordComponent {
  onFileSelected(file: File) {
    console.log('File selected:', file.name);
    // Upload to backend
  }
}
```

**Features**:
- Drag and drop
- File type validation
- File size validation
- Progress indicator
- Error handling

---

### Timeline

Medical history timeline component.

**Location**: `src/app/shared/components/domain/timeline.component.ts`

**Usage**:
```typescript
@Component({
  template: `
    <app-timeline [events]="medicalHistory">
    </app-timeline>
  `,
  imports: [TimelineComponent]
})
export class PatientHistoryComponent {
  medicalHistory = [
    {
      date: new Date(2024, 6, 15),
      type: 'appointment',
      title: 'Follow-up Appointment',
      description: 'Dr. Sarah Lee',
      status: 'completed'
    },
    {
      date: new Date(2024, 6, 10),
      type: 'medical_record',
      title: 'SOAP Note',
      description: 'Checkup - All vitals normal',
      status: 'completed'
    }
  ];
}
```

---

### Vitals Card

Card component displaying patient vital signs.

**Location**: `src/app/shared/components/domain/vitals-card.component.ts`

**Usage**:
```html
<app-vitals-card 
  [vitals]="patientVitals"
  [interpretation]="vitalInterpretation">
</app-vitals-card>
```

**Display**:
- Blood Pressure (with interpretation)
- Heart Rate
- Temperature
- Oxygen Saturation
- Respiratory Rate

---

### Lab Results Summary

Component for displaying lab test results.

**Location**: `src/app/shared/components/domain/lab-results-summary.component.ts`

**Usage**:
```html
<app-lab-results-summary 
  [results]="labResults"
  [showTrends]="true">
</app-lab-results-summary>
```

**Features**:
- Results display
- Trend charts
- Reference ranges
- Abnormal value highlighting

---

## 🎯 Using Shared Components

### Import from Barrel Export

```typescript
// ✅ GOOD: Import from barrel export
import { 
  ButtonComponent, 
  FormFieldComponent, 
  ModalComponent 
} from '@shared/components';

// ❌ AVOID: Direct imports
import { ButtonComponent } from '@shared/components/ui/button.component';
```

### Standalone Component Usage

All shared components are standalone and can be imported directly:

```typescript
@Component({
  selector: 'app-my-page',
  template: `
    <app-button (clicked)="doSomething()">Click</app-button>
  `,
  standalone: true,
  imports: [ButtonComponent, CommonModule]
})
export class MyPageComponent {}
```

---

## 🎨 Theming & Customization

### CSS Classes

All components support custom classes:

```html
<app-button 
  class="custom-button"
  variant="primary">
  Button
</app-button>
```

### Dark Mode

Components automatically adapt to dark mode:

```typescript
// Dark mode is toggled in TopbarComponent
// Or programmatically:
this.themeService.setDarkMode(true);
```

---

## 🧪 Component Testing

Example test for Button component:

```typescript
describe('ButtonComponent', () => {
  let component: ButtonComponent;
  let fixture: ComponentFixture<ButtonComponent>;
  
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ButtonComponent]
    }).compileComponents();
    
    fixture = TestBed.createComponent(ButtonComponent);
    component = fixture.componentInstance;
  });
  
  it('should emit clicked event', () => {
    spyOn(component.clicked, 'emit');
    component.click();
    expect(component.clicked.emit).toHaveBeenCalled();
  });
});
```

---

## 📖 Best Practices

1. **Use Type Safety**: Always define input/output types
2. **Accessibility**: Add ARIA labels and keyboard navigation
3. **Responsive**: Test on mobile, tablet, desktop
4. **Performance**: Use OnPush change detection
5. **Documentation**: JSDoc comments for public APIs
6. **Testing**: Test happy path, errors, and edge cases
7. **Reusability**: Design for multiple use cases

---

**Version**: 1.0.0 | Last Updated: July 2026
