import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@env/environment';

export type AuditAction = 'create' | 'read' | 'update' | 'delete' | 'export' | 'print' | 'login' | 'logout';

export interface AuditLog {
  id?: string;
  userId: string;
  userName: string;
  action: AuditAction;
  resourceType: string;
  resourceId: string;
  resourceName: string;
  oldValue?: any;
  newValue?: any;
  status: 'success' | 'failure';
  failureReason?: string;
  ipAddress?: string;
  userAgent?: string;
  timestamp: Date;
  notes?: string;
}

/**
 * Audit Log Service
 * Tracks user actions for HIPAA compliance
 */
@Injectable({
  providedIn: 'root',
})
export class AuditLogService {
  private apiUrl = `${environment.apiUrl}/audit-logs`;
  private localLogs: AuditLog[] = [];

  constructor(private http: HttpClient) {
    this.loadLocalLogs();
  }

  /**
   * Log an action
   */
  logAction(log: Omit<AuditLog, 'id' | 'timestamp'>): void {
    const auditLog: AuditLog = {
      ...log,
      timestamp: new Date(),
      ipAddress: this.getClientIP(),
      userAgent: navigator.userAgent,
    };

    // Store locally
    this.localLogs.push(auditLog);
    this.saveLocalLogs();

    // Send to server
    if (!environment.production) {
      console.log('[AUDIT LOG]', auditLog);
    }
    // Uncomment when backend is ready
    // this.http.post(this.apiUrl, auditLog).subscribe(
    //   () => console.log('Audit log sent'),
    //   (error) => console.error('Failed to send audit log', error)
    // );
  }

  /**
   * Log patient access
   */
  logPatientAccess(userId: string, userName: string, patientId: string, patientName: string): void {
    this.logAction({
      userId,
      userName,
      action: 'read',
      resourceType: 'patient',
      resourceId: patientId,
      resourceName: patientName,
      status: 'success',
    });
  }

  /**
   * Log patient record modification
   */
  logPatientModification(
    userId: string,
    userName: string,
    patientId: string,
    patientName: string,
    oldValue: any,
    newValue: any
  ): void {
    this.logAction({
      userId,
      userName,
      action: 'update',
      resourceType: 'patient',
      resourceId: patientId,
      resourceName: patientName,
      oldValue,
      newValue,
      status: 'success',
    });
  }

  /**
   * Log data export
   */
  logDataExport(userId: string, userName: string, dataType: string, recordCount: number): void {
    this.logAction({
      userId,
      userName,
      action: 'export',
      resourceType: dataType,
      resourceId: 'export-' + Date.now(),
      resourceName: `${dataType} export (${recordCount} records)`,
      status: 'success',
      notes: `Exported ${recordCount} ${dataType} records`,
    });
  }

  /**
   * Log document print
   */
  logDocumentPrint(userId: string, userName: string, documentType: string, documentId: string): void {
    this.logAction({
      userId,
      userName,
      action: 'print',
      resourceType: documentType,
      resourceId: documentId,
      resourceName: `${documentType} print`,
      status: 'success',
    });
  }

  /**
   * Log login
   */
  logLogin(userId: string, userName: string): void {
    this.logAction({
      userId,
      userName,
      action: 'login',
      resourceType: 'auth',
      resourceId: userId,
      resourceName: 'User Login',
      status: 'success',
    });
  }

  /**
   * Log logout
   */
  logLogout(userId: string, userName: string): void {
    this.logAction({
      userId,
      userName,
      action: 'logout',
      resourceType: 'auth',
      resourceId: userId,
      resourceName: 'User Logout',
      status: 'success',
    });
  }

  /**
   * Log failed action
   */
  logFailedAction(
    userId: string,
    userName: string,
    action: AuditAction,
    resourceType: string,
    resourceId: string,
    reason: string
  ): void {
    this.logAction({
      userId,
      userName,
      action,
      resourceType,
      resourceId,
      resourceName: `Failed: ${resourceType}`,
      status: 'failure',
      failureReason: reason,
    });
  }

  /**
   * Get all audit logs
   */
  getLogs(): AuditLog[] {
    return [...this.localLogs];
  }

  /**
   * Get audit logs by user
   */
  getLogsByUser(userId: string): AuditLog[] {
    return this.localLogs.filter((log) => log.userId === userId);
  }

  /**
   * Get audit logs by resource
   */
  getLogsByResource(resourceId: string): AuditLog[] {
    return this.localLogs.filter((log) => log.resourceId === resourceId);
  }

  /**
   * Get audit logs by date range
   */
  getLogsByDateRange(startDate: Date, endDate: Date): AuditLog[] {
    return this.localLogs.filter(
      (log) => log.timestamp >= startDate && log.timestamp <= endDate
    );
  }

  /**
   * Get audit logs by action
   */
  getLogsByAction(action: AuditAction): AuditLog[] {
    return this.localLogs.filter((log) => log.action === action);
  }

  /**
   * Clear old logs (retention policy)
   */
  clearOldLogs(daysToKeep: number = 90): void {
    const cutoffDate = new Date();
    cutoffDate.setDate(cutoffDate.getDate() - daysToKeep);

    this.localLogs = this.localLogs.filter((log) => log.timestamp > cutoffDate);
    this.saveLocalLogs();
  }

  /**
   * Export logs as CSV
   */
  exportLogsAsCSV(): string {
    const headers = [
      'Date',
      'Time',
      'User ID',
      'User Name',
      'Action',
      'Resource Type',
      'Resource ID',
      'Resource Name',
      'Status',
    ];

    const rows = this.localLogs.map((log) => [
      log.timestamp.toLocaleDateString(),
      log.timestamp.toLocaleTimeString(),
      log.userId,
      log.userName,
      log.action,
      log.resourceType,
      log.resourceId,
      log.resourceName,
      log.status,
    ]);

    const csv = [headers, ...rows].map((row) => row.map((cell) => `"${cell}"`).join(',')).join('\n');
    return csv;
  }

  private saveLocalLogs(): void {
    try {
      localStorage.setItem('audit_logs', JSON.stringify(this.localLogs));
    } catch (error) {
      console.warn('Failed to save audit logs to localStorage', error);
    }
  }

  private loadLocalLogs(): void {
    try {
      const stored = localStorage.getItem('audit_logs');
      if (stored) {
        this.localLogs = JSON.parse(stored);
      }
    } catch (error) {
      console.warn('Failed to load audit logs from localStorage', error);
    }
  }

  private getClientIP(): string {
    // This would need to be retrieved from backend in production
    return 'Unknown';
  }
}
