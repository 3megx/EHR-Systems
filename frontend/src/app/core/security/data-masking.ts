/**
 * Data Masking Utilities
 * HIPAA-compliant data masking for sensitive information
 */

/**
 * Mask SSN (Social Security Number)
 * Format: XXX-XX-1234 (shows last 4 digits)
 */
export function maskSSN(ssn: string): string {
  if (!ssn || ssn.length < 4) return ssn;
  const lastFour = ssn.slice(-4);
  return `XXX-XX-${lastFour}`;
}

/**
 * Mask email address
 * Format: j***@email.com (shows first letter and domain)
 */
export function maskEmail(email: string): string {
  if (!email || !email.includes('@')) return email;
  const [localPart, domain] = email.split('@');
  const maskedLocal = localPart.charAt(0) + '*'.repeat(localPart.length - 1);
  return `${maskedLocal}@${domain}`;
}

/**
 * Mask phone number
 * Format: (XXX) XXX-1234 (shows last 4 digits)
 */
export function maskPhoneNumber(phone: string): string {
  if (!phone || phone.length < 4) return phone;
  const lastFour = phone.slice(-4);
  return `(XXX) XXX-${lastFour}`;
}

/**
 * Mask credit card number
 * Format: XXXX-XXXX-XXXX-1234 (shows last 4 digits)
 */
export function maskCreditCard(cardNumber: string): string {
  if (!cardNumber || cardNumber.length < 4) return cardNumber;
  const lastFour = cardNumber.slice(-4);
  const masked = 'X'.repeat(cardNumber.length - 4) + lastFour;
  return masked.replace(/(.{4})/g, '$1-').slice(0, -1);
}

/**
 * Mask medical record number
 * Format: MRN***234 (shows prefix and last 3 digits)
 */
export function maskMRN(mrn: string): string {
  if (!mrn || mrn.length < 3) return mrn;
  const prefix = mrn.slice(0, 3);
  const lastThree = mrn.slice(-3);
  const masked = '*'.repeat(Math.max(0, mrn.length - 6));
  return `${prefix}${masked}${lastThree}`;
}

/**
 * Mask patient name
 * Format: John D. (shows first name and initial)
 */
export function maskPatientName(firstName: string, lastName: string): string {
  const firstInitial = firstName?.charAt(0) || '';
  const lastInitial = lastName?.charAt(0) || '';
  return `${firstInitial}. ${lastInitial}.`;
}

/**
 * Mask date of birth
 * Format: 1965-XX-XX (shows only year)
 */
export function maskDOB(dob: Date | string): string {
  const date = new Date(dob);
  const year = date.getFullYear();
  return `${year}-XX-XX`;
}

/**
 * Mask sensitive object properties
 */
export function maskObjectProperties(
  obj: any,
  propertiesToMask: Record<string, 'ssn' | 'email' | 'phone' | 'credit-card' | 'mrn' | 'dob'>
): any {
  const masked = { ...obj };

  for (const [key, maskType] of Object.entries(propertiesToMask)) {
    if (key in masked) {
      switch (maskType) {
        case 'ssn':
          masked[key] = maskSSN(masked[key]);
          break;
        case 'email':
          masked[key] = maskEmail(masked[key]);
          break;
        case 'phone':
          masked[key] = maskPhoneNumber(masked[key]);
          break;
        case 'credit-card':
          masked[key] = maskCreditCard(masked[key]);
          break;
        case 'mrn':
          masked[key] = maskMRN(masked[key]);
          break;
        case 'dob':
          masked[key] = maskDOB(masked[key]);
          break;
      }
    }
  }

  return masked;
}

/**
 * Check if user has permission to view unmasked data
 * Based on roles and HIPAA compliance
 */
export function canViewUnmaskedData(userRoles: string[], dataType: string): boolean {
  const allowedRoles: Record<string, string[]> = {
    'medical-record': ['doctor', 'nurse', 'admin'],
    'billing-info': ['admin', 'billing-officer'],
    'financial': ['admin'],
    'insurance': ['admin', 'doctor', 'nurse'],
  };

  const requiredRoles = allowedRoles[dataType] || [];
  return userRoles.some((role) => requiredRoles.includes(role));
}
