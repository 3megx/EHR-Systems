import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../../../../shared/components/ui/card/card.component';

/**
 * prescription-list-page Component
 * Page for prescription-list-page
 */
@Component({
  selector: 'app-p-re-sc-ri-pt-io-n-l-is-t-p-ag-e',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: 
    <app-card title="prescription-list-page">
      <div class="text-center py-12">
        <p class="text-gray-600 dark:text-gray-400">
          prescription-list-page page - Implementation in progress
        </p>
      </div>
    </app-card>
  ,
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class prescription-list-pageComponent implements OnInit {
  ngOnInit(): void {
    // Initialize component
  }
}
