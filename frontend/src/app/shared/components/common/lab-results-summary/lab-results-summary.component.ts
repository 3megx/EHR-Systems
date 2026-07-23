import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface LabResult {
  id: string;
  name: string;
  value: number;
  unit: string;
  normal: { min: number; max: number };
  status: 'normal' | 'abnormal' | 'critical';
  testDate: Date;
  previousValue?: number;
}

/**
 * Lab Results Summary Component
 * Displays lab test results with status
 * Usage: <app-lab-results-summary [results]="labTests" />
 */
@Component({
  selector: 'app-lab-results-summary',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="overflow-x-auto">
      <table class="w-full">
        <thead>
          <tr class="bg-gray-100 dark:bg-gray-700 border-b border-gray-200 dark:border-gray-600">
            <th class="px-4 py-3 text-left text-sm font-semibold text-gray-900 dark:text-white">
              Test Name
            </th>
            <th class="px-4 py-3 text-left text-sm font-semibold text-gray-900 dark:text-white">
              Value
            </th>
            <th class="px-4 py-3 text-left text-sm font-semibold text-gray-900 dark:text-white">
              Normal Range
            </th>
            <th class="px-4 py-3 text-left text-sm font-semibold text-gray-900 dark:text-white">
              Status
            </th>
            <th class="px-4 py-3 text-left text-sm font-semibold text-gray-900 dark:text-white">
              Change
            </th>
            <th class="px-4 py-3 text-left text-sm font-semibold text-gray-900 dark:text-white">
              Date
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            *ngFor="let result of results"
            class="border-b border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700/50"
          >
            <td class="px-4 py-3 text-sm font-medium text-gray-900 dark:text-white">
              {{ result.name }}
            </td>
            <td class="px-4 py-3 text-sm" [ngClass]="getValueClasses(result.status)">
              <strong>{{ result.value }}</strong> {{ result.unit }}
            </td>
            <td class="px-4 py-3 text-sm text-gray-600 dark:text-gray-400">
              {{ result.normal.min }}-{{ result.normal.max }} {{ result.unit }}
            </td>
            <td class="px-4 py-3 text-sm">
              <span
                class="px-3 py-1 rounded-full text-xs font-semibold"
                [ngClass]="getStatusBadgeClasses(result.status)"
              >
                {{ result.status }}
              </span>
            </td>
            <td class="px-4 py-3 text-sm">
              <span *ngIf="result.previousValue !== undefined" [ngClass]="getTrendClasses(result)">
                {{ getTrendIcon(result) }} {{ getTrendPercent(result) }}%
              </span>
              <span *ngIf="result.previousValue === undefined" class="text-gray-400">
                —
              </span>
            </td>
            <td class="px-4 py-3 text-sm text-gray-600 dark:text-gray-400">
              {{ result.testDate | dateFormat: 'short' }}
            </td>
          </tr>
        </tbody>
      </table>

      <div *ngIf="results.length === 0" class="text-center py-8">
        <p class="text-gray-500 dark:text-gray-400">No lab results available</p>
      </div>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LabResultsSummaryComponent {
  @Input() results: LabResult[] = [];

  getValueClasses(status: string): Record<string, boolean> {
    return {
      'text-green-600 dark:text-green-400': status === 'normal',
      'text-orange-600 dark:text-orange-400': status === 'abnormal',
      'text-red-600 dark:text-red-400': status === 'critical',
    };
  }

  getStatusBadgeClasses(status: string): Record<string, boolean> {
    return {
      'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-200':
        status === 'normal',
      'bg-orange-100 dark:bg-orange-900/30 text-orange-800 dark:text-orange-200':
        status === 'abnormal',
      'bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-200':
        status === 'critical',
    };
  }

  getTrendIcon(result: LabResult): string {
    if (!result.previousValue) return '';
    return result.value > result.previousValue ? '📈' : result.value < result.previousValue ? '📉' : '➡️';
  }

  getTrendPercent(result: LabResult): number {
    if (!result.previousValue || result.previousValue === 0) return 0;
    return Math.round(((result.value - result.previousValue) / result.previousValue) * 100);
  }

  getTrendClasses(result: LabResult): Record<string, boolean> {
    const change = result.value - (result.previousValue || 0);
    return {
      'text-red-600 dark:text-red-400': change > 0,
      'text-green-600 dark:text-green-400': change < 0,
      'text-gray-600 dark:text-gray-400': change === 0,
    };
  }
}
