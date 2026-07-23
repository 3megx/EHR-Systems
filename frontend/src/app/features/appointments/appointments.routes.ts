import { Routes } from '@angular/router';
import { AppointmentListPageComponent } from './pages/appointment-list-page/appointment-list-page.component';
import { AppointmentSchedulePageComponent } from './pages/appointment-schedule-page/appointment-schedule-page.component';
import { AppointmentDetailPageComponent } from './pages/appointment-detail-page/appointment-detail-page.component';

/**
 * Appointments Feature Routes
 */
export const appointmentsRoutes: Routes = [
  {
    path: '',
    component: AppointmentListPageComponent,
    data: { title: 'Appointments', breadcrumb: 'Appointments' },
  },
  {
    path: 'schedule',
    component: AppointmentSchedulePageComponent,
    data: { title: 'Schedule Appointment', breadcrumb: 'Schedule' },
  },
  {
    path: ':id',
    component: AppointmentDetailPageComponent,
    data: { title: 'Appointment Details', breadcrumb: 'Details' },
  },
];
