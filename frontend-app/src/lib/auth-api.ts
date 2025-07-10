import { 
  LoginDto, 
  ChangePasswordDto, 
  CreateUserDto, 
  UpdateUserDto,
  AssignRoleDto,
  LoginResponse, 
  User, 
  Role, 
  Permission,
  RefreshTokenDto
} from "./auth-schema";
import { ApiErrorHandler } from "./error-handler";

// Use proxy for development, direct API for production
const API_BASE = process.env.NODE_ENV === 'development' 
  ? "/api/proxy"  // Use Next.js proxy to avoid CORS
  : process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000/api";

// Token management
export class TokenManager {
  private static readonly TOKEN_KEY = "auth_token";
  private static readonly REFRESH_TOKEN_KEY = "refresh_token";
  private static readonly USER_KEY = "auth_user";

  static setTokens(token: string, refreshToken: string) {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
  }

  static getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  static getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  static setUser(user: User) {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  static getUser(): User | null {
    const userData = localStorage.getItem(this.USER_KEY);
    return userData ? JSON.parse(userData) : null;
  }

  static clearTokens() {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
  }

  static isAuthenticated(): boolean {
    return !!this.getToken();
  }
}

// HTTP client with auth handling
class ApiClient {
  private async request<T>(
    endpoint: string, 
    options: RequestInit = {}
  ): Promise<T> {
    const token = TokenManager.getToken();
    
    const config: RequestInit = {
      ...options,
      headers: {
        "Content-Type": "application/json",
        ...(token && { Authorization: `Bearer ${token}` }),
        ...options.headers,
      },
    };

    const response = await fetch(`${API_BASE}${endpoint}`, config);

    if (response.status === 401) {
      // Try to refresh token
      const refreshToken = TokenManager.getRefreshToken();
      if (refreshToken) {
        try {
          const refreshResponse = await this.refreshToken({ refreshToken });
          TokenManager.setTokens(refreshResponse.token, refreshResponse.refreshToken);
          
          // Retry original request with new token
          const retryConfig = {
            ...config,
            headers: {
              ...config.headers,
              Authorization: `Bearer ${refreshResponse.token}`,
            },
          };
          
          const retryResponse = await fetch(`${API_BASE}${endpoint}`, retryConfig);
          if (!retryResponse.ok) {
            if (retryResponse.status === 401) {
              // Still unauthorized after refresh, redirect to login
              TokenManager.clearTokens();
              ApiErrorHandler.handleError(new Error("Session expired"));
              throw new Error("Session expired");
            }
            if (retryResponse.status === 403) {
              throw new Error("You don't have permission to access this resource");
            }
            throw new Error(`HTTP error! status: ${retryResponse.status}`);
          }
          return retryResponse.json();
        } catch {
          // Refresh failed, clear tokens and redirect to login
          TokenManager.clearTokens();
          ApiErrorHandler.handleError(new Error("Session expired"));
          throw new Error("Session expired");
        }
      } else {
        TokenManager.clearTokens();
        ApiErrorHandler.handleError(new Error("Authentication required"));
        throw new Error("Authentication required");
      }
    }

    if (response.status === 403) {
      throw new Error("You don't have permission to access this resource");
    }

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  }

  async get<T>(endpoint: string): Promise<T> {
    return this.request<T>(endpoint, { method: "GET" });
  }

  async post<T>(endpoint: string, data?: unknown): Promise<T> {
    return this.request<T>(endpoint, {
      method: "POST",
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  async put<T>(endpoint: string, data?: unknown): Promise<T> {
    return this.request<T>(endpoint, {
      method: "PUT",
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  async delete<T>(endpoint: string): Promise<T> {
    return this.request<T>(endpoint, { method: "DELETE" });
  }

  async refreshToken(data: RefreshTokenDto): Promise<LoginResponse> {
    // Don't use the auth-wrapped request for refresh
    const response = await fetch(`${API_BASE}/auth/refresh`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      throw new Error("Failed to refresh token");
    }

    return response.json();
  }
}

const apiClient = new ApiClient();

// Authentication API
export const authApi = {
  async login(data: LoginDto): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>("/auth/login", data);
    TokenManager.setTokens(response.token, response.refreshToken);
    TokenManager.setUser(response.user);
    return response;
  },

  async logout(): Promise<void> {
    try {
      await apiClient.post("/auth/logout");
    } finally {
      TokenManager.clearTokens();
    }
  },

  async changePassword(data: ChangePasswordDto): Promise<void> {
    return apiClient.put("/auth/change-password", data);
  },

  async refreshToken(data: RefreshTokenDto): Promise<LoginResponse> {
    const response = await apiClient.refreshToken(data);
    TokenManager.setTokens(response.token, response.refreshToken);
    TokenManager.setUser(response.user);
    return response;
  },

  getCurrentUser(): User | null {
    return TokenManager.getUser();
  },

  isAuthenticated(): boolean {
    return TokenManager.isAuthenticated();
  },
};

// User Management API
export const userApi = {
  async getUsers(): Promise<User[]> {
    return apiClient.get<User[]>("/users");
  },

  async getUserById(id: number): Promise<User> {
    return apiClient.get<User>(`/users/${id}`);
  },

  async createUser(data: CreateUserDto): Promise<User> {
    return apiClient.post<User>("/users", data);
  },

  async updateUser(id: number, data: UpdateUserDto): Promise<User> {
    return apiClient.put<User>(`/users/${id}`, data);
  },

  async deleteUser(id: number): Promise<void> {
    return apiClient.delete(`/users/${id}`);
  },

  async activateUser(id: number): Promise<void> {
    return apiClient.put(`/users/${id}`, { isActive: true });
  },

  async deactivateUser(id: number): Promise<void> {
    return apiClient.put(`/users/${id}`, { isActive: false });
  },

  async assignRoles(data: AssignRoleDto): Promise<void> {
    return apiClient.post(`/users/${data.userId}/assign-roles`, data);
  },

  async getUserPermissions(id: number): Promise<Permission[]> {
    return apiClient.get<Permission[]>(`/users/${id}/permissions`);
  },
};

// Role Management API
export const roleApi = {
  async getRoles(): Promise<Role[]> {
    return apiClient.get<Role[]>("/roles");
  },

  async getRoleById(id: number): Promise<Role> {
    return apiClient.get<Role>(`/roles/${id}`);
  },

  async createRole(data: { name: string; description?: string; permissionIds?: number[] }): Promise<Role> {
    return apiClient.post<Role>("/roles", data);
  },

  async updateRole(id: number, data: { name: string; description?: string; permissionIds?: number[] }): Promise<Role> {
    return apiClient.put<Role>(`/roles/${id}`, data);
  },

  async deleteRole(id: number): Promise<void> {
    return apiClient.delete(`/roles/${id}`);
  },

  async assignPermissions(roleId: number, permissionIds: number[]): Promise<void> {
    return apiClient.put(`/roles/${roleId}/permissions`, { permissionIds });
  },
};

// Permission API
export const permissionApi = {
  async getPermissions(): Promise<Permission[]> {
    return apiClient.get<Permission[]>("/permissions");
  },

  async getPermissionById(id: number): Promise<Permission> {
    return apiClient.get<Permission>(`/permissions/${id}`);
  },
};

// Export combined API
export const api = {
  auth: authApi,
  users: userApi,
  roles: roleApi,
  permissions: permissionApi,
};
