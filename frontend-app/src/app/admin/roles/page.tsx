"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { PermissionGate } from "@/lib/auth-context";
import { Role, User } from "@/lib/auth-schema";
import { userApi } from "@/lib/auth-api";
import { PERMISSIONS } from "@/lib/auth-schema";
import { toast } from "sonner";
import { Users, Shield } from "lucide-react";

export default function AdminRolesPage() {
  const [roles, setRoles] = useState<Role[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    fetchRolesFromUsers();
  }, []);

  const fetchRolesFromUsers = async () => {
    try {
      setIsLoading(true);
      // Get all users to extract unique roles
      const fetchedUsers = await userApi.getUsers();
      setUsers(fetchedUsers);
      
      // Extract unique roles from all users
      const allRoles: Role[] = [];
      const roleMap = new Map<number, Role>();
      
      fetchedUsers.forEach(user => {
        user.roles?.forEach(role => {
          if (!roleMap.has(role.id)) {
            roleMap.set(role.id, role);
            allRoles.push(role);
          }
        });
      });
      
      setRoles(allRoles);
    } catch (error) {
      toast.error("Failed to fetch roles");
      console.error("Error fetching roles:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const getUserCountForRole = (roleId: number): number => {
    return users.filter(user => 
      user.roles?.some(role => role.id === roleId)
    ).length;
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-[200px]">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto"></div>
          <p className="mt-2 text-sm text-gray-600">Loading roles...</p>
        </div>
      </div>
    );
  }

  return (
    <PermissionGate permissions={[PERMISSIONS.SYSTEM_ADMIN]}>
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Role Management</h1>
          <p className="text-muted-foreground">
            View system roles and permissions extracted from user data.
          </p>
        </div>

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {roles.map((role) => (
            <Card key={role.id} className="relative">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="flex items-center gap-2">
                    <Shield className="h-5 w-5" />
                    {role.name}
                  </CardTitle>
                  <Badge variant={role.isActive ? "default" : "secondary"}>
                    {role.isActive ? "Active" : "Inactive"}
                  </Badge>
                </div>
                <CardDescription>
                  {role.description || "No description available"}
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <Users className="h-4 w-4" />
                    <span>{getUserCountForRole(role.id)} users assigned</span>
                  </div>
                  
                  {role.permissions && role.permissions.length > 0 && (
                    <div>
                      <h4 className="text-sm font-medium mb-2">Permissions</h4>
                      <div className="flex flex-wrap gap-1">
                        {role.permissions.slice(0, 3).map((permission) => (
                          <Badge key={permission.id} variant="outline" className="text-xs">
                            {permission.name}
                          </Badge>
                        ))}
                        {role.permissions.length > 3 && (
                          <Badge variant="outline" className="text-xs">
                            +{role.permissions.length - 3} more
                          </Badge>
                        )}
                      </div>
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>
          ))}
        </div>

        {roles.length === 0 && (
          <Card>
            <CardContent className="flex flex-col items-center justify-center py-12">
              <Shield className="h-12 w-12 text-muted-foreground mb-4" />
              <h3 className="text-lg font-semibold mb-2">No Roles Found</h3>
              <p className="text-muted-foreground text-center">
                No roles are currently assigned to users in the system.
              </p>
            </CardContent>
          </Card>
        )}
      </div>
    </PermissionGate>
  );
}
