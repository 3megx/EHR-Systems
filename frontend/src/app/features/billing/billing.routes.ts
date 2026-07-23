import { Routes } from '@angular/router';
import { BillingPageComponent } from './pages/billing-page/billing-page.component';
import { InvoiceListPageComponent } from './pages/invoice-list-page/invoice-list-page.component';

/**
 * Billing Feature Routes
 */
export const billingRoutes: Routes = [
  {
    path: '',
    component: BillingPageComponent,
    data: { title: 'Billing & Claims', breadcrumb: 'Billing' },
  },
  {
    path: 'invoices',
    component: InvoiceListPageComponent,
    data: { title: 'Invoices', breadcrumb: 'Invoices' },
  },
];
