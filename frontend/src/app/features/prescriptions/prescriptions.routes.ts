import { Routes } from '@angular/router';
import { PrescriptionListPageComponent } from './pages/prescription-list-page/prescription-list-page.component';
import { PrescriptionCreatePageComponent } from './pages/prescription-create-page/prescription-create-page.component';
import { PrescriptionDetailPageComponent } from './pages/prescription-detail-page/prescription-detail-page.component';

/**
 * Prescriptions Feature Routes
 */
export const prescriptionsRoutes: Routes = [
  {
    path: '',
    component: PrescriptionListPageComponent,
    data: { title: 'Prescriptions', breadcrumb: 'Prescriptions' },
  },
  {
    path: 'new',
    component: PrescriptionCreatePageComponent,
    data: { title: 'New Prescription', breadcrumb: 'New' },
  },
  {
    path: ':id',
    component: PrescriptionDetailPageComponent,
    data: { title: 'Prescription Details', breadcrumb: 'Details' },
  },
];
