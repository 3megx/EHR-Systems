/**
 * String Utilities
 * Common string manipulation functions
 */

/**
 * Capitalize first letter
 */
export function capitalize(str: string): string {
  if (!str) return '';
  return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
}

/**
 * Convert camelCase to title case
 */
export function camelCaseToTitle(str: string): string {
  return str
    .replace(/([A-Z])/g, ' $1')
    .replace(/^./, (str) => str.toUpperCase())
    .trim();
}

/**
 * Convert snake_case to title case
 */
export function snakeCaseToTitle(str: string): string {
  return str
    .split('_')
    .map((word) => capitalize(word))
    .join(' ');
}

/**
 * Truncate string with ellipsis
 */
export function truncate(str: string, length: number, ellipsis: string = '...'): string {
  if (str.length <= length) return str;
  return str.substring(0, length - ellipsis.length) + ellipsis;
}

/**
 * Highlight search term in string
 */
export function highlightSearchTerm(text: string, searchTerm: string): string {
  if (!searchTerm) return text;
  const regex = new RegExp(`(${escapeRegex(searchTerm)})`, 'gi');
  return text.replace(regex, '<mark>$1</mark>');
}

/**
 * Escape regex special characters
 */
export function escapeRegex(str: string): string {
  return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

/**
 * Extract initials from name
 */
export function getInitials(name: string): string {
  return name
    .split(' ')
    .map((n) => n.charAt(0).toUpperCase())
    .join('')
    .slice(0, 2);
}

/**
 * Format phone number
 */
export function formatPhone(phone: string): string {
  const cleaned = phone.replace(/\D/g, '');
  if (cleaned.length !== 10) return phone;
  return `(${cleaned.slice(0, 3)}) ${cleaned.slice(3, 6)}-${cleaned.slice(6)}`;
}

/**
 * Mask sensitive data (for HIPAA compliance)
 */
export function maskSensitiveData(data: string, showChars: number = 4): string {
  if (!data) return '';
  const visible = data.slice(-showChars);
  const masked = '*'.repeat(Math.max(0, data.length - showChars));
  return masked + visible;
}

/**
 * Remove special characters
 */
export function removeSpecialCharacters(str: string): string {
  return str.replace(/[^\w\s]/gi, '');
}

/**
 * Slugify string (convert to URL-safe format)
 */
export function slugify(str: string): string {
  return str
    .toLowerCase()
    .trim()
    .replace(/[^\w\s-]/g, '')
    .replace(/[\s_]+/g, '-')
    .replace(/^-+|-+$/g, '');
}
