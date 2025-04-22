import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { Api } from '../services/api.service';
import { UploadPhotoRequest } from '../services/api/auth.dto';

@Component({
  selector: 'app-upload',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './upload.component.html',
})
export class UploadComponent {
  form: FormGroup;
  previewUrl = signal<string | null>(null);
  submitting = signal(false);

  constructor(
    private fb: FormBuilder,
    private api: Api,
    private router: Router
  ) {
    this.form = this.fb.group({
      image: [null as File | null, Validators.required],
      comment: ['', Validators.required],
    });
  }

  onFileChange(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      this.form.patchValue({ image: file });

      const reader = new FileReader();
      reader.onload = () => {
        this.previewUrl.set(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  }

  async onSubmit() {
    if (this.form.invalid) return;

    this.submitting.set(true);
    const file = this.form.value.image!;
    const comment = this.form.value.comment!;
    const arrayBuffer = await file.arrayBuffer();
    const byteArray = new Uint8Array(arrayBuffer);

    const payload: UploadPhotoRequest = {
      contentType: file.type,
      contents: Array.from(byteArray) as any,
      description: comment,
    };

    try {
      await this.api.upload.photos(payload);
      this.router.navigate(['/feed']);
    } catch (e) {
      alert('Upload failed.');
    } finally {
      this.submitting.set(false);
    }
  }
}
