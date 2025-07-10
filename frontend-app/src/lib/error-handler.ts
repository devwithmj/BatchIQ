import { toast } from "sonner";
import { AppRouterInstance } from "next/dist/shared/lib/app-router-context.shared-runtime";

// Global error handler for API responses
export class ApiErrorHandler {
  private static router: AppRouterInstance | null = null;

  // Set router instance (should be called from a component)
  static setRouter(router: AppRouterInstance) {
    this.router = router;
  }

  // Handle different types of errors
  static handleError(error: Error | unknown, context?: string) {
    const message = error instanceof Error ? error.message : "An unexpected error occurred";
    
    // Check for specific error messages
    if (message === "Session expired" || message === "Authentication required") {
      toast.error("Session expired. Please login again.");
      this.redirectToLogin();
      return;
    }
    
    if (message.includes("You don't have permission")) {
      toast.error("You don't have permission to access this resource.");
      return;
    }

    // Generic error handling
    if (context) {
      toast.error(`${context}: ${message}`);
    } else {
      toast.error(message);
    }
  }

  // Safe redirect to login
  private static redirectToLogin() {
    if (typeof window !== "undefined") {
      if (this.router) {
        this.router.push("/login");
      } else {
        window.location.href = "/login";
      }
    }
  }

  // Handle HTTP status codes
  static handleHttpError(status: number, message?: string) {
    switch (status) {
      case 401:
        this.handleError(new Error("Session expired"));
        break;
      case 403:
        this.handleError(new Error("You don't have permission to access this resource"));
        break;
      case 404:
        toast.error(message || "Resource not found");
        break;
      case 500:
        toast.error(message || "Server error. Please try again later.");
        break;
      default:
        toast.error(message || `Error: ${status}`);
    }
  }
}
