"use client";

import React, { createContext, useContext, useEffect, useState, ReactNode } from "react";
import { useRouter } from "next/navigation";
import { User, Role } from "@/lib/auth-schema";
import { authApi } from "@/lib/auth-api";
import { ApiErrorHandler } from "@/lib/error-handler";

interface AuthContextType {
  user: User | null;
  roles: Role[];
  permissions: string[];
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (username: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  refreshUser: () => Promise<void>;
  hasPermission: (permission: string) => boolean;
  hasRole: (roleName: string) => boolean;
  hasAnyPermission: (permissions: string[]) => boolean;
  hasAllPermissions: (permissions: string[]) => boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<User | null>(null);
  const [roles, setRoles] = useState<Role[]>([]);
  const [permissions, setPermissions] = useState<string[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const router = useRouter();

  const isAuthenticated = !!user;

  // Set router in error handler
  useEffect(() => {
    ApiErrorHandler.setRouter(router);
  }, [router]);

  // Initialize auth state on mount
  useEffect(() => {
    const initAuth = async () => {
      try {
        if (authApi.isAuthenticated()) {
          const currentUser = authApi.getCurrentUser();
          if (currentUser) {
            setUser(currentUser);
            setRoles(currentUser.roles || []);
            
            // Extract permissions from roles
            const allPermissions = currentUser.roles?.flatMap(role => 
              role.permissions?.map(p => p.name) || []
            ) || [];
            setPermissions([...new Set(allPermissions)]);
          }
        }
      } catch (error) {
        console.error("Failed to initialize auth:", error);
        // Clear invalid tokens
        await logout();
      } finally {
        setIsLoading(false);
      }
    };

    initAuth();
  }, []);

  const login = async (username: string, password: string) => {
    try {
      const response = await authApi.login({ username, password });
      setUser(response.user);
      setRoles(response.user.roles || []);
      setPermissions(response.permissions);        } catch (error) {
          ApiErrorHandler.handleError(error, "Login failed");
          throw error; // Re-throw to allow components to handle
        }
  };

  const logout = async () => {
    try {
      await authApi.logout();
    } catch (error) {
      console.error("Logout error:", error);
    } finally {
      setUser(null);
      setRoles([]);
      setPermissions([]);
    }
  };

  const refreshUser = async () => {
    try {
      if (authApi.isAuthenticated()) {
        const currentUser = authApi.getCurrentUser();
        if (currentUser) {
          setUser(currentUser);
          setRoles(currentUser.roles || []);
          
          const allPermissions = currentUser.roles?.flatMap(role => 
            role.permissions?.map(p => p.name) || []
          ) || [];
          setPermissions([...new Set(allPermissions)]);
        }
      }
    } catch (error) {
      console.error("Failed to refresh user:", error);
    }
  };

  const hasPermission = (permission: string): boolean => {
    return permissions.includes(permission);
  };

  const hasRole = (roleName: string): boolean => {
    return roles.some(role => role.name === roleName);
  };

  const hasAnyPermission = (requiredPermissions: string[]): boolean => {
    return requiredPermissions.some(permission => permissions.includes(permission));
  };

  const hasAllPermissions = (requiredPermissions: string[]): boolean => {
    return requiredPermissions.every(permission => permissions.includes(permission));
  };

  const value: AuthContextType = {
    user,
    roles,
    permissions,
    isAuthenticated,
    isLoading,
    login,
    logout,
    refreshUser,
    hasPermission,
    hasRole,
    hasAnyPermission,
    hasAllPermissions,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}

// Higher-order component for protected routes
export function withAuth<P extends object>(
  Component: React.ComponentType<P>,
  requiredPermissions?: string[]
) {
  return function AuthenticatedComponent(props: P) {
    const { isAuthenticated, isLoading, hasAnyPermission } = useAuth();

    if (isLoading) {
      return <div>Loading...</div>; // Replace with your loading component
    }

    if (!isAuthenticated) {
      // Redirect to login or show unauthorized
      return <div>Unauthorized - Please log in</div>;
    }

    if (requiredPermissions && !hasAnyPermission(requiredPermissions)) {
      return <div>Insufficient permissions</div>;
    }

    return <Component {...props} />;
  };
}

// Component for conditional rendering based on permissions
interface PermissionGateProps {
  children: ReactNode;
  permissions?: string[];
  roles?: string[];
  requireAll?: boolean; // If true, requires all permissions/roles; if false, requires any
  fallback?: ReactNode;
}

export function PermissionGate({
  children,
  permissions = [],
  roles = [],
  requireAll = false,
  fallback = null,
}: PermissionGateProps) {
  const { hasRole, hasAnyPermission, hasAllPermissions } = useAuth();

  const hasRequiredPermissions = () => {
    if (permissions.length === 0 && roles.length === 0) return true;
    
    const permissionCheck = permissions.length === 0 ? true : 
      requireAll ? hasAllPermissions(permissions) : hasAnyPermission(permissions);
    
    const roleCheck = roles.length === 0 ? true :
      requireAll ? roles.every(role => hasRole(role)) : roles.some(role => hasRole(role));
    
    return requireAll ? (permissionCheck && roleCheck) : (permissionCheck || roleCheck);
  };

  return hasRequiredPermissions() ? <>{children}</> : <>{fallback}</>;
}
