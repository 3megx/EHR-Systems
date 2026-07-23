import { Routes } from '@angular/router';
import { ReportsPageComponent } from './pages/reports-page/reports-page.component';
import { PopulationHealthPageComponent } from './pages/population-health-page/population-health-page.component';
import { CompliancePageComponent } from './pages/compliance-page/compliance-page.component';

/**
 * Reports & Analytics Feature Routes
 */
export const reportsAnalyticsRoutes: Routes = [
  {
    path: '',
    component: ReportsPageComponent,
    data: { title: 'Reports & Analytics', breadcrumb: 'Reports' },
  },
  {
    path: 'population-health',
    component: PopulationHealthPageComponent,
    data: { title: 'Population Health', breadcrumb: 'Population Health' },
  },
  {
    path: 'compliance',
    component: CompliancePageComponent,
    data: { title: 'Compliance Reports', breadcrumb: 'Compliance' },
  },
];
