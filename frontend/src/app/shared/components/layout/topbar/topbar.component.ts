import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

export interface TopbarAction {
  id: string;
  icon: string;
  label: string;
  badge?: number;
}

/**
 * Topbar Component
 * Top navigation bar with user menu and actions
 * Usage: <app-topbar [title]="pageTitle" [actions]="actionList" />
 */
@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <header class="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 px-6 py-4">
      <div class="flex items-center justify-between">
        <!-- Left Section -->
        <div class="flex items-center gap-4">
          <button (click)="toggleSidebar.emit()" class="p-2 hover:bg-gray-100 dark:hover:bg-gray-700 rounded-lg md:hidden">
            ☰
          </button>
          <h2 class="text-2xl font-semibold text-gray-900 dark:text-white">{{ title }}</h2>
        </div>

        <!-- Right Section -->
        <div class="flex items-center gap-6">
          <!-- Actions -->
          <div class="flex items-center gap-4">
            <button
              *ngFor="let action of actions"
              [title]="action.label"
              (click)="actionClick.emit(action)"
              class="relative p-2 hover:bg-gray-100 dark:hover:bg-gray-700 rounded-lg transition-colors text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white"
            >
              <span class="text-lg">{{ action.icon }}</span>
              <span
                *ngIf="action.badge"
                class="absolute top-0 right-0 w-5 h-5 flex items-center justify-center bg-red-600 text-white text-xs rounded-full"
              >
                {{ action.badge }}
              </span>
            </button>
          </div>

          <!-- Divider -->
          <div class="h-6 border-l border-gray-300 dark:border-gray-600"></div>

          <!-- User Menu -->
          <div class="relative">
            <button
              (click)="toggleUserMenu()"
              class="flex items-center gap-3 p-2 hover:bg-gray-100 dark:hover:bg-gray-700 rounded-lg transition-colors"
            >
              <img
                *ngIf="userAvatar"
                [src]="userAvatar"
                alt="User"
                class="w-8 h-8 rounded-full"
              />
              <div *ngIf="!userAvatar" class="w-8 h-8 rounded-full bg-blue-600 flex items-center justify-center text-white text-sm font-semibold">
                {{ userName?.charAt(0)?.toUpperCase() }}
              </div>
              <span class="hidden sm:block text-sm font-medium text-gray-700 dark:text-gray-300">{{ userName }}</span>
            </button>

            <!-- User Menu Dropdown -->
            <div
              *ngIf="userMenuOpen"
              class="absolute right-0 mt-2 w-48 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-lg shadow-lg z-50"
            >
              <div class="py-2">
                <a
                  href="/profile"
                  class="block px-4 py-2 text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
                >
                  👤 Profile
                </a>
                <a
                  href="/settings"
                  class="block px-4 py-2 text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
                >
                  ⚙️ Settings
                </a>
                <button
                  (click)="logout.emit()"
                  class="w-full text-left px-4 py-2 text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
                >
                  🚪 Logout
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </header>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TopbarComponent {
  @Input() title = '';
  @Input() actions: TopbarAction[] = [];
  @Input() userName = '';
  @Input() userAvatar = '';

  @Output() actionClick = new EventEmitter<TopbarAction>();
  @Output() toggleSidebar = new EventEmitter<void>();
  @Output() logout = new EventEmitter<void>();

  userMenuOpen = false;

  toggleUserMenu(): void {
    this.userMenuOpen = !this.userMenuOpen;
  }
}
