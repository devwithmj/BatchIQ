import axios from "axios";

// Use proxy for development, direct API for production
const baseURL = process.env.NODE_ENV === 'development' 
  ? "/api/proxy"  // Use Next.js proxy to avoid CORS
  : process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

export const api = axios.create({
  baseURL,
  withCredentials: false,
});
