import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface Tab {
  id: string;
  label: string;
  disabled?: boolean;
}

/**
 * Tabs Component
 * Reusable tab component
 * Usage: <app-tabs [tabs]="tabList" [activeTab]="'tab1'" (tabChange)="onTabChange($event)">...</app-tabs>
 */
@Component({
  selector: 'app-tabs',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div>
      <div class="border-b border-gray-200 dark:border-gray-700">
        <div class="flex gap-4 overflow-x-auto">
          <button
            *ngFor="let tab of tabs"
            (click)="selectTab(tab.id)"
            [disabled]="tab.disabled"
            [class.border-b-2]="activeTab === tab.id"
            [class.border-blue-600]="activeTab === tab.id"
            [class.text-blue-600]="activeTab === tab.id"
            class="px-4 py-3 font-medium text-gray-700 dark:text-gray-300 border-b-2 border-transparent hover:text-gray-900 dark:hover:text-white disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            {{ tab.label }}
          </button>
        </div>
      </div>

      <div class="mt-4">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TabsComponent {
  @Input() tabs: Tab[] = [];
  @Input() activeTab = '';

  @Output() tabChange = new EventEmitter<string>();

  selectTab(tabId: string): void {
    const tab = this.tabs.find((t) => t.id === tabId);
    if (tab && !tab.disabled) {
      this.activeTab = tabId;
      this.tabChange.emit(tabId);
    }
  }
}
