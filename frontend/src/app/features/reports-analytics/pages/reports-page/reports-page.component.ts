import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../../../../shared/components/ui/card/card.component';

/**
 * reports-page Component
 * Page for reports-page
 */
@Component({
  selector: 'app-r-ep-or-ts-p-ag-e',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: 
    <app-card title="reports-page">
      <div class="text-center py-12">
        <p class="text-gray-600 dark:text-gray-400">
          reports-page page - Implementation in progress
        </p>
      </div>
    </app-card>
  ,
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class reports-pageComponent implements OnInit {
  ngOnInit(): void {
    // Initialize component
  }
}
