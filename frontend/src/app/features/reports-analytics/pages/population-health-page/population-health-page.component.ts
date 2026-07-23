import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../../../../shared/components/ui/card/card.component';

/**
 * population-health-page Component
 * Page for population-health-page
 */
@Component({
  selector: 'app-p-op-ul-at-io-n-h-ea-lt-h-p-ag-e',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: 
    <app-card title="population-health-page">
      <div class="text-center py-12">
        <p class="text-gray-600 dark:text-gray-400">
          population-health-page page - Implementation in progress
        </p>
      </div>
    </app-card>
  ,
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class population-health-pageComponent implements OnInit {
  ngOnInit(): void {
    // Initialize component
  }
}
