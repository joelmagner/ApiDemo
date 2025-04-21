import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Api } from '../services/api.service';
import { CreateUserRequest } from '../services/api/user.dto';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterModule, CommonModule],
  templateUrl: './register.component.html',
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private api = inject(Api);
  private router = inject(Router);

  readonly form = this.fb.group({
    username: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
  });

  submitting = false;
  errorMessage: string | null = null;

  onSubmit() {
    if (this.form.invalid) return;

    this.submitting = true;
    this.errorMessage = null;

    const payload = this.form.value as CreateUserRequest;

    this.api.user.createUser(payload).subscribe({
      next: (data) => {
        this.submitting = false;
        console.log('User registered:', data);
        this.router.navigate(['/login']);
      },
      error: () => {
        this.errorMessage = 'Failed to register.';
        this.submitting = false;
      },
    });
  }
}
