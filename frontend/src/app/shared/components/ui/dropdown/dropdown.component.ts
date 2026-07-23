import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { trigger, transition, style, animate } from '@angular/animations';

export interface DropdownOption {
  id: string | number;
  label: string;
  icon?: string;
  divider?: boolean;
}

/**
 * Dropdown Component
 * Reusable dropdown menu
 * Usage: <app-dropdown [options]="optionsList" (select)="onSelect($event)">Dropdown</app-dropdown>
 */
@Component({
  selector: 'app-dropdown',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="relative inline-block">
      <button
        (click)="toggleOpen()"
        class="px-4 py-2 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 flex items-center gap-2"
      >
        <ng-content></ng-content>
        <span>▼</span>
      </button>

      <div
        *ngIf="isOpen"
        @dropdownAnimation
        class="absolute top-full left-0 mt-2 w-48 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-lg shadow-lg z-50"
      >
        <div class="py-2">
          <button
            *ngFor="let option of options"
            *ngIf="!option.divider"
            (click)="selectOption(option)"
            class="w-full px-4 py-2 text-left hover:bg-gray-100 dark:hover:bg-gray-700 text-gray-700 dark:text-gray-300 flex items-center gap-2"
          >
            <span *ngIf="option.icon">{{ option.icon }}</span>
            <span>{{ option.label }}</span>
          </button>
          <div *ngIf="option.divider" class="border-t border-gray-200 dark:border-gray-700 my-2"></div>
        </div>
      </div>
    </div>
  `,
  animations: [
    trigger('dropdownAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(-10px)' }),
        animate('200ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
      ]),
    ]),
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DropdownComponent {
  @Input() options: DropdownOption[] = [];

  @Output() select = new EventEmitter<DropdownOption>();

  isOpen = false;

  toggleOpen(): void {
    this.isOpen = !this.isOpen;
  }

  selectOption(option: DropdownOption): void {
    this.select.emit(option);
    this.isOpen = false;
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('[appDropdown]')) {
      this.isOpen = false;
    }
  }
}
