import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

export interface NavItem {
  id: string;
  label: string;
  icon: string;
  route?: string;
  children?: NavItem[];
  badge?: number;
  expanded?: boolean;
}

/**
 * Sidebar Component
 * Navigation sidebar with collapsible menu items
 * Usage: <app-sidebar [navItems]="items" [collapsed]="isCollapsed" />
 */
@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <aside
      [class.w-64]="!collapsed"
      [class.w-20]="collapsed"
      class="h-screen bg-white dark:bg-gray-800 border-r border-gray-200 dark:border-gray-700 transition-all duration-300 overflow-y-auto"
    >
      <!-- Header -->
      <div class="p-4 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between">
          <h1 *ngIf="!collapsed" class="text-xl font-bold text-blue-600">EHR</h1>
          <button (click)="toggleCollapse()" class="p-1 hover:bg-gray-100 dark:hover:bg-gray-700 rounded">
            {{ collapsed ? '→' : '←' }}
          </button>
        </div>
      </div>

      <!-- Navigation Items -->
      <nav class="p-2">
        <ng-container *ngFor="let item of navItems">
          <div>
            <!-- Main Item -->
            <button
              [routerLink]="item.route"
              (click)="toggleItem(item)"
              class="w-full flex items-center justify-between px-4 py-3 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 text-gray-700 dark:text-gray-300 transition-colors group"
            >
              <div class="flex items-center gap-3 min-w-0">
                <span class="text-lg">{{ item.icon }}</span>
                <span *ngIf="!collapsed" class="truncate">{{ item.label }}</span>
              </div>
              <div class="flex items-center gap-2">
                <span
                  *ngIf="item.badge && !collapsed"
                  class="px-2 py-1 text-xs font-semibold text-white bg-red-600 rounded-full"
                >
                  {{ item.badge }}
                </span>
                <span *ngIf="item.children && !collapsed" [class.rotate-180]="item.expanded">▼</span>
              </div>
            </button>

            <!-- Sub Items -->
            <div *ngIf="item.children && item.expanded && !collapsed" class="ml-6 mt-1 border-l-2 border-gray-200 dark:border-gray-700">
              <button
                *ngFor="let child of item.children"
                [routerLink]="child.route"
                class="w-full text-left px-4 py-2 text-sm text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white hover:bg-gray-50 dark:hover:bg-gray-700/50 rounded transition-colors"
              >
                {{ child.label }}
              </button>
            </div>
          </div>
        </ng-container>
      </nav>
    </aside>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SidebarComponent {
  @Input() navItems: NavItem[] = [];
  @Input() collapsed = false;

  @Output() collapsedChange = new EventEmitter<boolean>();

  toggleCollapse(): void {
    this.collapsed = !this.collapsed;
    this.collapsedChange.emit(this.collapsed);
  }

  toggleItem(item: NavItem): void {
    if (item.children) {
      item.expanded = !item.expanded;
    }
  }
}
