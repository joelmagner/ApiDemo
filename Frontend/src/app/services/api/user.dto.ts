export type MeResponse = {
  email: string;
  fullName: string;
  userId: string;
  userName: string;
};

export interface CreateUserRequest {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface GetUserByUsernameRequest {
  username: string;
}

export interface Photo {
  id: string;
  userId: string;
  votes: number;
  contents: string;
  contentType: string;
  description?: string;
  createdAt: string;
  updatedAt?: string;
  isDeleted: boolean;
}
