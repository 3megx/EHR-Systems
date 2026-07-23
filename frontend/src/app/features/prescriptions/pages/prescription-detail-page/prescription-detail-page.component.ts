import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../../../../shared/components/ui/card/card.component';

/**
 * prescription-detail-page Component
 * Page for prescription-detail-page
 */
@Component({
  selector: 'app-p-re-sc-ri-pt-io-n-d-et-ai-l-p-ag-e',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: 
    <app-card title="prescription-detail-page">
      <div class="text-center py-12">
        <p class="text-gray-600 dark:text-gray-400">
          prescription-detail-page page - Implementation in progress
        </p>
      </div>
    </app-card>
  ,
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class prescription-detail-pageComponent implements OnInit {
  ngOnInit(): void {
    // Initialize component
  }
}
