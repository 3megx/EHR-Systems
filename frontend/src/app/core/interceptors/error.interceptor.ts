import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { Router } from '@angular/router';
import { NotificationService } from '../services/notification.service';

/**
 * Error Interceptor
 * Handles HTTP errors globally
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const notificationService = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An error occurred';

      if (error.error instanceof ErrorEvent) {
        // Client-side error
        errorMessage = error.error.message;
      } else {
        // Server-side error
        errorMessage = error.error?.message || error.statusText || errorMessage;

        // Handle specific status codes
        switch (error.status) {
          case 401:
            // Unauthorized
            notificationService.error('Session Expired', 'Please login again.');
            router.navigate(['/auth/login']);
            break;

          case 403:
            // Forbidden
            notificationService.error(
              'Access Denied',
              'You do not have permission to perform this action.'
            );
            break;

          case 404:
            // Not Found
            notificationService.error(
              'Not Found',
              'The requested resource was not found.'
            );
            break;

          case 500:
            // Server Error
            notificationService.error(
              'Server Error',
              'An internal server error occurred. Please try again later.'
            );
            break;

          default:
            notificationService.error('Error', errorMessage);
        }
      }

      return throwError(() => error);
    })
  );
};
