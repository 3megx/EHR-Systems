import { Injectable } from '@angular/core';
import { AbstractControl, AsyncValidator, ValidationErrors, AsyncValidatorFn } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { map, catchError, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

/**
 * Async Email Validator
 * Checks if email is already registered
 * Usage: validators: [Validators.required, Validators.email, uniqueEmailValidator()]
 */
@Injectable({
  providedIn: 'root',
})
export class AsyncEmailValidator implements AsyncValidator {
  constructor(private http: HttpClient) {}

  validate(control: AbstractControl): Observable<ValidationErrors | null> {
    if (!control.value) {
      return of(null);
    }

    return this.checkEmailAvailability(control.value).pipe(
      debounceTime(300),
      distinctUntilChanged(),
      map((exists: boolean) => (exists ? { emailTaken: true } : null)),
      catchError(() => of(null))
    );
  }

  private checkEmailAvailability(email: string): Observable<boolean> {
    // Mock implementation - replace with real API call
    return of(false);
    // return this.http.post<{ exists: boolean }>('/api/auth/check-email', { email })
    //   .pipe(
    //     map(response => response.exists),
    //     catchError(() => of(false))
    //   );
  }
}

/**
 * Async Email Validator Function
 */
export function uniqueEmailValidator(http: HttpClient): AsyncValidatorFn {
  return (control: AbstractControl): Observable<ValidationErrors | null> => {
    if (!control.value) {
      return of(null);
    }

    return of(false).pipe(
      debounceTime(300),
      distinctUntilChanged(),
      map((exists: boolean) => (exists ? { emailTaken: true } : null)),
      catchError(() => of(null))
    );
  };
}
