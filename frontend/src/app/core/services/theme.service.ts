import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

/**
 * Theme Service
 * Manages application theme (light/dark mode)
 */
@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly themeKey = 'app_theme';
  private isDarkModeSubject = new BehaviorSubject<boolean>(this.getStoredTheme());

  isDarkMode$ = this.isDarkModeSubject.asObservable();

  constructor() {
    this.initializeTheme();
  }

  /**
   * Initialize theme from storage or system preference
   */
  initializeTheme(): void {
    const isDark = this.getStoredTheme();
    this.applyTheme(isDark);
  }

  /**
   * Toggle dark mode
   */
  toggleDarkMode(): void {
    const isDark = !this.isDarkModeSubject.value;
    this.setTheme(isDark);
  }

  /**
   * Set theme
   */
  setTheme(isDark: boolean): void {
    localStorage.setItem(this.themeKey, isDark ? 'dark' : 'light');
    this.applyTheme(isDark);
    this.isDarkModeSubject.next(isDark);
  }

  /**
   * Get current theme
   */
  isDarkMode(): boolean {
    return this.isDarkModeSubject.value;
  }

  /**
   * Apply theme to document
   */
  private applyTheme(isDark: boolean): void {
    const htmlElement = document.documentElement;
    if (isDark) {
      htmlElement.classList.add('dark');
    } else {
      htmlElement.classList.remove('dark');
    }
  }

  /**
   * Get stored theme preference
   */
  private getStoredTheme(): boolean {
    const stored = localStorage.getItem(this.themeKey);
    if (stored) {
      return stored === 'dark';
    }

    // Check system preference
    return window.matchMedia('(prefers-color-scheme: dark)').matches;
  }
}
