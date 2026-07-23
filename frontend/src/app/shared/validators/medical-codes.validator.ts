import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * ICD-10 Code Validator
 * Validates ICD-10 diagnostic codes
 * Format: A00-Z99(.X{0,2})?
 * Usage: validators: [Validators.required, icd10Validator()]
 */
export function icd10Validator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    const icd10Regex = /^[A-Z][0-9]{2}(\.[A-Z0-9]{1,2})?$/;

    if (!icd10Regex.test(control.value)) {
      return { invalidICD10: { value: control.value } };
    }

    return null;
  };
}

/**
 * CPT Code Validator
 * Validates CPT (Current Procedural Terminology) codes
 * Format: 5 digits
 * Usage: validators: [Validators.required, cptValidator()]
 */
export function cptValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    const cptRegex = /^\d{5}$/;

    if (!cptRegex.test(control.value)) {
      return { invalidCPT: { value: control.value } };
    }

    return null;
  };
}

/**
 * Medical Record Number (MRN) Validator
 * Validates MRN format
 * Usage: validators: [Validators.required, mrnValidator()]
 */
export function mrnValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    // MRN format: 7-10 alphanumeric characters
    const mrnRegex = /^[A-Z0-9]{7,10}$/i;

    if (!mrnRegex.test(control.value)) {
      return { invalidMRN: { value: control.value } };
    }

    return null;
  };
}

/**
 * National Provider Identifier (NPI) Validator
 * Validates 10-digit NPI
 * Usage: validators: [Validators.required, npiValidator()]
 */
export function npiValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    const npiRegex = /^\d{10}$/;

    if (!npiRegex.test(control.value)) {
      return { invalidNPI: { value: control.value } };
    }

    return null;
  };
}

/**
 * NDC (National Drug Code) Validator
 * Validates 10 or 11 digit NDC format
 * Usage: validators: [Validators.required, ndcValidator()]
 */
export function ndcValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    const ndcRegex = /^\d{10,11}$/;

    if (!ndcRegex.test(control.value)) {
      return { invalidNDC: { value: control.value } };
    }

    return null;
  };
}
