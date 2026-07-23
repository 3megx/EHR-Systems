import { Injectable } from '@angular/core';
import { environment } from '@env/environment';

export type LogLevel = 'debug' | 'info' | 'warn' | 'error';

export interface LogEntry {
  timestamp: Date;
  level: LogLevel;
  message: string;
  data?: any;
  stack?: string;
}

/**
 * Logger Service
 * Centralized logging with different severity levels
 */
@Injectable({
  providedIn: 'root',
})
export class LoggerService {
  private logs: LogEntry[] = [];
  private maxLogs = 1000;

  debug(message: string, data?: any): void {
    this.log('debug', message, data);
  }

  info(message: string, data?: any): void {
    this.log('info', message, data);
  }

  warn(message: string, data?: any): void {
    this.log('warn', message, data);
  }

  error(message: string, error?: any, data?: any): void {
    const errorData = {
      ...data,
      errorMessage: error?.message,
      errorStack: error?.stack,
    };
    this.log('error', message, errorData);
  }

  private log(level: LogLevel, message: string, data?: any): void {
    const logEntry: LogEntry = {
      timestamp: new Date(),
      level,
      message,
      data,
    };

    this.logs.push(logEntry);

    // Keep logs limited
    if (this.logs.length > this.maxLogs) {
      this.logs.shift();
    }

    // Console output in development
    if (!environment.production) {
      this.consoleLog(level, message, data);
    }
  }

  private consoleLog(level: LogLevel, message: string, data?: any): void {
    const timestamp = new Date().toLocaleTimeString();
    const prefix = `[${timestamp}] [${level.toUpperCase()}]`;

    switch (level) {
      case 'debug':
        console.debug(prefix, message, data);
        break;
      case 'info':
        console.info(prefix, message, data);
        break;
      case 'warn':
        console.warn(prefix, message, data);
        break;
      case 'error':
        console.error(prefix, message, data);
        break;
    }
  }

  /**
   * Get all logs
   */
  getLogs(): LogEntry[] {
    return [...this.logs];
  }

  /**
   * Get logs by level
   */
  getLogsByLevel(level: LogLevel): LogEntry[] {
    return this.logs.filter((log) => log.level === level);
  }

  /**
   * Get recent logs
   */
  getRecentLogs(count: number): LogEntry[] {
    return this.logs.slice(-count);
  }

  /**
   * Clear all logs
   */
  clearLogs(): void {
    this.logs = [];
  }

  /**
   * Export logs as JSON
   */
  exportLogs(): string {
    return JSON.stringify(this.logs, null, 2);
  }
}
