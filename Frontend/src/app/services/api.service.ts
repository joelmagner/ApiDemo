import { Injectable } from '@angular/core';
import { AuthRoutes } from './api/auth.api';
import { UploadRoutes } from './api/upload.api';
import { UserRoutes } from './api/user.api';

@Injectable({ providedIn: 'root' })
export class Api {
  constructor(
    public readonly auth: AuthRoutes,
    public readonly user: UserRoutes,
    public readonly upload: UploadRoutes
  ) {}
}
