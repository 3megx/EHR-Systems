import { Routes } from '@angular/router';
import { PatientListPageComponent } from './pages/patient-list-page/patient-list-page.component';
import { PatientSearchPageComponent } from './pages/patient-search-page/patient-search-page.component';
import { PatientDetailPageComponent } from './pages/patient-detail-page/patient-detail-page.component';
import { PatientTimelinePageComponent } from './pages/patient-timeline-page/patient-timeline-page.component';

/**
 * Patients Feature Routes
 */
export const patientsRoutes: Routes = [
  {
    path: '',
    component: PatientListPageComponent,
    data: { title: 'Patients', breadcrumb: 'Patients' },
  },
  {
    path: 'search',
    component: PatientSearchPageComponent,
    data: { title: 'Patient Search', breadcrumb: 'Search' },
  },
  {
    path: ':id',
    component: PatientDetailPageComponent,
    data: { title: 'Patient Details', breadcrumb: 'Details' },
  },
  {
    path: ':id/timeline',
    component: PatientTimelinePageComponent,
    data: { title: 'Patient Timeline', breadcrumb: 'Timeline' },
  },
];
