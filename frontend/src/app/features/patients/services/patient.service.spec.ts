import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { PatientService } from './patient.service';
import { MOCK_PATIENTS } from '@shared/mock-data';

describe('PatientService', () => {
  let service: PatientService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PatientService],
    });

    service = TestBed.inject(PatientService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all patients', (done) => {
    service.getPatients({ page: 1, pageSize: 10 }).subscribe((response) => {
      expect(response.data).toBeDefined();
      expect(response.total).toBeGreaterThan(0);
      done();
    });
  });

  it('should get patient by ID', (done) => {
    const patientId = MOCK_PATIENTS[0].id;

    service.getPatientById(patientId).subscribe((patient) => {
      expect(patient.id).toBe(patientId);
      expect(patient.firstName).toBe(MOCK_PATIENTS[0].firstName);
      done();
    });
  });

  it('should search patients', (done) => {
    const searchTerm = 'Robert';

    service.searchPatients(searchTerm).subscribe((results) => {
      expect(results.length).toBeGreaterThan(0);
      expect(results[0].fullName).toContain('Robert');
      done();
    });
  });

  it('should create new patient', (done) => {
    const newPatient = {
      mrn: 'MRN123456',
      firstName: 'Jane',
      lastName: 'Doe',
      dateOfBirth: new Date('1990-01-01'),
      gender: 'female' as const,
      allergies: [],
      chronicConditions: [],
      isActive: true,
    };

    service.createPatient(newPatient).subscribe((created) => {
      expect(created.id).toBeDefined();
      expect(created.mrn).toBe('MRN123456');
      done();
    });
  });

  it('should get patient allergies', (done) => {
    const patientId = MOCK_PATIENTS[0].id;

    service.getPatientAllergies(patientId).subscribe((allergies) => {
      expect(allergies).toBeDefined();
      expect(Array.isArray(allergies)).toBe(true);
      done();
    });
  });

  it('should get patient conditions', (done) => {
    const patientId = MOCK_PATIENTS[0].id;

    service.getPatientConditions(patientId).subscribe((conditions) => {
      expect(conditions).toBeDefined();
      expect(Array.isArray(conditions)).toBe(true);
      done();
    });
  });
});
