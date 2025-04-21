import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MeResponse } from './services/api/user.dto';
import { UserService } from './services/user.service';

@Component({
  selector: 'home-screen',
  standalone: true,
  templateUrl: './home.component.html',
})
export class HomeScreenComponent implements OnInit {
  private userService = inject(UserService);
  currentUser: MeResponse | null = null;

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.currentUser = this.userService.currentUser();
    if (this.currentUser?.userId) {
      this.router.navigate(['/feed']);
    }
  }

  navigateToLogin(): void {
    this.router.navigate(['/login']);
  }
}
