import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

/**
 * Main Layout Component
 * Primary layout for authenticated users
 */
@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="flex h-screen bg-gray-50 dark:bg-gray-900">
      <!-- Sidebar -->
      <aside class="hidden md:flex w-64 bg-white dark:bg-gray-800 border-r border-gray-200 dark:border-gray-700">
        <div class="w-full p-4">
          <h1 class="text-xl font-bold text-blue-600 mb-8">EHR Platform</h1>
          <nav class="space-y-2">
            <!-- Navigation items will go here -->
          </nav>
        </div>
      </aside>

      <!-- Main Content -->
      <main class="flex-1 flex flex-col overflow-hidden">
        <!-- Topbar -->
        <header class="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 px-6 py-4">
          <div class="flex items-center justify-between">
            <h2 class="text-2xl font-semibold text-gray-900 dark:text-white">Dashboard</h2>
            <div class="flex items-center gap-4">
              <!-- User menu will go here -->
            </div>
          </div>
        </header>

        <!-- Page Content -->
        <div class="flex-1 overflow-auto">
          <div class="p-6">
            <router-outlet></router-outlet>
          </div>
        </div>
      </main>
    </div>
  `,
  styles: [],
})
export class MainLayoutComponent implements OnInit {
  ngOnInit(): void {
    // Initialize layout
  }
}
