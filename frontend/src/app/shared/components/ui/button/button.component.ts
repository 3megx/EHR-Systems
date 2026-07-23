import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

export type ButtonVariant = 'primary' | 'secondary' | 'danger' | 'success' | 'warning';
export type ButtonSize = 'sm' | 'md' | 'lg';

/**
 * Button Component
 * Reusable button with multiple variants and sizes
 */
@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button
      [ngClass]="getClasses()"
      [disabled]="disabled || loading"
      (click)="onClick()"
      type="button"
    >
      <span *ngIf="loading" class="mr-2">
        <span class="inline-block animate-spin">⟳</span>
      </span>
      <ng-content></ng-content>
    </button>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ButtonComponent {
  @Input() variant: ButtonVariant = 'primary';
  @Input() size: ButtonSize = 'md';
  @Input() disabled = false;
  @Input() loading = false;
  @Output() clicked = new EventEmitter<void>();

  onClick(): void {
    if (!this.disabled && !this.loading) {
      this.clicked.emit();
    }
  }

  getClasses(): Record<string, boolean> {
    return {
      // Base styles
      'inline-flex items-center justify-center font-semibold rounded-lg transition duration-200':
        true,
      'focus:outline-none focus:ring-2 focus:ring-offset-2': true,

      // Variants
      'bg-blue-600 text-white hover:bg-blue-700 focus:ring-blue-500':
        this.variant === 'primary',
      'bg-gray-200 text-gray-900 hover:bg-gray-300 focus:ring-gray-500':
        this.variant === 'secondary',
      'bg-red-600 text-white hover:bg-red-700 focus:ring-red-500':
        this.variant === 'danger',
      'bg-green-600 text-white hover:bg-green-700 focus:ring-green-500':
        this.variant === 'success',
      'bg-yellow-600 text-white hover:bg-yellow-700 focus:ring-yellow-500':
        this.variant === 'warning',

      // Sizes
      'px-3 py-1 text-sm': this.size === 'sm',
      'px-4 py-2 text-base': this.size === 'md',
      'px-6 py-3 text-lg': this.size === 'lg',

      // States
      'opacity-50 cursor-not-allowed': this.disabled || this.loading,
    };
  }
}
