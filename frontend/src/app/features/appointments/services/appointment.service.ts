import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { environment } from '@env/environment';

export interface Appointment {
  id: string;
  patientId: string;
  patientName: string;
  providerId: string;
  providerName: string;
  startTime: Date;
  endTime: Date;
  type: 'consultation' | 'follow-up' | 'procedure' | 'emergency';
  status: 'scheduled' | 'in-progress' | 'completed' | 'cancelled' | 'no-show';
  location: string;
  notes?: string;
  reason: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface AppointmentQuery {
  providerId?: string;
  patientId?: string;
  startDate?: Date;
  endDate?: Date;
  status?: string;
}

/**
 * Appointment Service
 * Handles appointment scheduling and management
 */
@Injectable({
  providedIn: 'root',
})
export class AppointmentService {
  private apiUrl = `${environment.apiUrl}/appointments`;

  constructor(private http: HttpClient) {}

  /**
   * Get appointments
   */
  getAppointments(query: AppointmentQuery): Observable<Appointment[]> {
    // Mock implementation
    const mockAppointments: Appointment[] = [
      {
        id: 'apt-1',
        patientId: 'pat-1',
        patientName: 'Robert Wilson',
        providerId: 'user-1',
        providerName: 'John Smith',
        startTime: new Date(Date.now() + 86400000),
        endTime: new Date(Date.now() + 90000000),
        type: 'consultation',
        status: 'scheduled',
        location: 'Room 101',
        reason: 'Annual checkup',
        createdAt: new Date(),
        updatedAt: new Date(),
      },
    ];
    return of(mockAppointments).pipe(delay(300));
  }

  /**
   * Get appointment by ID
   */
  getAppointmentById(id: string): Observable<Appointment> {
    // Mock implementation
    return of({} as Appointment).pipe(delay(300));
  }

  /**
   * Create appointment
   */
  createAppointment(appointment: Omit<Appointment, 'id' | 'createdAt' | 'updatedAt'>): Observable<Appointment> {
    // Mock implementation
    const newAppointment: Appointment = {
      ...appointment,
      id: `apt-${Date.now()}`,
      createdAt: new Date(),
      updatedAt: new Date(),
    };
    return of(newAppointment).pipe(delay(500));
  }

  /**
   * Update appointment
   */
  updateAppointment(id: string, updates: Partial<Appointment>): Observable<Appointment> {
    // Mock implementation
    return of({} as Appointment).pipe(delay(500));
  }

  /**
   * Cancel appointment
   */
  cancelAppointment(id: string, reason: string): Observable<void> {
    // Mock implementation
    return of(void 0).pipe(delay(500));
  }

  /**
   * Get available slots
   */
  getAvailableSlots(providerId: string, date: Date): Observable<Date[]> {
    // Mock implementation
    const slots: Date[] = [];
    const baseDate = new Date(date);
    for (let hour = 9; hour < 17; hour++) {
      slots.push(new Date(baseDate.getFullYear(), baseDate.getMonth(), baseDate.getDate(), hour, 0));
    }
    return of(slots).pipe(delay(300));
  }

  /**
   * Check provider availability
   */
  isProviderAvailable(providerId: string, startTime: Date, endTime: Date): Observable<boolean> {
    // Mock implementation
    return of(true).pipe(delay(200));
  }
}
