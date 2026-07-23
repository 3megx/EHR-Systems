import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Patient } from '../../../core/models';

/**
 * Patient Header Component
 * Sticky header displaying patient key information
 * Usage: <app-patient-header [patient]="currentPatient" />
 */
@Component({
  selector: 'app-patient-header',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      *ngIf="patient"
      class="sticky top-0 z-40 bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 px-6 py-4 shadow-sm"
    >
      <div class="flex items-center justify-between gap-6">
        <!-- Patient Info -->
        <div class="flex items-center gap-4 flex-1">
          <!-- Avatar -->
          <div class="w-12 h-12 rounded-full bg-blue-600 flex items-center justify-center text-white text-lg font-semibold">
            {{ getInitials() }}
          </div>

          <!-- Details -->
          <div>
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
              {{ patient.firstName }} {{ patient.lastName }}
            </h3>
            <div class="flex items-center gap-4 text-sm text-gray-600 dark:text-gray-400">
              <span>MRN: <span class="font-medium">{{ patient.mrn }}</span></span>
              <span>DOB: <span class="font-medium">{{ patient.dateOfBirth | dateFormat: 'medium' }}</span></span>
              <span>Age: <span class="font-medium">{{ getAge() }}</span></span>
              <span *ngIf="patient.gender" class="capitalize">{{ patient.gender }}</span>
            </div>
          </div>
        </div>

        <!-- Quick Info -->
        <div class="hidden lg:grid grid-cols-3 gap-4">
          <!-- Allergies -->
          <div class="text-center">
            <div class="text-2xl font-bold text-red-600">{{ patient.allergies?.length || 0 }}</div>
            <div class="text-xs text-gray-600 dark:text-gray-400">Allergies</div>
          </div>

          <!-- Conditions -->
          <div class="text-center">
            <div class="text-2xl font-bold text-orange-600">{{ patient.chronicConditions?.length || 0 }}</div>
            <div class="text-xs text-gray-600 dark:text-gray-400">Conditions</div>
          </div>

          <!-- Contact -->
          <div class="text-center">
            <a
              *ngIf="patient.phone"
              [href]="'tel:' + patient.phone"
              class="text-2xl font-bold text-green-600 hover:text-green-700"
              title="Call patient"
            >
              📞
            </a>
            <div class="text-xs text-gray-600 dark:text-gray-400">{{ patient.phone || 'No phone' }}</div>
          </div>
        </div>

        <!-- Actions -->
        <div class="flex items-center gap-2">
          <button class="px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition-colors">
            Edit
          </button>
          <button class="p-2 hover:bg-gray-100 dark:hover:bg-gray-700 rounded-lg transition-colors">
            ⋮
          </button>
        </div>
      </div>

      <!-- Alerts/Warnings -->
      <div *ngIf="hasAlerts()" class="mt-4 grid grid-cols-1 md:grid-cols-2 gap-3">
        <div
          *ngIf="patient.allergies && patient.allergies.length > 0"
          class="flex items-center gap-2 p-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg"
        >
          <span class="text-lg">⚠️</span>
          <span class="text-sm text-red-800 dark:text-red-200">
            {{ patient.allergies.length }} known allergie(s)
          </span>
        </div>

        <div
          *ngIf="patient.chronicConditions && patient.chronicConditions.length > 0"
          class="flex items-center gap-2 p-3 bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded-lg"
        >
          <span class="text-lg">📋</span>
          <span class="text-sm text-yellow-800 dark:text-yellow-200">
            {{ patient.chronicConditions.length }} chronic condition(s)
          </span>
        </div>
      </div>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PatientHeaderComponent {
  @Input() patient: Patient | null = null;

  getInitials(): string {
    if (!this.patient) return '';
    return `${this.patient.firstName?.charAt(0) || ''}${this.patient.lastName?.charAt(0) || ''}`.toUpperCase();
  }

  getAge(): number {
    if (!this.patient?.dateOfBirth) return 0;
    const today = new Date();
    const birthDate = new Date(this.patient.dateOfBirth);
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();

    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }

    return age;
  }

  hasAlerts(): boolean {
    return !!(
      (this.patient?.allergies && this.patient.allergies.length > 0) ||
      (this.patient?.chronicConditions && this.patient.chronicConditions.length > 0)
    );
  }
}
