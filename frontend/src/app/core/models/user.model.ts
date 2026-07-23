/**
 * User Model
 * Represents authenticated user information
 */
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phone?: string;
  avatar?: string;
  roles: Role[];
  permissions: Permission[];
  isActive: boolean;
  lastLogin?: Date;
  createdAt: Date;
  updatedAt: Date;
}

/**
 * Role Model
 * Defines user roles for RBAC
 */
export interface Role {
  id: string;
  name: string;
  description: string;
  permissions: Permission[];
  isActive: boolean;
}

/**
 * Permission Model
 * Defines granular permissions
 */
export interface Permission {
  id: string;
  name: string;
  resource: string;
  action: string; // 'create', 'read', 'update', 'delete'
  description: string;
}

/**
 * AuthToken Response
 */
export interface AuthTokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  tokenType: string;
}

/**
 * Login Request
 */
export interface LoginRequest {
  email: string;
  password: string;
  rememberMe?: boolean;
}

/**
 * Login Response
 */
export interface LoginResponse {
  user: User;
  token: AuthTokenResponse;
}
