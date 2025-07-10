import axios from "axios";
import { ApiErrorHandler } from "./error-handler";

// Use proxy for development, direct API for production
const baseURL = process.env.NODE_ENV === 'development' 
  ? "/api/proxy"  // Use Next.js proxy to avoid CORS
  : process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

export const api = axios.create({
  baseURL,
  withCredentials: false,
});

// Add response interceptor to handle errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      // Handle specific HTTP status codes
      const { status, data } = error.response;
      
      if (status === 401) {
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
