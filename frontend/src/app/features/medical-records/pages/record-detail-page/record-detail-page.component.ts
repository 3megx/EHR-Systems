import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../../../../shared/components/ui/card/card.component';

/**
 * record-detail-page Component
 * Page for record-detail-page
 */
@Component({
  selector: 'app-r-ec-or-d-d-et-ai-l-p-ag-e',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: 
    <app-card title="record-detail-page">
      <div class="text-center py-12">
        <p class="text-gray-600 dark:text-gray-400">
          record-detail-page page - Implementation in progress
        </p>
      </div>
    </app-card>
  ,
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class record-detail-pageComponent implements OnInit {
  ngOnInit(): void {
    // Initialize component
  }
}
