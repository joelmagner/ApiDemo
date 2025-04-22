import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

import { Api } from '../services/api.service';
import { LoginRequest } from '../services/api/auth.dto';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterModule, CommonModule],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  form: FormGroup;
  submitting = signal(false);
  errorMessage = signal<string | null>(null);

  constructor(
    private fb: FormBuilder,
    private api: Api,
    private router: Router
  ) {
    this.form = this.fb.group({
      identifier: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  async onSubmit() {
    if (this.form.invalid) return;

    this.submitting.set(true);
    this.errorMessage.set(null);

    const { identifier, password } = this.form.value;

    const payload: LoginRequest = {
      password,
      ...(identifier?.includes('@')
        ? { email: identifier }
        : { username: identifier }),
    };

    try {
      const login = await this.api.auth.login(payload);
      if (login?.access_token) {
        console.log('login successfull', login.access_token);
        localStorage.setItem('access_token', login.access_token);
        this.router.navigate(['/feed']);
      } else {
        this.errorMessage.set('Invalid login credentials.');
      }

      this.submitting.set(false);
    } catch (e) {
      this.errorMessage.set('Something went wrong. Please try again.');
      this.submitting.set(false);
    }
  }
}
