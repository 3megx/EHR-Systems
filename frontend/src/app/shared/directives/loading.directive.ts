import { Directive, ElementRef, Input, Renderer2, OnChanges, SimpleChanges } from '@angular/core';

/**
 * Loading Directive
 * Shows loading state overlay on element
 * Usage: [appLoading]="isLoading" or [appLoading]="{ loading: true, message: 'Loading...' }"
 */
@Directive({
  selector: '[appLoading]',
  standalone: true,
})
export class LoadingDirective implements OnChanges {
  @Input() appLoading: boolean | { loading: boolean; message?: string } = false;
  @Input() loadingColor = '#3b82f6';

  private loadingOverlay: HTMLElement | null = null;

  constructor(
    private el: ElementRef,
    private renderer: Renderer2
  ) {
    // Make host element position: relative
    this.renderer.setStyle(this.el.nativeElement, 'position', 'relative');
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['appLoading']) {
      this.updateLoadingState();
    }
  }

  private updateLoadingState(): void {
    const isLoading = this.getLoadingState();

    if (isLoading) {
      this.showLoadingOverlay();
    } else {
      this.hideLoadingOverlay();
    }
  }

  private getLoadingState(): boolean {
    if (typeof this.appLoading === 'boolean') {
      return this.appLoading;
    }
    return this.appLoading?.loading ?? false;
  }

  private showLoadingOverlay(): void {
    if (this.loadingOverlay) {
      return; // Already shown
    }

    const overlay = this.renderer.createElement('div');
    this.renderer.setStyle(overlay, 'position', 'absolute');
    this.renderer.setStyle(overlay, 'top', '0');
    this.renderer.setStyle(overlay, 'left', '0');
    this.renderer.setStyle(overlay, 'right', '0');
    this.renderer.setStyle(overlay, 'bottom', '0');
    this.renderer.setStyle(overlay, 'background-color', 'rgba(255, 255, 255, 0.8)');
    this.renderer.setStyle(overlay, 'display', 'flex');
    this.renderer.setStyle(overlay, 'align-items', 'center');
    this.renderer.setStyle(overlay, 'justify-content', 'center');
    this.renderer.setStyle(overlay, 'z-index', '1000');
    this.renderer.setStyle(overlay, 'border-radius', 'inherit');

    // Create spinner
    const spinner = this.renderer.createElement('div');
    this.renderer.setStyle(spinner, 'border', `4px solid #f3f4f6`);
    this.renderer.setStyle(spinner, 'border-top', `4px solid ${this.loadingColor}`);
    this.renderer.setStyle(spinner, 'border-radius', '50%');
    this.renderer.setStyle(spinner, 'width', '40px');
    this.renderer.setStyle(spinner, 'height', '40px');
    this.renderer.setStyle(spinner, 'animation', 'spin 1s linear infinite');

    // Add animation
    if (!document.querySelector('style[data-loading-spinner]')) {
      const style = this.renderer.createElement('style');
      this.renderer.setAttribute(style, 'data-loading-spinner', '');
      const keyframes = `
        @keyframes spin {
          0% { transform: rotate(0deg); }
          100% { transform: rotate(360deg); }
        }
      `;
      this.renderer.appendChild(style, this.renderer.createText(keyframes));
      this.renderer.appendChild(document.head, style);
    }

    this.renderer.appendChild(overlay, spinner);
    this.renderer.appendChild(this.el.nativeElement, overlay);
    this.loadingOverlay = overlay;
  }

  private hideLoadingOverlay(): void {
    if (this.loadingOverlay) {
      this.renderer.removeChild(this.el.nativeElement, this.loadingOverlay);
      this.loadingOverlay = null;
    }
  }
}
