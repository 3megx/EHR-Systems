import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../../../../shared/components/ui/card/card.component';

/**
 * medical-records-page Component
 * Page for medical-records-page
 */
@Component({
  selector: 'app-m-ed-ic-al-r-ec-or-ds-p-ag-e',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: 
    <app-card title="medical-records-page">
      <div class="text-center py-12">
        <p class="text-gray-600 dark:text-gray-400">
          medical-records-page page - Implementation in progress
        </p>
      </div>
    </app-card>
  ,
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class medical-records-pageComponent implements OnInit {
  ngOnInit(): void {
    // Initialize component
  }
}
