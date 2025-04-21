import { Routes } from '@angular/router';
import { FeedComponent } from './components/feed.component';
import { LoginComponent } from './components/login.component';
import { RegisterComponent } from './components/register.component';
import { UploadComponent } from './components/upload.component';
import { HomeScreenComponent } from './home.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'feed', component: FeedComponent },
  { path: 'upload', component: UploadComponent },
  { path: '', component: HomeScreenComponent },
  { path: '**', component: LoginComponent },
];
