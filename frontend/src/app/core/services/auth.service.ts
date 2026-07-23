import { Injectable, computed, effect } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { User, LoginRequest, LoginResponse, AuthTokenResponse } from '../models';
import { environment } from '@env/environment';

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  loading: boolean;
  error: string | null;
  token: string | null;
}

/**
 * Auth Service with NgRx Signals
 * Manages authentication state and operations
 */
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = environment.apiUrl;
  private tokenKey = 'auth_token';
  private refreshTokenKey = 'refresh_token';
  private userKey = 'current_user';

  // Signals for reactive state management
  private authStore = signalStore(
    { providedIn: 'root' },
    withState<AuthState>({
      user: null,
      isAuthenticated: false,
      loading: false,
      error: null,
      token: null,
    }),
    withMethods((store) => ({
      setUser: (user: User) => patchState(store, { user, isAuthenticated: true }),
      setToken: (token: string) => patchState(store, { token }),
      setLoading: (loading: boolean) => patchState(store, { loading }),
      setError: (error: string | null) => patchState(store, { error }),
      clearAuth: () =>
        patchState(store, {
          user: null,
          isAuthenticated: false,
          token: null,
          error: null,
        }),
    }))
  );

  // Public state signals
  user$ = this.authStore.user;
  isAuthenticated$ = this.authStore.isAuthenticated;
  loading$ = this.authStore.loading;
  error$ = this.authStore.error;

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.initializeAuth();
  }

  /**
   * Initialize authentication from stored token
   */
  private initializeAuth(): void {
    const token = this.getStoredToken();
    if (token) {
      this.validateToken(token);
    }
  }

  /**
   * Login user
   */
  login(credentials: LoginRequest): Observable<LoginResponse> {
    this.authStore.setLoading(true);
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/auth/login`, credentials)
      .pipe(
        tap((response) => {
          this.setAuthTokens(response.token);
          this.authStore.setUser(response.user);
          this.authStore.setLoading(false);
          this.storeUser(response.user);
          this.router.navigate(['/dashboard']);
        }),
        catchError((error) => {
          this.authStore.setError(error.error?.message || 'Login failed');
          this.authStore.setLoading(false);
          throw error;
        })
      );
  }

  /**
   * Logout user
   */
  logout(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/auth/logout`, {}).pipe(
      tap(() => {
        this.clearAuthTokens();
        this.authStore.clearAuth();
        this.router.navigate(['/auth/login']);
      }),
      catchError(() => {
        // Clear auth even if logout request fails
        this.clearAuthTokens();
        this.authStore.clearAuth();
        this.router.navigate(['/auth/login']);
        return of(void 0);
      })
    );
  }

  /**
   * Refresh authentication token
   */
  refreshToken(): Observable<AuthTokenResponse> {
    const refreshToken = this.getStoredRefreshToken();
    if (!refreshToken) {
      return of(null as any);
    }

    return this.http
      .post<AuthTokenResponse>(`${this.apiUrl}/auth/refresh`, {
        refreshToken,
      })
      .pipe(
        tap((response) => {
          this.setAuthTokens(response);
        }),
        catchError(() => {
          this.clearAuthTokens();
          this.authStore.clearAuth();
          this.router.navigate(['/auth/login']);
          return of(null as any);
        })
      );
  }

  /**
   * Validate token
   */
  private validateToken(token: string): void {
    this.http
      .post<User>(`${this.apiUrl}/auth/validate`, { token })
      .pipe(
        tap((user) => {
          this.authStore.setUser(user);
          this.authStore.setToken(token);
        }),
        catchError(() => {
          this.clearAuthTokens();
          return of(null);
        })
      )
      .subscribe();
  }

  /**
   * Get current user
   */
  getCurrentUser(): User | null {
    return this.authStore.user() || this.getStoredUser();
  }

  /**
   * Check if user is authenticated
   */
  isAuthenticated(): boolean {
    return !!this.getStoredToken();
  }

  /**
   * Check if user has role
   */
  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.roles.some((r) => r.name === role) || false;
  }

  /**
   * Check if user has permission
   */
  hasPermission(resource: string, action: string): boolean {
    const user = this.getCurrentUser();
    return (
      user?.permissions.some(
        (p) => p.resource === resource && p.action === action
      ) || false
    );
  }

  /**
   * Get stored auth token
   */
  getToken(): string | null {
    return this.getStoredToken();
  }

  // ============ PRIVATE HELPER METHODS ============

  private setAuthTokens(token: AuthTokenResponse): void {
    localStorage.setItem(this.tokenKey, token.accessToken);
    localStorage.setItem(this.refreshTokenKey, token.refreshToken);
  }

  private getStoredToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private getStoredRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  private clearAuthTokens(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.refreshTokenKey);
    localStorage.removeItem(this.userKey);
  }

  private storeUser(user: User): void {
    localStorage.setItem(this.userKey, JSON.stringify(user));
  }

  private getStoredUser(): User | null {
    const userData = localStorage.getItem(this.userKey);
    return userData ? JSON.parse(userData) : null;
  }
}
