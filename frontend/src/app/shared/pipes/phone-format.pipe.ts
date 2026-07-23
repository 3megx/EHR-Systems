import { Pipe, PipeTransform } from '@angular/core';

export type PhoneFormatType = 'us' | 'international' | 'simple';

/**
 * Phone Format Pipe
 * Formats phone numbers in various formats
 * Usage: {{ phoneNumber | phoneFormat:'us' }}
 */
@Pipe({
  name: 'phoneFormat',
  standalone: true,
})
export class PhoneFormatPipe implements PipeTransform {
  transform(value: string | null | undefined, format: PhoneFormatType = 'us'): string {
    if (!value) {
      return '';
    }

    // Remove all non-digit characters
    const cleaned = value.replace(/\D/g, '');

    switch (format) {
      case 'us':
        return this.formatUS(cleaned);
      case 'international':
        return this.formatInternational(cleaned);
      case 'simple':
        return this.formatSimple(cleaned);
      default:
        return value;
    }
  }

  private formatUS(phone: string): string {
    // Format: (XXX) XXX-XXXX
    if (phone.length !== 10) {
      return phone;
    }
    return `(${phone.slice(0, 3)}) ${phone.slice(3, 6)}-${phone.slice(6)}`;
  }

  private formatInternational(phone: string): string {
    // Format: +1 (XXX) XXX-XXXX
    if (phone.length !== 10) {
      return phone;
    }
    return `+1 (${phone.slice(0, 3)}) ${phone.slice(3, 6)}-${phone.slice(6)}`;
  }

  private formatSimple(phone: string): string {
    // Format: XXX-XXX-XXXX
    if (phone.length !== 10) {
      return phone;
    }
    return `${phone.slice(0, 3)}-${phone.slice(3, 6)}-${phone.slice(6)}`;
  }
}
