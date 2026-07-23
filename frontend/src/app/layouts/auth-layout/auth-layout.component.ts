import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

/**
 * Auth Layout Component
 * Layout for login, register, and password reset pages
 */
@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800 px-4">
      <div class="w-full max-w-md">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-8">
          <div class="text-center mb-8">
            <h1 class="text-3xl font-bold text-blue-600 mb-2">EHR Platform</h1>
            <p class="text-gray-600 dark:text-gray-400">Modern Healthcare Management</p>
          </div>

          <router-outlet></router-outlet>
        </div>
      </div>
    </div>
  `,
  styles: [],
})
export class AuthLayoutComponent {}
