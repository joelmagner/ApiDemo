import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';

export abstract class BaseApi {
  protected readonly http = inject(HttpClient);
  protected apiUrl = environment.apiUrl;

  constructor(private basePath: string) {
    this.apiUrl = this.apiUrl + this.basePath;
  }

  protected get<T>(path: string) {
    return firstValueFrom(
      this.http.get<T>(this.getUrl(path), { withCredentials: true })
    );
  }

  protected post<T>(path: string, body: any) {
    return firstValueFrom(
      this.http.post<T>(this.getUrl(path), body, { withCredentials: true })
    );
  }

  protected put<T>(path: string, body: any) {
    return firstValueFrom(
      this.http.put<T>(this.getUrl(path), body, { withCredentials: true })
    );
  }

  protected delete<T>(path: string) {
    return firstValueFrom(
      this.http.delete<T>(this.getUrl(path), { withCredentials: true })
    );
  }

  protected getUrl = (url?: string) => `${this.apiUrl}${url ?? ''}`;
}
