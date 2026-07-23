import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml, SafeResourceUrl, SafeScript, SafeStyle, SafeUrl } from '@angular/platform-browser';

/**
 * Safe Pipe
 * Sanitizes potentially dangerous HTML and URLs
 * Usage: {{ htmlContent | safe:'html' }} or {{ url | safe:'url' }}
 */
@Pipe({
  name: 'safe',
  standalone: true,
})
export class SafePipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) {}

  transform(value: string, type: string): SafeHtml | SafeStyle | SafeScript | SafeUrl | SafeResourceUrl {
    if (!value) {
      return value;
    }

    switch (type.toLowerCase()) {
      case 'html':
        return this.sanitizer.bypassSecurityTrustHtml(value);
      case 'style':
        return this.sanitizer.bypassSecurityTrustStyle(value);
      case 'script':
        return this.sanitizer.bypassSecurityTrustScript(value);
      case 'url':
        return this.sanitizer.bypassSecurityTrustUrl(value);
      case 'resourceurl':
        return this.sanitizer.bypassSecurityTrustResourceUrl(value);
      default:
        throw new Error(`Invalid safe type specified: ${type}`);
    }
  }
}
