import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export abstract class BaseApi {
  protected readonly http = inject(HttpClient);
  protected apiUrl = environment.apiUrl;

  constructor(private basePath: string) {
    this.apiUrl = this.apiUrl + this.basePath;
  }

  protected get<T>(path: string, params?: any): Observable<T> {
    return this.http.get<T>(this.getUrl(path), {
      params,
      withCredentials: true,
    });
  }

  protected post<T>(path: string, body: any): Observable<T> {
    return this.http.post<T>(this.getUrl(path), body, {
      withCredentials: true,
    });
  }

  protected put<T>(path: string, body: any): Observable<T> {
    return this.http.put<T>(this.getUrl(path), body, {
      withCredentials: true,
    });
  }

  protected delete<T>(path: string): Observable<T> {
    return this.http.delete<T>(this.getUrl(path), {
      withCredentials: true,
    });
  }

  protected getUrl = (url?: string) => `${this.apiUrl}${url ?? ''}`;
}
