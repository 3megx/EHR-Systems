import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../../../../shared/components/ui/card/card.component';

/**
 * appointment-schedule-page Component
 * Page for appointment-schedule-page
 */
@Component({
  selector: 'app-a-pp-oi-nt-me-nt-s-ch-ed-ul-e-p-ag-e',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: 
    <app-card title="appointment-schedule-page">
      <div class="text-center py-12">
        <p class="text-gray-600 dark:text-gray-400">
          appointment-schedule-page page - Implementation in progress
        </p>
      </div>
    </app-card>
  ,
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class appointment-schedule-pageComponent implements OnInit {
  ngOnInit(): void {
    // Initialize component
  }
}
