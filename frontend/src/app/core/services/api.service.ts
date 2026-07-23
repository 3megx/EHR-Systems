import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';

/**
 * API Service
 * Base service for all HTTP requests
 */
@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /**
   * GET request
   */
  get<T>(
    endpoint: string,
    params?: HttpParams | Record<string, string | number | boolean>
  ): Observable<T> {
    return this.http.get<T>(`${this.apiUrl}${endpoint}`, { params });
  }

  /**
   * POST request
   */
  post<T>(
    endpoint: string,
    body: any,
    params?: HttpParams | Record<string, string | number | boolean>
  ): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}${endpoint}`, body, { params });
  }

  /**
   * PUT request
   */
  put<T>(
    endpoint: string,
    body: any,
    params?: HttpParams | Record<string, string | number | boolean>
  ): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}${endpoint}`, body, { params });
  }

  /**
   * PATCH request
   */
  patch<T>(
    endpoint: string,
    body: any,
    params?: HttpParams | Record<string, string | number | boolean>
  ): Observable<T> {
    return this.http.patch<T>(`${this.apiUrl}${endpoint}`, body, { params });
  }

  /**
   * DELETE request
   */
  delete<T>(
    endpoint: string,
    params?: HttpParams | Record<string, string | number | boolean>
  ): Observable<T> {
    return this.http.delete<T>(`${this.apiUrl}${endpoint}`, { params });
  }

  /**
   * Upload file
   */
  uploadFile(endpoint: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(`${this.apiUrl}${endpoint}`, formData);
  }
}
