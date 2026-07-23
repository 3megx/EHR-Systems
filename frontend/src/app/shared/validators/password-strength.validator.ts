import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export enum PasswordStrength {
  Weak = 1,
  Fair = 2,
  Good = 3,
  Strong = 4,
  VeryStrong = 5,
}

/**
 * Password Strength Validator
 * Validates password complexity and strength
 * Usage: [Validators.pattern(/regex/), passwordStrengthValidator()]
 */
export function passwordStrengthValidator(
  minStrength: PasswordStrength = PasswordStrength.Strong
): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    const password = control.value as string;
    let strength = 0;

    // Check length
    if (password.length >= 8) strength++;
    if (password.length >= 12) strength++;

    // Check character types
    if (/[a-z]/.test(password)) strength++;
    if (/[A-Z]/.test(password)) strength++;
    if (/[0-9]/.test(password)) strength++;
    if (/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) strength++;

    // Normalize to 1-5
    strength = Math.min(strength, PasswordStrength.VeryStrong);

    if (strength < minStrength) {
      return {
        passwordStrength: {
          requiredStrength: minStrength,
          actualStrength: strength,
        },
      };
    }

    return null;
  };
}

/**
 * Get password strength label
 */
export function getPasswordStrengthLabel(strength: number): string {
  switch (strength) {
    case PasswordStrength.Weak:
      return 'Weak';
    case PasswordStrength.Fair:
      return 'Fair';
    case PasswordStrength.Good:
      return 'Good';
    case PasswordStrength.Strong:
      return 'Strong';
    case PasswordStrength.VeryStrong:
      return 'Very Strong';
    default:
      return 'Unknown';
  }
}

/**
 * Validate matching passwords
 * Usage: validators: [Validators.required, matchPasswordValidator('password')]
 */
export function matchPasswordValidator(passwordFieldName: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    const passwordControl = control.parent?.get(passwordFieldName);
    if (!passwordControl) {
      return null;
    }

    if (control.value !== passwordControl.value) {
      return { passwordMismatch: true };
    }

    return null;
  };
}
