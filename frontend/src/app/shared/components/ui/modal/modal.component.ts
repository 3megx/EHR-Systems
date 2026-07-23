import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { trigger, transition, style, animate } from '@angular/animations';

/**
 * Modal Component
 * Reusable modal/dialog component
 * Usage: <app-modal [(open)]="isOpen" title="Confirm" (confirm)="onConfirm()">...</app-modal>
 */
@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      *ngIf="open"
      @fadeInOut
      class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50"
      (click)="onBackdropClick()"
    >
      <div
        @slideIn
        class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full mx-4"
        (click)="$event.stopPropagation()"
      >
        <!-- Header -->
        <div class="flex items-center justify-between p-6 border-b border-gray-200 dark:border-gray-700">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white">{{ title }}</h2>
          <button
            (click)="onClose()"
            class="text-gray-500 hover:text-gray-700 dark:hover:text-gray-300"
          >
            ✕
          </button>
        </div>

        <!-- Content -->
        <div class="p-6">
          <ng-content></ng-content>
        </div>

        <!-- Footer -->
        <div class="flex justify-end gap-3 p-6 border-t border-gray-200 dark:border-gray-700">
          <button
            (click)="onClose()"
            class="px-4 py-2 text-gray-700 dark:text-gray-300 bg-gray-100 dark:bg-gray-700 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-600"
          >
            {{ cancelLabel }}
          </button>
          <button
            (click)="onConfirm()"
            class="px-4 py-2 text-white bg-blue-600 rounded-lg hover:bg-blue-700"
          >
            {{ confirmLabel }}
          </button>
        </div>
      </div>
    </div>
  `,
  animations: [
    trigger('fadeInOut', [
      transition(':enter', [
        style({ opacity: 0 }),
        animate('200ms ease-in', style({ opacity: 1 })),
      ]),
      transition(':leave', [animate('200ms ease-out', style({ opacity: 0 }))]),
    ]),
    trigger('slideIn', [
      transition(':enter', [
        style({ transform: 'translateY(-20px)', opacity: 0 }),
        animate('300ms ease-out', style({ transform: 'translateY(0)', opacity: 1 })),
      ]),
    ]),
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ModalComponent {
  @Input() open = false;
  @Input() title = 'Modal';
  @Input() confirmLabel = 'Confirm';
  @Input() cancelLabel = 'Cancel';
  @Input() closeOnBackdrop = true;

  @Output() openChange = new EventEmitter<boolean>();
  @Output() confirm = new EventEmitter<void>();
  @Output() close = new EventEmitter<void>();

  onConfirm(): void {
    this.confirm.emit();
    this.setOpen(false);
  }

  onClose(): void {
    this.close.emit();
    this.setOpen(false);
  }

  onBackdropClick(): void {
    if (this.closeOnBackdrop) {
      this.onClose();
    }
  }

  private setOpen(value: boolean): void {
    this.open = value;
    this.openChange.emit(value);
  }

  @HostListener('keydown.escape')
  onEscapeKey(): void {
    if (this.closeOnBackdrop) {
      this.onClose();
    }
  }
}
