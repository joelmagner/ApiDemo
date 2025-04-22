import { EnvironmentInjector, inject, Injectable, signal } from '@angular/core';
import { Api } from './api.service';
import { MeResponse } from './api/user.dto';

@Injectable({ providedIn: 'root' })
export class UserService {
  private api = inject(Api);
  private injector = inject(EnvironmentInjector);
  private readonly _currentUser = signal<MeResponse | null>(null);

  constructor() {
    this.fetchCurrentUser();
  }

  get currentUser() {
    return this._currentUser;
  }

  async fetchCurrentUser() {
    const isLoggedIn = localStorage.getItem('access_token');
    if (!isLoggedIn || this._currentUser()?.userId) return;
    try {
      const user = await this.api.auth.getMe();
      this._currentUser.set(user);
    } catch (e) {
      this._currentUser.set(null);
      localStorage.clear();
    }
  }
}
