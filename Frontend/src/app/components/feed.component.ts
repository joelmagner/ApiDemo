import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { Api } from '../services/api.service';
import { MeResponse, Photo } from '../services/api/user.dto';
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

  currentUser: MeResponse | null = null;
  activePhotoId: string | null = null;

  async ngOnInit() {
    try {
      this.currentUser = await this.userService.waitForUser();
      await this.fetchPhotos();
    } catch (err) {
      console.log(err);
      this.error = 'Could not load user or photos.';
    } finally {
      this.loading = false;
    }
  }

  async fetchPhotos() {
    if (!this.currentUser) return;
    this.api.user.getPhotos(this.currentUser.userId).subscribe({
      next: (data) => {
        this.photos = data;
      },
      error: () => {
        this.error = 'Failed to load photos.';
      },
    });
  }

  openContextMenu(photoId: string) {
    this.activePhotoId = this.activePhotoId === photoId ? null : photoId;
  }

  closeContextMenu() {
    this.activePhotoId = null;
  }

  deletePhoto(photoId: string) {
    this.api.user.deletePhoto(photoId).subscribe({
      next: () => {
        this.photos = this.photos.filter((photo) => photo.id !== photoId);
      },
      error: () => {
        this.error = 'Failed to delete photo';
      },
    });
  }

  editPhoto(photoId: string) {
    this.api.user.editPhoto(photoId).subscribe({
      next: () => {
        this.photos = this.photos.filter((photo) => photo.id !== photoId);
      },
      error: () => {
        this.error = 'Failed to edit photo details';
      },
    });
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
