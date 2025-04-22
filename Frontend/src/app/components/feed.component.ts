import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { Api } from '../services/api.service';
import { Photo } from '../services/api/user.dto';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-feed',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './feed.component.html',
})
export class FeedComponent implements OnInit {
  private api = inject(Api);
  private userService = inject(UserService);
  private router = inject(Router);

  photos: Photo[] = [];
  loading = true;
  error: string | null = null;

  activePhotoId: string | null = null;

  async ngOnInit() {
    try {
      await this.userService.fetchCurrentUser();
      await this.fetchPhotos();
    } catch (err) {
      console.log(err);
      this.error = 'Could not load user or photos.';
    } finally {
      this.loading = false;
    }
  }

  async fetchPhotos() {
    if (!this.userService.currentUser()) return;
    try {
      this.photos = await this.api.user.getPhotos(
        this.userService.currentUser()?.userId!
      );
    } catch (e) {
      this.error = 'Could not get photos';
      console.log('Error fetching photos', e);
    }
  }

  openContextMenu(photoId: string) {
    this.activePhotoId = this.activePhotoId === photoId ? null : photoId;
  }

  closeContextMenu() {
    this.activePhotoId = null;
  }

  async deletePhoto(photoId: string) {
    try {
      await this.api.user.deletePhoto(photoId);
    } catch (e) {
      this.error = 'Could not delete photo';
    }
  }

  async editPhoto(photoId: string) {
    try {
      await this.api.user.editPhoto(photoId);
    } catch (e) {
      this.error = 'Failed to edit photo details';
    }
  }

  likePhoto(photoId: string) {
    const photo = this.photos.find((p) => p.id === photoId);
    if (photo) {
      // todo: create backend url.
    }
  }

  getImageSrc(photo: Photo): string {
    return `data:${photo.contentType};base64,${photo.contents}`;
  }
}
