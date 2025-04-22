import { Injectable } from '@angular/core';
import { BaseApi } from './base.api';
import { CreateUserRequest, Photo } from './user.dto';

@Injectable({
  providedIn: 'root',
})
export class UserRoutes extends BaseApi {
  constructor() {
    super('/user');
  }

  createUser(payload: CreateUserRequest) {
    return this.post('/create', payload);
  }

  getPhotos(userId: string) {
    return this.get<Photo[]>(`/photos/${userId}`);
  }

  editPhoto(photoId: string) {
    return this.put<Photo>(`/photo/edit/${photoId}`, {});
  }

  deletePhoto(photoId: string) {
    return this.delete<Photo>(`/photo/delete/${photoId}`);
  }
}
