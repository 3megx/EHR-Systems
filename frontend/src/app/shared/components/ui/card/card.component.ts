import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Card Component
 * Reusable card container for content
 */
@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [ngClass]="getClasses()">
      <div *ngIf="title" class="border-b border-gray-200 dark:border-gray-700 pb-4 mb-4">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ title }}</h3>
      </div>
      <ng-content></ng-content>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CardComponent {
  @Input() title?: string;
  @Input() padding: 'sm' | 'md' | 'lg' = 'md';

  getClasses(): Record<string, boolean> {
    return {
      'bg-white dark:bg-gray-800 rounded-lg shadow-md': true,
      'border border-gray-200 dark:border-gray-700': true,
      'p-3': this.padding === 'sm',
      'p-4': this.padding === 'md',
      'p-6': this.padding === 'lg',
    };
  }
}
