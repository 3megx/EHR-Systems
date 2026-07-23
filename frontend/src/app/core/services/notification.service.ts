import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';

export type NotificationType = 'success' | 'error' | 'info' | 'warning';

export interface Notification {
  id: string;
  type: NotificationType;
  title: string;
  message: string;
  duration?: number;
}

/**
 * Notification Service
 * Centralized notification management
 */
@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private notificationSubject = new Subject<Notification>();
  notifications$ = this.notificationSubject.asObservable();

  private closeSubject = new Subject<string>();
  close$ = this.closeSubject.asObservable();

  /**
   * Show success notification
   */
  success(title: string, message: string, duration?: number): void {
    this.notify('success', title, message, duration);
  }

  /**
   * Show error notification
   */
  error(title: string, message: string, duration?: number): void {
    this.notify('error', title, message, duration);
  }

  /**
   * Show info notification
   */
  info(title: string, message: string, duration?: number): void {
    this.notify('info', title, message, duration);
  }

  /**
   * Show warning notification
   */
  warning(title: string, message: string, duration?: number): void {
    this.notify('warning', title, message, duration);
  }

  /**
   * Notify
   */
  private notify(
    type: NotificationType,
    title: string,
    message: string,
    duration: number = 3000
  ): void {
    const id = this.generateId();
    const notification: Notification = {
      id,
      type,
      title,
      message,
      duration,
    };

    this.notificationSubject.next(notification);

    if (duration > 0) {
      setTimeout(() => {
        this.close(id);
      }, duration);
    }
  }

  /**
   * Close notification
   */
  close(id: string): void {
    this.closeSubject.next(id);
  }

  /**
   * Generate unique ID
   */
  private generateId(): string {
    return `notification-${Date.now()}-${Math.random()}`;
  }
}
