import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { environment } from '@env/environment';

export interface MedicalRecord {
  id: string;
  patientId: string;
  recordType: 'soap' | 'vitals' | 'lab' | 'imaging' | 'procedure' | 'discharge';
  title: string;
  content: string;
  createdBy: string;
  createdAt: Date;
  updatedAt: Date;
  attachments?: string[];
  icdCodes?: string[];
  cptCodes?: string[];
}

export interface SOAPNote {
  id: string;
  patientId: string;
  subjective: string;
  objective: string;
  assessment: string;
  plan: string;
  createdBy: string;
  createdAt: Date;
}

/**
 * Medical Record Service
 * Manages medical records and SOAP notes
 */
@Injectable({
  providedIn: 'root',
})
export class MedicalRecordService {
  private apiUrl = `${environment.apiUrl}/medical-records`;

  constructor(private http: HttpClient) {}

  /**
   * Get patient medical records
   */
  getPatientRecords(patientId: string): Observable<MedicalRecord[]> {
    // Mock implementation
    return of([]).pipe(delay(300));
  }

  /**
   * Get record by ID
   */
  getRecordById(id: string): Observable<MedicalRecord> {
    // Mock implementation
    return of({} as MedicalRecord).pipe(delay(300));
  }

  /**
   * Create new medical record
   */
  createRecord(record: Omit<MedicalRecord, 'id' | 'createdAt' | 'updatedAt'>): Observable<MedicalRecord> {
    // Mock implementation
    const newRecord: MedicalRecord = {
      ...record,
      id: `rec-${Date.now()}`,
      createdAt: new Date(),
      updatedAt: new Date(),
    };
    return of(newRecord).pipe(delay(500));
  }

  /**
   * Update medical record
   */
  updateRecord(id: string, updates: Partial<MedicalRecord>): Observable<MedicalRecord> {
    // Mock implementation
    return of({} as MedicalRecord).pipe(delay(500));
  }

  /**
   * Delete medical record
   */
  deleteRecord(id: string): Observable<void> {
    // Mock implementation
    return of(void 0).pipe(delay(500));
  }

  /**
   * Create SOAP note
   */
  createSOAPNote(note: Omit<SOAPNote, 'id' | 'createdAt'>): Observable<SOAPNote> {
    // Mock implementation
    const newNote: SOAPNote = {
      ...note,
      id: `soap-${Date.now()}`,
      createdAt: new Date(),
    };
    return of(newNote).pipe(delay(500));
  }

  /**
   * Get SOAP notes for patient
   */
  getPatientSOAPNotes(patientId: string): Observable<SOAPNote[]> {
    // Mock implementation
    return of([]).pipe(delay(300));
  }

  /**
   * Upload attachment
   */
  uploadAttachment(file: File): Observable<string> {
    // Mock implementation - returns file URL
    return of(`/uploads/${Date.now()}-${file.name}`).pipe(delay(500));
  }
}
