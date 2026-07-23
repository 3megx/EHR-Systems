/**
 * Patient Base Model
 * Core patient information
 */
export interface Patient {
  id: string;
  mrn: string; // Medical Record Number
  firstName: string;
  lastName: string;
  dateOfBirth: Date;
  gender: 'male' | 'female' | 'other';
  email?: string;
  phone?: string;
  address?: Address;
  emergencyContact?: EmergencyContact;
  allergies: Allergy[];
  chronicConditions: ChronicCondition[];
  insuranceInfo?: InsuranceInfo;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

/**
 * Patient Address
 */
export interface Address {
  street: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
}

/**
 * Emergency Contact
 */
export interface EmergencyContact {
  name: string;
  relationship: string;
  phone: string;
  email?: string;
}

/**
 * Patient Allergy
 */
export interface Allergy {
  id: string;
  name: string;
  severity: 'mild' | 'moderate' | 'severe';
  reaction: string;
  recordedDate: Date;
  resolvedDate?: Date;
}

/**
 * Chronic Condition
 */
export interface ChronicCondition {
  id: string;
  icdCode: string; // ICD-10 code
  name: string;
  diagnosisDate: Date;
  status: 'active' | 'resolved' | 'controlled';
  notes?: string;
}

/**
 * Insurance Information
 */
export interface InsuranceInfo {
  provider: string;
  policyNumber: string;
  groupNumber?: string;
  effectiveDate: Date;
  expirationDate?: Date;
  coverageType: 'commercial' | 'medicaid' | 'medicare' | 'self-pay';
}

/**
 * Patient Search Result
 */
export interface PatientSearchResult {
  id: string;
  mrn: string;
  fullName: string;
  dateOfBirth: Date;
  phone?: string;
  lastVisit?: Date;
}

/**
 * Patient Demographics Update
 */
export interface PatientDemographicsUpdate {
  firstName?: string;
  lastName?: string;
  phone?: string;
  email?: string;
  address?: Address;
  emergencyContact?: EmergencyContact;
}
