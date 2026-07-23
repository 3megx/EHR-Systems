import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

/**
 * Print Layout Component
 * Layout for printable documents (reports, prescriptions, etc.)
 */
@Component({
  selector: 'app-print-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="print:bg-white bg-gray-50">
      <router-outlet></router-outlet>
    </div>
  `,
  styles: [
    `
      @media print {
        body {
          margin: 0;
          padding: 0;
        }
      }
    `,
  ],
})
export class PrintLayoutComponent {}
