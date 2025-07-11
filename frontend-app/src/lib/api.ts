import axios from "axios";
import { ApiErrorHandler } from "./error-handler";
import { TokenManager } from "./auth-api";

// Use proxy for development, direct API for production
const baseURL = process.env.NODE_ENV === 'development' 
  ? "/api/proxy"  // Use Next.js proxy to avoid CORS
  : process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

export const api = axios.create({
  baseURL,
  withCredentials: false,
});

// Add request interceptor to include auth headers
api.interceptors.request.use(
  (config) => {
    const token = TokenManager.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Add response interceptor to handle errors and token refresh
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    
    if (error.response) {
      const { status, data } = error.response;
      
      if (status === 401 && !originalRequest._retry) {
        originalRequest._retry = true;
        
        // Try to refresh token
        const refreshToken = TokenManager.getRefreshToken();
        if (refreshToken) {
          try {
            // Use a simple fetch to avoid circular dependency
            const response = await fetch(`${baseURL}/auth/refresh`, {
              method: "POST",
              headers: { "Content-Type": "application/json" },
              body: JSON.stringify({ refreshToken }),
            });

            if (response.ok) {
              const refreshResponse = await response.json();
              TokenManager.setTokens(refreshResponse.token, refreshResponse.refreshToken);
              TokenManager.setUser(refreshResponse.user);
              
              // Retry original request with new token
              originalRequest.headers.Authorization = `Bearer ${refreshResponse.token}`;
              return api(originalRequest);
            }
          } catch {
            // Refresh failed
          }
        }
        
        // Clear tokens and handle auth error
        TokenManager.clearTokens();
        ApiErrorHandler.handleError(new Error("Session expired"));
      } else if (status === 403) {
        ApiErrorHandler.handleError(new Error("You don't have permission to access this resource"));
      } else {
        ApiErrorHandler.handleError(new Error(data?.message || error.message));
      }
    } else if (error.request) {
      ApiErrorHandler.handleError(new Error("Network error. Please check your connection."));
    } else {
      ApiErrorHandler.handleError(error);
    }
    
    return Promise.reject(error);
  }
);
