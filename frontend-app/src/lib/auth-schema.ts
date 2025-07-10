import * as z from "zod";
import { zodResolver } from "@hookform/resolvers/zod";

// Auth DTOs based on swagger.json
export const loginSchema = z.object({
  username: z.string().min(1, "Username is required"),
  password: z.string().min(1, "Password is required"),
});

export const changePasswordSchema = z.object({
  currentPassword: z.string().min(1, "Current password is required"),
  newPassword: z.string().min(6, "Password must be at least 6 characters"),
});

export const createUserSchema = z.object({
  username: z.string().min(3, "Username must be at least 3 characters"),
  email: z.string().email("Valid email is required"),
  password: z.string().min(6, "Password must be at least 6 characters"),
  firstName: z.string().min(1, "First name is required"),
  lastName: z.string().min(1, "Last name is required"),
  firstNameFa: z.string().optional(),
  lastNameFa: z.string().optional(),
  roleIds: z.array(z.number()).optional(),
});

export const updateUserSchema = z.object({
  firstName: z.string().min(1, "First name is required"),
  lastName: z.string().min(1, "Last name is required"),
  firstNameFa: z.string().optional(),
  lastNameFa: z.string().optional(),
  isActive: z.boolean().optional(),
  roleIds: z.array(z.number()).optional(),
});

export const assignRoleSchema = z.object({
  userId: z.number(),
  roleIds: z.array(z.number()),
});

// Type definitions based on swagger schemas
export type LoginDto = z.infer<typeof loginSchema>;
export type ChangePasswordDto = z.infer<typeof changePasswordSchema>;
export type CreateUserDto = z.infer<typeof createUserSchema>;
export type UpdateUserDto = z.infer<typeof updateUserSchema>;
export type AssignRoleDto = z.infer<typeof assignRoleSchema>;

export interface Permission {
  id: number;
  name: string;
  description?: string;
  descriptionFa?: string;
  category?: string;
  isActive: boolean;
}

export interface Role {
  id: number;
  name: string;
  description?: string;
  descriptionFa?: string;
  isActive: boolean;
  permissions?: Permission[];
}

export interface User {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  firstNameFa?: string;
  lastNameFa?: string;
  isActive: boolean;
  emailConfirmed: boolean;
  createdAt: string;
  lastLoginAt?: string;
  roles?: Role[];
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  user: User;
  roles: string[];
  permissions: string[];
}

export interface RefreshTokenDto {
  refreshToken: string;
}

// Resolvers for forms
export const loginResolver = zodResolver(loginSchema);
export const changePasswordResolver = zodResolver(changePasswordSchema);
export const createUserResolver = zodResolver(createUserSchema);
export const updateUserResolver = zodResolver(updateUserSchema);
export const assignRoleResolver = zodResolver(assignRoleSchema);

// Permission categories for organization
export const PERMISSION_CATEGORIES = {
  USER_MANAGEMENT: "User Management",
  PRODUCT_MANAGEMENT: "Product Management", 
  INVENTORY_MANAGEMENT: "Inventory Management",
  MANUFACTURING: "Manufacturing",
  REPORTING: "Reporting",
  SYSTEM_ADMIN: "System Administration",
} as const;

// Common permission names
export const PERMISSIONS = {
  // User Management
  VIEW_USERS: "view_users",
  CREATE_USERS: "create_users", 
  EDIT_USERS: "edit_users",
  DELETE_USERS: "delete_users",
  ASSIGN_ROLES: "assign_roles",
  
  // Product Management
  VIEW_PRODUCTS: "view_products",
  CREATE_PRODUCTS: "create_products",
  EDIT_PRODUCTS: "edit_products", 
  DELETE_PRODUCTS: "delete_products",
  
  // Manufacturing
  VIEW_BOM: "view_bom",
  EDIT_BOM: "edit_bom",
  VIEW_MANUFACTURING: "view_manufacturing",
  CREATE_PRODUCTION_BATCHES: "create_production_batches",
  
  // Inventory
  VIEW_INVENTORY: "view_inventory",
  EDIT_INVENTORY: "edit_inventory",
  VIEW_TRANSACTIONS: "view_transactions",
  CREATE_TRANSACTIONS: "create_transactions",
  
  // System
  SYSTEM_ADMIN: "system_admin",
  VIEW_REPORTS: "view_reports",
} as const;
