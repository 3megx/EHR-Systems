import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../core/services/auth.service';
import { CardComponent } from '../../../../shared/components/ui/card/card.component';

/**
 * Dashboard Page Component
 * Main dashboard for authenticated users
 */
@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: `
    <div class="space-y-6">
      <!-- Welcome Header -->
      <div>
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Welcome back!</h1>
        <p class="text-gray-600 dark:text-gray-400">
          {{ currentUser?.firstName }} {{ currentUser?.lastName }}
        </p>
      </div>

      <!-- Dashboard Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <app-card title="Total Patients">
          <div class="text-3xl font-bold text-blue-600">1,234</div>
          <p class="text-sm text-gray-600 dark:text-gray-400">Active patients</p>
        </app-card>

        <app-card title="Appointments Today">
          <div class="text-3xl font-bold text-green-600">12</div>
          <p class="text-sm text-gray-600 dark:text-gray-400">Scheduled</p>
        </app-card>

        <app-card title="Pending Orders">
          <div class="text-3xl font-bold text-yellow-600">8</div>
          <p class="text-sm text-gray-600 dark:text-gray-400">Lab & imaging</p>
        </app-card>

        <app-card title="Pending Approvals">
          <div class="text-3xl font-bold text-red-600">3</div>
          <p class="text-sm text-gray-600 dark:text-gray-400">Prescriptions</p>
        </app-card>
      </div>

      <!-- Recent Activity -->
      <app-card title="Recent Activity">
        <div class="space-y-2">
          <p class="text-gray-600 dark:text-gray-400">
            Dashboard coming soon - Feature implementation in progress
          </p>
        </div>
      </app-card>
    </div>
  `,
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardPageComponent implements OnInit {
  currentUser = this.authService.getCurrentUser();

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    // Load dashboard data
  }
}
