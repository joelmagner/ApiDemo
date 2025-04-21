import { Injectable } from '@angular/core';
import { map } from 'rxjs';
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
    return this.post('/create', payload).pipe(map((res) => res ?? null));
  }

  getPhotos(userId: string) {
    return this.get<Photo[]>(`/photos/${userId}`).pipe(
      map((res) => res ?? null)
    );
  }

  editPhoto(photoId: string) {
    return this.put<Photo>(`/photo/edit/${photoId}`, {}).pipe(
      map((res) => res ?? null)
    );
  }

  deletePhoto(photoId: string) {
    return this.delete<Photo>(`/photo/delete/${photoId}`).pipe(
      map((res) => res ?? null)
    );
  }
}
