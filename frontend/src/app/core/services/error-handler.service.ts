import { ErrorHandler, Injectable, Injector } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { LoggerService } from './logger.service';
import { NotificationService } from './notification.service';

/**
 * Global Error Handler
 * Centralized error handling and reporting
 */
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(private injector: Injector) {}

  handleError(error: Error | HttpErrorResponse): void {
    const logger = this.injector.get(LoggerService);
    const notificationService = this.injector.get(NotificationService);

    // Log the error
    logger.error('Global Error Handler', error);

    // Handle HTTP errors
    if (error instanceof HttpErrorResponse) {
      this.handleHttpError(error, logger, notificationService);
    } else {
      // Handle client-side errors
      this.handleClientError(error, logger, notificationService);
    }
  }

  private handleHttpError(
    error: HttpErrorResponse,
    logger: LoggerService,
    notificationService: NotificationService
  ): void {
    let message = 'An error occurred';
    let title = 'Error';

    switch (error.status) {
      case 0:
        title = 'Network Error';
        message = 'Unable to connect to the server. Please check your internet connection.';
        break;

      case 400:
        title = 'Bad Request';
        message = error.error?.message || 'Invalid request. Please check your input.';
        break;

      case 401:
        title = 'Unauthorized';
        message = 'Your session has expired. Please login again.';
        break;

      case 403:
        title = 'Forbidden';
        message = 'You do not have permission to access this resource.';
        break;

      case 404:
        title = 'Not Found';
        message = 'The requested resource was not found.';
        break;

      case 500:
        title = 'Server Error';
        message = 'An internal server error occurred. Please try again later.';
        break;

      case 503:
        title = 'Service Unavailable';
        message = 'The service is temporarily unavailable. Please try again later.';
        break;

      default:
        title = `HTTP Error ${error.status}`;
        message = error.error?.message || 'An unexpected error occurred.';
    }

    logger.error(title, error, { status: error.status, message: error.message });
    notificationService.error(title, message);
  }

  private handleClientError(
    error: Error,
    logger: LoggerService,
    notificationService: NotificationService
  ): void {
    const message = error.message || 'An unexpected error occurred';
    const stack = error.stack;

    logger.error('Client Error', error, { stack });

    // Show user-friendly error message
    notificationService.error('Error', 'Something went wrong. Please try again.');
  }
}
