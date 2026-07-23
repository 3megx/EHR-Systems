import { Directive, ElementRef, Input, OnInit } from '@angular/core';

export type HighlightColor = 'primary' | 'success' | 'warning' | 'danger' | 'info';

/**
 * Highlight Directive
 * Highlights text matching a search term or adds background highlight
 * Usage: [appHighlight]="searchTerm" or [appHighlight]="'critical'" [highlightColor]="'danger'"
 */
@Directive({
  selector: '[appHighlight]',
  standalone: true,
})
export class HighlightDirective implements OnInit {
  @Input() appHighlight: string = '';
  @Input() highlightColor: HighlightColor = 'primary';
  @Input() caseInsensitive = true;

  private colorMap: Record<HighlightColor, string> = {
    primary: '#3b82f6',
    success: '#10b981',
    warning: '#f59e0b',
    danger: '#ef4444',
    info: '#0ea5e9',
  };

  constructor(private el: ElementRef) {}

  ngOnInit(): void {
    if (this.appHighlight) {
      this.highlightText();
    }
  }

  private highlightText(): void {
    const text = this.el.nativeElement.textContent || '';
    if (!text || !this.appHighlight) {
      return;
    }

    const regex = new RegExp(
      `(${this.escapeRegex(this.appHighlight)})`,
      this.caseInsensitive ? 'gi' : 'g'
    );

    const highlightedText = text.replace(
      regex,
      `<mark style="background-color: ${this.colorMap[this.highlightColor]}; padding: 2px 4px; border-radius: 3px;">$1</mark>`
    );

    this.el.nativeElement.innerHTML = highlightedText;
  }

  private escapeRegex(str: string): string {
    return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
  }
}
