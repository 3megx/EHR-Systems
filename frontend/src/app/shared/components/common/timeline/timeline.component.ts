import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface TimelineEvent {
  id: string;
  title: string;
  description?: string;
  timestamp: Date;
  icon?: string;
  color?: 'primary' | 'success' | 'warning' | 'danger' | 'info';
  details?: string;
}

/**
 * Timeline Component
 * Displays medical history timeline
 * Usage: <app-timeline [events]="medicalHistory" />
 */
@Component({
  selector: 'app-timeline',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="relative">
      <!-- Timeline Items -->
      <div *ngFor="let event of events; let last = last" class="flex gap-4 pb-8" [class.pb-0]="last">
        <!-- Timeline Node -->
        <div class="flex flex-col items-center">
          <!-- Connector -->
          <div
            class="w-4 h-4 rounded-full border-4 flex items-center justify-center"
            [ngClass]="getColorClasses(event.color)"
          >
            <span *ngIf="event.icon" class="text-xs">{{ event.icon }}</span>
          </div>
          <!-- Line -->
          <div
            *ngIf="!last"
            class="w-1 flex-1 mt-4 bg-gray-300 dark:bg-gray-600"
          ></div>
        </div>

        <!-- Content -->
        <div class="flex-1 pt-1">
          <div class="flex items-start justify-between gap-4">
            <div>
              <h4 class="font-semibold text-gray-900 dark:text-white">{{ event.title }}</h4>
              <p *ngIf="event.description" class="text-sm text-gray-600 dark:text-gray-400">
                {{ event.description }}
              </p>
            </div>
            <span class="text-xs text-gray-500 dark:text-gray-500 whitespace-nowrap">
              {{ event.timestamp | dateFormat: 'medium' }}
            </span>
          </div>

          <!-- Details -->
          <div *ngIf="event.details" class="mt-2 p-3 bg-gray-50 dark:bg-gray-700/50 rounded text-sm text-gray-600 dark:text-gray-400">
            {{ event.details }}
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div *ngIf="events.length === 0" class="text-center py-8">
        <p class="text-gray-500 dark:text-gray-400">No events to display</p>
      </div>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TimelineComponent {
  @Input() events: TimelineEvent[] = [];

  getColorClasses(color?: string): Record<string, boolean> {
    const baseClasses = {
      'border-blue-600 bg-blue-100': color === 'primary' || !color,
      'border-green-600 bg-green-100': color === 'success',
      'border-yellow-600 bg-yellow-100': color === 'warning',
      'border-red-600 bg-red-100': color === 'danger',
      'border-cyan-600 bg-cyan-100': color === 'info',
    };
    return baseClasses;
  }
}
