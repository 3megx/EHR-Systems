import { Routes } from '@angular/router';
import { AdminDashboardPageComponent } from './pages/admin-dashboard-page/admin-dashboard-page.component';
import { UserManagementPageComponent } from './pages/user-management-page/user-management-page.component';
import { RoleManagementPageComponent } from './pages/role-management-page/role-management-page.component';
import { SettingsPageComponent } from './pages/settings-page/settings-page.component';
import { AuditLogsPageComponent } from './pages/audit-logs-page/audit-logs-page.component';

/**
 * Admin Feature Routes
 */
export const adminRoutes: Routes = [
  {
    path: '',
    component: AdminDashboardPageComponent,
    data: { title: 'Administration', breadcrumb: 'Admin' },
  },
  {
    path: 'users',
    component: UserManagementPageComponent,
    data: { title: 'User Management', breadcrumb: 'Users' },
  },
  {
    path: 'roles',
    component: RoleManagementPageComponent,
    data: { title: 'Role Management', breadcrumb: 'Roles' },
  },
  {
    path: 'settings',
    component: SettingsPageComponent,
    data: { title: 'System Settings', breadcrumb: 'Settings' },
  },
  {
    path: 'audit-logs',
    component: AuditLogsPageComponent,
    data: { title: 'Audit Logs', breadcrumb: 'Audit Logs' },
  },
];
