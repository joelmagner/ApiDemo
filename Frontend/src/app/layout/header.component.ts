import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './header.component.html',
})
export class HeaderComponent {
  private userService = inject(UserService);
  private router = inject(Router);
  readonly currentUser = this.userService.currentUser;

  logout() {
    localStorage.clear();
    this.router.navigate(['/']);
  }
}
