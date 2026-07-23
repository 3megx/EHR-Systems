import { Routes } from '@angular/router';
import { LabResultsPageComponent } from './pages/lab-results-page/lab-results-page.component';
import { LabResultDetailPageComponent } from './pages/lab-result-detail-page/lab-result-detail-page.component';

/**
 * Lab Results Feature Routes
 */
export const labResultsRoutes: Routes = [
  {
    path: '',
    component: LabResultsPageComponent,
    data: { title: 'Lab Results', breadcrumb: 'Lab Results' },
  },
  {
    path: ':id',
    component: LabResultDetailPageComponent,
    data: { title: 'Lab Result Details', breadcrumb: 'Details' },
  },
];
