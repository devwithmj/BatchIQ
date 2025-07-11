"use client";

import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { PermissionGate } from "@/lib/auth-context";
import { Role, User } from "@/lib/auth-schema";
import { roleApi, userApi } from "@/lib/auth-api";
import { PERMISSIONS } from "@/lib/auth-schema";
import { toast } from "sonner";
import { Users, Shield, Plus, Edit, Trash2, Eye } from "lucide-react";
import { RoleFormDialog } from "@/components/admin/RoleFormDialog";
import { RoleViewDialog } from "@/components/admin/RoleViewDialog";
import AdminLayout from "@/components/layout/AdminLayout";

export default function AdminRolesPage() {
  const [roles, setRoles] = useState<Role[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [editingRole, setEditingRole] = useState<Role | null>(null);
  const [viewingRole, setViewingRole] = useState<Role | null>(null);

  useEffect(() => {
    fetchRoles();
    fetchUsers();
  }, []);

  const fetchRoles = async () => {
    try {
      setIsLoading(true);
      const fetchedRoles = await roleApi.getRoles();
      setRoles(fetchedRoles);
    } catch (error) {
      toast.error("Failed to fetch roles");
      console.error("Error fetching roles:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const fetchUsers = async () => {
    try {
      const fetchedUsers = await userApi.getUsers();
      setUsers(fetchedUsers);
    } catch (error) {
      console.error("Error fetching users:", error);
    }
  };

  const getUserCountForRole = (roleId: number): number => {
    return users.filter((user) =>
      user.roles?.some((role) => role.id === roleId)
    ).length;
  };

  const handleCreateRole = async () => {
    await fetchRoles();
    setIsCreateDialogOpen(false);
  };

  const handleUpdateRole = async () => {
    await fetchRoles();
    setEditingRole(null);
  };

  const handleDeleteRole = async (roleId: number) => {
    if (!confirm("Are you sure you want to delete this role?")) return;

    try {
      await roleApi.deleteRole(roleId);
      await fetchRoles();
      toast.success("Role deleted successfully");
    } catch (error) {
      toast.error("Failed to delete role");
      console.error("Error deleting role:", error);
    }
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
    <AdminLayout>
      <PermissionGate permissions={[PERMISSIONS.SYSTEM_ADMIN]}>
        <div className="space-y-6">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-3xl font-bold tracking-tight">
                Role Management
              </h1>
              <p className="text-muted-foreground">
                Manage system roles and permissions dynamically.
              </p>
            </div>
            <Button onClick={() => setIsCreateDialogOpen(true)}>
              <Plus className="mr-2 h-4 w-4" />
              Create Role
            </Button>
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
                    <div className="flex items-center gap-2">
                      <Badge variant={role.isActive ? "default" : "secondary"}>
                        {role.isActive ? "Active" : "Inactive"}
                      </Badge>
                    </div>
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
                        <h4 className="text-sm font-medium mb-2">
                          Permissions
                        </h4>
                        <div className="flex flex-wrap gap-1">
                          {role.permissions.slice(0, 3).map((permission) => (
                            <Badge
                              key={permission.id}
                              variant="outline"
                              className="text-xs"
                            >
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

                    <div className="flex items-center gap-2 pt-2">
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => setViewingRole(role)}
                      >
                        <Eye className="mr-1 h-3 w-3" />
                        View
                      </Button>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => setEditingRole(role)}
                      >
                        <Edit className="mr-1 h-3 w-3" />
                        Edit
                      </Button>
                      <Button
                        variant="destructive"
                        size="sm"
                        onClick={() => handleDeleteRole(role.id)}
                      >
                        <Trash2 className="mr-1 h-3 w-3" />
                        Delete
                      </Button>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>

          {roles.length === 0 && !isLoading && (
            <Card>
              <CardContent className="flex flex-col items-center justify-center py-12">
                <Shield className="h-12 w-12 text-muted-foreground mb-4" />
                <h3 className="text-lg font-semibold mb-2">No Roles Found</h3>
                <p className="text-muted-foreground text-center mb-4">
                  No roles have been created in the system yet.
                </p>
                <Button onClick={() => setIsCreateDialogOpen(true)}>
                  <Plus className="mr-2 h-4 w-4" />
                  Create First Role
                </Button>
              </CardContent>
            </Card>
          )}

          {/* Dialogs */}
          <RoleFormDialog
            open={isCreateDialogOpen}
            onOpenChange={setIsCreateDialogOpen}
            onSuccess={handleCreateRole}
          />

          <RoleFormDialog
            open={!!editingRole}
            onOpenChange={(open) => !open && setEditingRole(null)}
            onSuccess={handleUpdateRole}
            role={editingRole}
          />

          <RoleViewDialog
            role={viewingRole}
            open={!!viewingRole}
            onOpenChange={(open) => !open && setViewingRole(null)}
            userCount={viewingRole ? getUserCountForRole(viewingRole.id) : 0}
          />
        </div>
      </PermissionGate>
    </AdminLayout>
  );
}
