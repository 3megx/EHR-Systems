import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface TableColumn {
  key: string;
  label: string;
  sortable?: boolean;
  width?: string;
}

export interface SortEvent {
  column: string;
  direction: 'asc' | 'desc';
}

/**
 * Table Component
 * Reusable data table with sorting
 * Usage: <app-table [columns]="cols" [data]="rows" />
 */
@Component({
  selector: 'app-table',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="overflow-x-auto">
      <table class="w-full border-collapse">
        <thead>
          <tr class="bg-gray-100 dark:bg-gray-700 border-b border-gray-200 dark:border-gray-600">
            <th
              *ngFor="let col of columns"
              [style.width]="col.width || 'auto'"
              class="px-6 py-3 text-left text-sm font-semibold text-gray-900 dark:text-white"
            >
              <button
                *ngIf="col.sortable"
                (click)="onSort(col.key)"
                class="flex items-center gap-2 hover:text-blue-600"
              >
                {{ col.label }}
                <span *ngIf="sortColumn === col.key">
                  {{ sortDirection === 'asc' ? '▲' : '▼' }}
                </span>
              </button>
              <span *ngIf="!col.sortable">{{ col.label }}</span>
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            *ngFor="let row of data; let i = index"
            class="border-b border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700/50"
          >
            <td *ngFor="let col of columns" class="px-6 py-4 text-sm text-gray-700 dark:text-gray-300">
              {{ row[col.key] }}
            </td>
          </tr>
          <tr *ngIf="data.length === 0">
            <td [attr.colspan]="columns.length" class="px-6 py-8 text-center text-gray-500 dark:text-gray-400">
              No data available
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TableComponent {
  @Input() columns: TableColumn[] = [];
  @Input() data: any[] = [];

  @Output() sort = new EventEmitter<SortEvent>();

  sortColumn = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  onSort(column: string): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }
    this.sort.emit({ column: this.sortColumn, direction: this.sortDirection });
  }
}
