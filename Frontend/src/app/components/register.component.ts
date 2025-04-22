import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
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
  form: FormGroup;
  submitting = false;
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private api: Api,
    private router: Router
  ) {
    this.form = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.submitting = true;
    this.errorMessage = null;

    const payload = this.form.value as CreateUserRequest;

    try {
      const createdUser = this.api.user.createUser(payload);
      this.submitting = false;
      console.log('User created:', createdUser);
      this.router.navigate(['/login']);
    } catch (e) {
      this.errorMessage = 'Failed to register.';
      this.submitting = false;
    }
  }
}
