import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { catchError, of } from 'rxjs';

import { Api } from '../services/api.service';
import { LoginRequest } from '../services/api/auth.dto';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterModule, CommonModule],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private api = inject(Api);
  private router = inject(Router);
  private userService = inject(UserService);

  readonly form = this.fb.group({
    identifier: ['', Validators.required],
    password: ['', Validators.required],
  });

  submitting = signal(false);
  errorMessage = signal<string | null>(null);

  get isFormValid() {
    return this.form.valid;
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.submitting.set(true);
    this.errorMessage.set(null);

    const { identifier, password } = this.form.value;

    const payload: LoginRequest = {
      password: password!,
    };

    if (identifier?.includes('@')) {
      payload.email = identifier;
    } else {
      payload.username = identifier!;
    }

    this.api.auth
      .login(payload)
      .pipe(
        catchError((err) => {
          this.errorMessage.set('Something went wrong. Please try again.');
          this.submitting.set(false);
          return of(null);
        })
      )
      .subscribe((result) => {
        if (!result?.access_token) {
          this.errorMessage.set('Invalid login credentials.');
        } else {
          const token = result.access_token;
          localStorage.setItem('access_token', token);
          this.userService.fetchCurrentUser();
          this.router.navigate(['/feed']);
        }

        this.submitting.set(false);
      });
  }
}
