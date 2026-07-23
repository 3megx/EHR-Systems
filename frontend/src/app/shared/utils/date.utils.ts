/**
 * Date Utilities
 * Common date manipulation and formatting functions
 */

/**
 * Calculate age from date of birth
 */
export function calculateAge(dateOfBirth: Date | string): number {
  const birthDate = new Date(dateOfBirth);
  const today = new Date();
  let age = today.getFullYear() - birthDate.getFullYear();
  const monthDiff = today.getMonth() - birthDate.getMonth();

  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
    age--;
  }

  return age;
}

/**
 * Format date to readable string
 */
export function formatDateReadable(date: Date | string, format: 'short' | 'long' = 'long'): string {
  const d = new Date(date);
  const options: Intl.DateTimeFormatOptions =
    format === 'short'
      ? { year: '2-digit', month: '2-digit', day: '2-digit' }
      : { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };

  return d.toLocaleDateString('en-US', options);
}

/**
 * Get days between two dates
 */
export function daysBetween(date1: Date | string, date2: Date | string): number {
  const d1 = new Date(date1);
  const d2 = new Date(date2);
  const diffTime = Math.abs(d2.getTime() - d1.getTime());
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  return diffDays;
}

/**
 * Check if date is today
 */
export function isToday(date: Date | string): boolean {
  const d = new Date(date);
  const today = new Date();
  return (
    d.getDate() === today.getDate() &&
    d.getMonth() === today.getMonth() &&
    d.getFullYear() === today.getFullYear()
  );
}

/**
 * Check if date is in the past
 */
export function isPastDate(date: Date | string): boolean {
  const d = new Date(date);
  const now = new Date();
  return d < now;
}

/**
 * Check if date is in the future
 */
export function isFutureDate(date: Date | string): boolean {
  const d = new Date(date);
  const now = new Date();
  return d > now;
}

/**
 * Get date range (start and end of day)
 */
export function getDateRange(date: Date | string): { start: Date; end: Date } {
  const d = new Date(date);
  const start = new Date(d.getFullYear(), d.getMonth(), d.getDate(), 0, 0, 0, 0);
  const end = new Date(d.getFullYear(), d.getMonth(), d.getDate(), 23, 59, 59, 999);
  return { start, end };
}

/**
 * Get week start and end dates
 */
export function getWeekRange(date: Date | string): { start: Date; end: Date } {
  const d = new Date(date);
  const day = d.getDay();
  const diff = d.getDate() - day + (day === 0 ? -6 : 1); // Adjust when day is Sunday

  const start = new Date(d.setDate(diff));
  start.setHours(0, 0, 0, 0);

  const end = new Date(start);
  end.setDate(end.getDate() + 6);
  end.setHours(23, 59, 59, 999);

  return { start, end };
}

/**
 * Get month start and end dates
 */
export function getMonthRange(date: Date | string): { start: Date; end: Date } {
  const d = new Date(date);
  const start = new Date(d.getFullYear(), d.getMonth(), 1);
  const end = new Date(d.getFullYear(), d.getMonth() + 1, 0);
  end.setHours(23, 59, 59, 999);

  return { start, end };
}
