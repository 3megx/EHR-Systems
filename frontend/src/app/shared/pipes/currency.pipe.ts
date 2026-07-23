import { Pipe, PipeTransform } from '@angular/core';

export interface CurrencyOptions {
  currency?: string;
  symbol?: string;
  digitsInfo?: string;
  locale?: string;
}

/**
 * Currency Pipe
 * Formats numbers as currency with customizable options
 * Usage: {{ amount | currencyFormat:'USD' }} or {{ amount | currencyFormat:'USD':'$' }}
 */
@Pipe({
  name: 'currencyFormat',
  standalone: true,
})
export class CurrencyFormatPipe implements PipeTransform {
  private currencySymbols: Record<string, string> = {
    USD: '$',
    EUR: '€',
    GBP: '£',
    JPY: '¥',
    AED: 'د.إ',
    SAR: 'ر.س',
    EGP: '£',
  };

  transform(value: number | null | undefined, currency: string = 'USD', symbol?: string, digitsInfo: string = '1.2-2'): string {
    if (value === null || value === undefined) {
      return '';
    }

    const sym = symbol || this.currencySymbols[currency] || currency;
    const [minIntegerDigits, fractionSize] = this.parseDigitsInfo(digitsInfo);

    const formatted = this.formatNumber(value, minIntegerDigits, fractionSize);
    return `${sym} ${formatted}`;
  }

  private parseDigitsInfo(digitsInfo: string): [number, number] {
    const [integerDigits, fractionDigits] = digitsInfo.split('.');
    const minIntegerDigits = parseInt(integerDigits, 10);
    const fractionSize = fractionDigits ? parseInt(fractionDigits.split('-')[1], 10) : 2;
    return [minIntegerDigits, fractionSize];
  }

  private formatNumber(value: number, minIntegerDigits: number, fractionSize: number): string {
    const factor = Math.pow(10, fractionSize);
    const rounded = Math.round(value * factor) / factor;
    return rounded.toLocaleString('en-US', {
      minimumIntegerDigits: minIntegerDigits,
      minimumFractionDigits: fractionSize,
      maximumFractionDigits: fractionSize,
    });
  }
}
