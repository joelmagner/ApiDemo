import {
  EnvironmentInjector,
  inject,
  Injectable,
  runInInjectionContext,
  signal,
} from '@angular/core';
import { toObservable } from '@angular/core/rxjs-interop';
import { filter, firstValueFrom } from 'rxjs';
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

  fetchCurrentUser() {
    const isLoggedIn = localStorage.getItem('access_token');
    if (!isLoggedIn) return;
    this.api.auth.getMe().subscribe({
      next: (user) => this._currentUser.set(user),
      error: () => this._currentUser.set(null),
    });
  }

  waitForUser(): Promise<MeResponse> {
    return runInInjectionContext(this.injector, () =>
      firstValueFrom(
        toObservable(this._currentUser).pipe(
          filter((user): user is MeResponse => user !== null)
        )
      )
    );
  }
}
