export interface AuthResponse {
    accessToken: string;
    expiresAtUtc: string;
    email: string;
  }
  
  export interface MeResponse {
    userId: string;
    email: string;
  }
  
  export interface LoginCredentials {
    email: string;
    password: string;
  }