import { Component, OnInit, OnDestroy, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AuthService } from '../../../../core/services/auth.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { ButtonComponent } from '../../../../shared/components/ui/button/button.component';
import { CardComponent } from '../../../../shared/components/ui/card/card.component';

/**
 * Login Page Component
 * User authentication form
 */
@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    ButtonComponent,
    CardComponent,
  ],
  template: `
    <app-card>
      <h1 class="text-2xl font-bold mb-6 text-center text-gray-900 dark:text-white">
        Sign In
      </h1>

      <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="space-y-4">
        <!-- Email -->
        <div>
          <label for="email" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
            Email Address
          </label>
          <input
            id="email"
            type="email"
            formControlName="email"
            placeholder="your@email.com"
            class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg"
            class="focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
          />
          <p
            *ngIf="
              loginForm.get('email')?.invalid && loginForm.get('email')?.touched
            "
            class="mt-1 text-sm text-red-600"
          >
            Please enter a valid email
          </p>
        </div>

        <!-- Password -->
        <div>
          <div class="flex items-center justify-between mb-2">
            <label for="password" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Password
            </label>
            <a href="/auth/forgot-password" class="text-sm text-blue-600 hover:text-blue-700">
              Forgot?
            </a>
          </div>
          <input
            id="password"
            type="password"
            formControlName="password"
            placeholder="••••••••"
            class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg"
            class="focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
          />
          <p
            *ngIf="
              loginForm.get('password')?.invalid && loginForm.get('password')?.touched
            "
            class="mt-1 text-sm text-red-600"
          >
            Password is required
          </p>
        </div>

        <!-- Remember Me -->
        <div class="flex items-center">
          <input
            id="remember"
            type="checkbox"
            formControlName="rememberMe"
            class="h-4 w-4 text-blue-600 rounded"
          />
          <label for="remember" class="ml-2 text-sm text-gray-700 dark:text-gray-300">
            Remember me
          </label>
        </div>

        <!-- Error Message -->
        <div *ngIf="loginForm.get('error')?.value" class="p-3 bg-red-50 dark:bg-red-900/20 text-red-800 dark:text-red-200 rounded-lg text-sm">
          {{ loginForm.get('error')?.value }}
        </div>

        <!-- Submit Button -->
        <app-button
          variant="primary"
          size="lg"
          [disabled]="loginForm.invalid"
          [loading]="isLoading"
          (clicked)="onSubmit()"
          class="w-full"
        >
          Sign In
        </app-button>
      </form>

      <!-- Sign Up Link -->
      <p class="mt-4 text-center text-gray-600 dark:text-gray-400">
        Don't have an account?
        <a href="/auth/register" class="text-blue-600 hover:text-blue-700 font-medium">
          Sign up here
        </a>
      </p>
    </app-card>
  `,
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginPageComponent implements OnInit, OnDestroy {
  loginForm: FormGroup;
  isLoading = false;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private notificationService: NotificationService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false],
      error: [''],
    });
  }

  ngOnInit(): void {
    // Check if already logged in
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    const { email, password, rememberMe } = this.loginForm.value;

    this.authService
      .login({ email, password, rememberMe })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.notificationService.success('Success', 'Logged in successfully');
        },
        error: (error) => {
          this.isLoading = false;
          const errorMessage = error.error?.message || 'Login failed';
          this.loginForm.patchValue({ error: errorMessage });
        },
      });
  }
}
