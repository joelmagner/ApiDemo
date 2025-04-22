import { Injectable } from '@angular/core';
import { UploadPhotoRequest } from './auth.dto';
import { BaseApi } from './base.api';

@Injectable({
  providedIn: 'root',
})
export class UploadRoutes extends BaseApi {
  constructor() {
    super('/upload');
  }

  photos(payload: UploadPhotoRequest) {
    return this.post<UploadPhotoRequest>('/photo', payload);
  }
}
