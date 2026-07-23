import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface Vital {
  name: string;
  value: number | string;
  unit: string;
  normal: { min: number; max: number };
  status: 'normal' | 'warning' | 'critical';
  timestamp: Date;
  trend?: 'up' | 'down' | 'stable';
}

/**
 * Vitals Card Component
 * Displays patient vital signs
 * Usage: <app-vitals-card [vitals]="patientVitals" />
 */
@Component({
  selector: 'app-vitals-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <div
        *ngFor="let vital of vitals"
        class="p-4 rounded-lg border"
        [ngClass]="getVitalClasses(vital.status)"
      >
        <!-- Header -->
        <div class="flex items-center justify-between mb-3">
          <h4 class="font-semibold text-gray-900 dark:text-white">{{ vital.name }}</h4>
          <span *ngIf="vital.trend" [ngClass]="getTrendClasses(vital.trend)">
            {{ getTrendIcon(vital.trend) }}
          </span>
        </div>

        <!-- Value -->
        <div class="mb-3">
          <div class="text-3xl font-bold" [ngClass]="getTextColorClasses(vital.status)">
            {{ vital.value }}
          </div>
          <div class="text-sm text-gray-600 dark:text-gray-400">
            {{ vital.unit }}
          </div>
        </div>

        <!-- Status Badge -->
        <div class="flex items-center justify-between">
          <span class="text-xs" [ngClass]="getStatusLabelClasses(vital.status)">
            {{ vital.status }}
          </span>
          <span class="text-xs text-gray-500 dark:text-gray-500">
            {{ vital.timestamp | dateFormat: 'time' }}
          </span>
        </div>

        <!-- Normal Range -->
        <div class="mt-2 text-xs text-gray-600 dark:text-gray-400">
          Normal: {{ vital.normal.min }}-{{ vital.normal.max }}
        </div>
      </div>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class VitalsCardComponent {
  @Input() vitals: Vital[] = [];

  getVitalClasses(status: string): Record<string, boolean> {
    return {
      'bg-green-50 dark:bg-green-900/20 border-green-200 dark:border-green-800':
        status === 'normal',
      'bg-yellow-50 dark:bg-yellow-900/20 border-yellow-200 dark:border-yellow-800':
        status === 'warning',
      'bg-red-50 dark:bg-red-900/20 border-red-200 dark:border-red-800':
        status === 'critical',
    };
  }

  getTextColorClasses(status: string): Record<string, boolean> {
    return {
      'text-green-600 dark:text-green-400': status === 'normal',
      'text-yellow-600 dark:text-yellow-400': status === 'warning',
      'text-red-600 dark:text-red-400': status === 'critical',
    };
  }

  getStatusLabelClasses(status: string): Record<string, boolean> {
    return {
      'px-2 py-1 bg-green-200 dark:bg-green-800 text-green-800 dark:text-green-200 rounded':
        status === 'normal',
      'px-2 py-1 bg-yellow-200 dark:bg-yellow-800 text-yellow-800 dark:text-yellow-200 rounded':
        status === 'warning',
      'px-2 py-1 bg-red-200 dark:bg-red-800 text-red-800 dark:text-red-200 rounded':
        status === 'critical',
    };
  }

  getTrendIcon(trend: string): string {
    switch (trend) {
      case 'up':
        return '📈';
      case 'down':
        return '📉';
      case 'stable':
        return '➡️';
      default:
        return '';
    }
  }

  getTrendClasses(trend: string): Record<string, boolean> {
    return {
      'text-red-600 dark:text-red-400': trend === 'up',
      'text-green-600 dark:text-green-400': trend === 'down',
      'text-gray-600 dark:text-gray-400': trend === 'stable',
    };
  }
}
