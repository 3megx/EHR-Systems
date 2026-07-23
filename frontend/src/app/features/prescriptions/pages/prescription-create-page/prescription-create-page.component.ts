import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../../../../shared/components/ui/card/card.component';

/**
 * prescription-create-page Component
 * Page for prescription-create-page
 */
@Component({
  selector: 'app-p-re-sc-ri-pt-io-n-c-re-at-e-p-ag-e',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: 
    <app-card title="prescription-create-page">
      <div class="text-center py-12">
        <p class="text-gray-600 dark:text-gray-400">
          prescription-create-page page - Implementation in progress
        </p>
      </div>
    </app-card>
  ,
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class prescription-create-pageComponent implements OnInit {
  ngOnInit(): void {
    // Initialize component
  }
}
