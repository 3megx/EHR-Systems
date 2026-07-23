import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay, map } from 'rxjs/operators';
import { Patient, PatientSearchResult } from '../../../core/models';
import { MOCK_PATIENTS } from '../../../shared/mock-data';
import { environment } from '@env/environment';

export interface PatientQuery {
  search?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export interface PatientResponse {
  data: Patient[];
  total: number;
  page: number;
  pageSize: number;
}

/**
 * Patient Service
 * Handles patient data operations
 */
@Injectable({
  providedIn: 'root',
})
export class PatientService {
  private apiUrl = `${environment.apiUrl}/patients`;

  constructor(private http: HttpClient) {}

  /**
   * Get all patients with pagination
   */
  getPatients(query: PatientQuery): Observable<PatientResponse> {
    // Mock implementation - replace with real API call
    return of({
      data: MOCK_PATIENTS,
      total: MOCK_PATIENTS.length,
      page: query.page || 1,
      pageSize: query.pageSize || 10,
    }).pipe(delay(500));

    // Real implementation:
    // let params = new HttpParams();
    // if (query.search) params = params.set('search', query.search);
    // if (query.page) params = params.set('page', query.page.toString());
    // if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
    // return this.http.get<PatientResponse>(`${this.apiUrl}`, { params });
  }

  /**
   * Get single patient by ID
   */
  getPatientById(id: string): Observable<Patient> {
    // Mock implementation
    const patient = MOCK_PATIENTS.find((p) => p.id === id);
    return of(patient || ({} as Patient)).pipe(delay(300));

    // Real implementation:
    // return this.http.get<Patient>(`${this.apiUrl}/${id}`);
  }

  /**
   * Search patients
   */
  searchPatients(searchTerm: string): Observable<PatientSearchResult[]> {
    // Mock implementation
    const results = MOCK_PATIENTS.filter(
      (p) =>
        p.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        p.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        p.mrn.toLowerCase().includes(searchTerm.toLowerCase())
    ).map((p) => ({
      id: p.id,
      mrn: p.mrn,
      fullName: `${p.firstName} ${p.lastName}`,
      dateOfBirth: p.dateOfBirth,
      phone: p.phone,
      lastVisit: new Date(),
    }));

    return of(results).pipe(delay(300));

    // Real implementation:
    // return this.http.get<PatientSearchResult[]>(`${this.apiUrl}/search`, {
    //   params: new HttpParams().set('q', searchTerm)
    // });
  }

  /**
   * Create new patient
   */
  createPatient(patient: Omit<Patient, 'id' | 'createdAt' | 'updatedAt'>): Observable<Patient> {
    // Mock implementation
    const newPatient: Patient = {
      ...patient,
      id: `pat-${Date.now()}`,
      createdAt: new Date(),
      updatedAt: new Date(),
    };
    return of(newPatient).pipe(delay(500));

    // Real implementation:
    // return this.http.post<Patient>(`${this.apiUrl}`, patient);
  }

  /**
   * Update patient
   */
  updatePatient(id: string, updates: Partial<Patient>): Observable<Patient> {
    // Mock implementation
    const patient = MOCK_PATIENTS.find((p) => p.id === id);
    if (patient) {
      const updated = { ...patient, ...updates, updatedAt: new Date() };
      return of(updated).pipe(delay(500));
    }
    return of({} as Patient);

    // Real implementation:
    // return this.http.put<Patient>(`${this.apiUrl}/${id}`, updates);
  }

  /**
   * Delete patient
   */
  deletePatient(id: string): Observable<void> {
    // Mock implementation
    return of(void 0).pipe(delay(500));

    // Real implementation:
    // return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get patient allergies
   */
  getPatientAllergies(patientId: string): Observable<any[]> {
    const patient = MOCK_PATIENTS.find((p) => p.id === patientId);
    return of(patient?.allergies || []).pipe(delay(300));
  }

  /**
   * Get patient chronic conditions
   */
  getPatientConditions(patientId: string): Observable<any[]> {
    const patient = MOCK_PATIENTS.find((p) => p.id === patientId);
    return of(patient?.chronicConditions || []).pipe(delay(300));
  }

  /**
   * Export patients as CSV
   */
  exportPatientsAsCSV(query: PatientQuery): Observable<Blob> {
    // This would generate a CSV file
    return new Observable((observer) => {
      const csv = this.generatePatientCSV(MOCK_PATIENTS);
      const blob = new Blob([csv], { type: 'text/csv' });
      observer.next(blob);
      observer.complete();
    });
  }

  private generatePatientCSV(patients: Patient[]): string {
    const headers = ['MRN', 'First Name', 'Last Name', 'DOB', 'Gender', 'Phone', 'Email'];
    const rows = patients.map((p) => [
      p.mrn,
      p.firstName,
      p.lastName,
      p.dateOfBirth.toLocaleDateString(),
      p.gender,
      p.phone || '',
      p.email || '',
    ]);

    return [headers, ...rows].map((row) => row.map((cell) => `"${cell}"`).join(',')).join('\n');
  }
}
