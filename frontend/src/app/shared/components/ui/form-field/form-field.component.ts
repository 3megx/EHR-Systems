import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl } from '@angular/forms';

export type InputType = 'text' | 'email' | 'password' | 'number' | 'date' | 'tel' | 'url';

/**
 * Form Field Component
 * Wrapper for form inputs with labels, error messages, and validation
 * Usage: <app-form-field [control]="emailControl" label="Email" type="email" required />
 */
@Component({
  selector: 'app-form-field',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="mb-4">
      <label *ngIf="label" [for]="fieldId" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
        {{ label }}
        <span *ngIf="required" class="text-red-600">*</span>
      </label>

      <input
        [id]="fieldId"
        [type]="type"
        [formControl]="control"
        [placeholder]="placeholder"
        [disabled]="disabled"
        [required]="required"
        class="w-full px-4 py-2 border rounded-lg transition-colors"
        [ngClass]="getInputClasses()"
      />

      <!-- Error Messages -->
      <div *ngIf="control.invalid && control.touched" class="mt-2 space-y-1">
        <p *ngIf="control.errors?.['required']" class="text-sm text-red-600">
          {{ label || 'This field' }} is required
        </p>
        <p *ngIf="control.errors?.['email']" class="text-sm text-red-600">
          Please enter a valid email
        </p>
        <p *ngIf="control.errors?.['minlength']" class="text-sm text-red-600">
          Minimum length is {{ control.errors?.['minlength'].requiredLength }}
        </p>
        <p *ngIf="control.errors?.['maxlength']" class="text-sm text-red-600">
          Maximum length is {{ control.errors?.['maxlength'].requiredLength }}
        </p>
        <p *ngIf="control.errors?.['pattern']" class="text-sm text-red-600">
          Invalid format
        </p>
        <p *ngIf="control.errors?.['min']" class="text-sm text-red-600">
          Minimum value is {{ control.errors?.['min'].min }}
        </p>
        <p *ngIf="control.errors?.['max']" class="text-sm text-red-600">
          Maximum value is {{ control.errors?.['max'].max }}
        </p>
        <p *ngIf="error" class="text-sm text-red-600">{{ error }}</p>
      </div>

      <!-- Helper Text -->
      <p *ngIf="hint && !control.invalid" class="mt-1 text-xs text-gray-500 dark:text-gray-400">
        {{ hint }}
      </p>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormFieldComponent {
  @Input() control!: FormControl;
  @Input() label = '';
  @Input() placeholder = '';
  @Input() type: InputType = 'text';
  @Input() required = false;
  @Input() disabled = false;
  @Input() error = '';
  @Input() hint = '';
  @Input() fieldId = `field-${Math.random().toString(36).substr(2, 9)}`;

  getInputClasses(): Record<string, boolean> {
    return {
      'border-gray-300 dark:border-gray-600 focus:border-blue-500 focus:ring-2 focus:ring-blue-200':
        !this.control.invalid || !this.control.touched,
      'border-red-600 focus:border-red-600 focus:ring-2 focus:ring-red-200':
        this.control.invalid && this.control.touched,
      'dark:bg-gray-700 dark:text-white': true,
      'bg-gray-50 cursor-not-allowed': this.disabled,
    };
  }
}
