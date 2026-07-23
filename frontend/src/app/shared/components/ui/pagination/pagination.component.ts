import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Pagination Component
 * Reusable pagination controls
 * Usage: <app-pagination [currentPage]="page" [totalPages]="10" (pageChange)="onPageChange($event)" />
 */
@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="flex items-center justify-between">
      <div class="text-sm text-gray-600 dark:text-gray-400">
        Page {{ currentPage }} of {{ totalPages }}
      </div>

      <div class="flex gap-2">
        <button
          (click)="previousPage()"
          [disabled]="currentPage === 1"
          class="px-3 py-2 rounded-lg border border-gray-300 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          ← Previous
        </button>

        <div class="flex gap-1">
          <button
            *ngFor="let page of getPageNumbers()"
            (click)="goToPage(page)"
            [class.bg-blue-600]="page === currentPage"
            [class.text-white]="page === currentPage"
            [class.hover:bg-gray-50]="page !== currentPage"
            class="px-3 py-2 rounded-lg border border-gray-300 dark:border-gray-600"
          >
            {{ page }}
          </button>
        </div>

        <button
          (click)="nextPage()"
          [disabled]="currentPage === totalPages"
          class="px-3 py-2 rounded-lg border border-gray-300 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Next →
        </button>
      </div>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PaginationComponent {
  @Input() currentPage = 1;
  @Input() totalPages = 1;
  @Input() maxButtons = 5;

  @Output() pageChange = new EventEmitter<number>();

  previousPage(): void {
    if (this.currentPage > 1) {
      this.goToPage(this.currentPage - 1);
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.goToPage(this.currentPage + 1);
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.pageChange.emit(page);
    }
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const half = Math.floor(this.maxButtons / 2);

    let start = Math.max(1, this.currentPage - half);
    let end = Math.min(this.totalPages, this.currentPage + half);

    if (end - start < this.maxButtons - 1) {
      if (start === 1) {
        end = Math.min(this.totalPages, start + this.maxButtons - 1);
      } else {
        start = Math.max(1, end - this.maxButtons + 1);
      }
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    return pages;
  }
}
