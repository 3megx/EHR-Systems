import { Injectable } from '@angular/core';
import { Router, CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

/**
 * Role Guard
 * Protects routes based on user roles
 */
@Injectable({
  providedIn: 'root',
})
export class RoleGuard {
  constructor(
    private authService: AuthService,
    private router: Router,
    private notificationService: NotificationService
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    const requiredRoles = route.data['roles'] as string[];

    if (!requiredRoles || requiredRoles.length === 0) {
      return true;
    }

    const user = this.authService.getCurrentUser();
    if (!user) {
      this.router.navigate(['/auth/login']);
      return false;
    }

    const hasRole = requiredRoles.some((role) =>
      user.roles.some((userRole) => userRole.name === role)
    );

    if (hasRole) {
      return true;
    }

    // User doesn't have required role
    this.notificationService.error(
      'Access Denied',
      'You do not have permission to access this resource.'
    );
    this.router.navigate(['/dashboard']);
    return false;
  }
}

/**
 * Role Guard Function
 */
export const roleGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
): boolean => {
  const guard = new RoleGuard(
    new (require('../services/auth.service').AuthService as any)(),
    new (require('@angular/router').Router as any)(),
    new (require('../services/notification.service').NotificationService as any)()
  );
  return guard.canActivate(route, state);
};
