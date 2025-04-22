import { Injectable } from '@angular/core';
import { LoginRequest, LoginResponse, LogoutRequest } from './auth.dto';
import { BaseApi } from './base.api';
import { MeResponse } from './user.dto';

@Injectable({
  providedIn: 'root',
})
export class AuthRoutes extends BaseApi {
  constructor() {
    super('/auth');
  }

  login(payload: LoginRequest) {
    return this.post<LoginResponse>('/login', payload);
  }

  logout(payload: LogoutRequest) {
    return this.post<string>('/logout', payload);
  }

  getMe() {
    return this.get<MeResponse>('/me');
  }
}
