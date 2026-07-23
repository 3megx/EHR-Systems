import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { environment } from '@env/environment';

export interface Prescription {
  id: string;
  patientId: string;
  medicationName: string;
  ndc: string;
  dosage: string;
  frequency: string;
  route: 'oral' | 'injection' | 'intravenous' | 'topical' | 'inhaled';
  quantity: number;
  refills: number;
  startDate: Date;
  endDate?: Date;
  prescribedBy: string;
  status: 'active' | 'inactive' | 'filled' | 'cancelled';
  notes?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface MedicationInteraction {
  medication1: string;
  medication2: string;
  severity: 'mild' | 'moderate' | 'severe';
  description: string;
}

/**
 * Prescription Service
 * Manages prescriptions and medication interactions
 */
@Injectable({
  providedIn: 'root',
})
export class PrescriptionService {
  private apiUrl = `${environment.apiUrl}/prescriptions`;

  constructor(private http: HttpClient) {}

  /**
   * Get patient prescriptions
   */
  getPatientPrescriptions(patientId: string): Observable<Prescription[]> {
    // Mock implementation
    return of([]).pipe(delay(300));
  }

  /**
   * Get active prescriptions
   */
  getActivePrescriptions(patientId: string): Observable<Prescription[]> {
    // Mock implementation
    return of([]).pipe(delay(300));
  }

  /**
   * Get prescription by ID
   */
  getPrescriptionById(id: string): Observable<Prescription> {
    // Mock implementation
    return of({} as Prescription).pipe(delay(300));
  }

  /**
   * Create prescription
   */
  createPrescription(prescription: Omit<Prescription, 'id' | 'createdAt' | 'updatedAt'>): Observable<Prescription> {
    // Mock implementation
    const newPrescription: Prescription = {
      ...prescription,
      id: `presc-${Date.now()}`,
      createdAt: new Date(),
      updatedAt: new Date(),
    };
    return of(newPrescription).pipe(delay(500));
  }

  /**
   * Update prescription
   */
  updatePrescription(id: string, updates: Partial<Prescription>): Observable<Prescription> {
    // Mock implementation
    return of({} as Prescription).pipe(delay(500));
  }

  /**
   * Cancel prescription
   */
  cancelPrescription(id: string, reason: string): Observable<void> {
    // Mock implementation
    return of(void 0).pipe(delay(500));
  }

  /**
   * Check medication interactions
   */
  checkInteractions(medications: string[]): Observable<MedicationInteraction[]> {
    // Mock implementation
    const interactions: MedicationInteraction[] = [];

    // Simple mock interaction check
    if (medications.includes('Warfarin') && medications.includes('Aspirin')) {
      interactions.push({
        medication1: 'Warfarin',
        medication2: 'Aspirin',
        severity: 'severe',
        description: 'Increased bleeding risk when combined',
      });
    }

    return of(interactions).pipe(delay(300));
  }

  /**
   * Refill prescription
   */
  refillPrescription(id: string): Observable<Prescription> {
    // Mock implementation
    return of({} as Prescription).pipe(delay(500));
  }

  /**
   * Send prescription to pharmacy
   */
  sendToPharmacy(id: string, pharmacyId: string): Observable<void> {
    // Mock implementation
    return of(void 0).pipe(delay(500));
  }
}
