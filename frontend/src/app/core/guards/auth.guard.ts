import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Auth Guard
 * Protects routes that require authentication
 */
export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
): boolean => {
  const authService = new (require('../services/auth.service').AuthService as any)();
  const router = new (require('@angular/router').Router as any)();

  if (authService.isAuthenticated()) {
    return true;
  }

  // Navigate to login
  router.navigate(['/auth/login'], {
    queryParams: { returnUrl: state.url },
  });
  return false;
};

/**
 * Auth Guard (Injectable)
 * Alternative implementation for dependency injection
 */
@Injectable({
  providedIn: 'root',
})
export class AuthGuard {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    if (this.authService.isAuthenticated()) {
      return true;
    }

    this.router.navigate(['/auth/login'], {
      queryParams: { returnUrl: state.url },
    });
    return false;
  }
}
