import { Routes } from '@angular/router';
import { MedicalRecordsPageComponent } from './pages/medical-records-page/medical-records-page.component';
import { RecordDetailPageComponent } from './pages/record-detail-page/record-detail-page.component';

/**
 * Medical Records Feature Routes
 */
export const medicalRecordsRoutes: Routes = [
  {
    path: '',
    component: MedicalRecordsPageComponent,
    data: { title: 'Medical Records', breadcrumb: 'Medical Records' },
  },
  {
    path: ':id',
    component: RecordDetailPageComponent,
    data: { title: 'Record Details', breadcrumb: 'Details' },
  },
];
