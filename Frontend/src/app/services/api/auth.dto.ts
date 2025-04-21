export interface LoginRequest {
  username?: string;
  email?: string;
  password: string;
}

export interface LoginResponse {
  access_token: string;
  message: string;
}

export interface LogoutRequest {
  userId: string;
}

export interface UploadPhotoRequest {
  contentType: string;
  description: string;
  contents: string;
}

export interface CommentPhotoRequest {
  photoId: string;
  userId: string;
  comment: string;
}

export interface User {
  id: string;
  username: string;
  email: string;
  createdAt: string;
}
