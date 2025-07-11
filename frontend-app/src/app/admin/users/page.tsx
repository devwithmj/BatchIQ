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
import { User } from "@/lib/auth-schema";
import { userApi } from "@/lib/auth-api";
import { PERMISSIONS } from "@/lib/auth-schema";

import { toast } from "sonner";
import { Pencil, Eye, Shield, UserCheck, UserX } from "lucide-react";
import { UserFormDialog } from "@/components/admin/UserFormDialog";
import { UserViewDialog } from "@/components/admin/UserViewDialog";
import { UserRoleDialog } from "@/components/admin/UserRoleDialog";
import AdminLayout from "@/components/layout/AdminLayout";

export default function AdminUsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [showCreateDialog, setShowCreateDialog] = useState(false);
  const [showEditDialog, setShowEditDialog] = useState(false);
  const [showViewDialog, setShowViewDialog] = useState(false);
  const [showRoleDialog, setShowRoleDialog] = useState(false);

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      setIsLoading(true);
      const fetchedUsers = await userApi.getUsers();
      setUsers(fetchedUsers);
    } catch (error) {
      toast.error("Failed to fetch users");
      console.error("Error fetching users:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleActivateUser = async (userId: number) => {
    try {
      await userApi.activateUser(userId);
      toast.success("User activated successfully");
      fetchUsers();
    } catch {
      toast.error("Failed to activate user");
    }
  };

  const handleDeactivateUser = async (userId: number) => {
    try {
      await userApi.deactivateUser(userId);
      toast.success("User deactivated successfully");
      fetchUsers();
    } catch {
      toast.error("Failed to deactivate user");
    }
  };

  const handleUserCreated = () => {
    setShowCreateDialog(false);
    fetchUsers();
  };

  const handleUserUpdated = () => {
    setShowEditDialog(false);
    setSelectedUser(null);
    fetchUsers();
  };

  const handleRolesUpdated = () => {
    setShowRoleDialog(false);
    setSelectedUser(null);
    fetchUsers();
  };

  if (isLoading) {
    return (
      <div className="p-6">
        <div className="flex items-center justify-center h-64">
          <div className="text-lg">Loading users...</div>
        </div>
      </div>
    );
  }

  return (
    <AdminLayout>
      <div className="p-6 space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold">User Management</h1>
            <p className="text-gray-600">
              Manage users, roles, and permissions
            </p>
          </div>
          <PermissionGate permissions={[PERMISSIONS.CREATE_USERS]}>
            <Button onClick={() => setShowCreateDialog(true)}>Add User</Button>
          </PermissionGate>
        </div>

        <div className="grid gap-6">
          {users.map((user) => (
            <Card key={user.id}>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <div>
                    <CardTitle className="flex items-center gap-2">
                      {user.firstName} {user.lastName}
                      {!user.isActive && (
                        <Badge variant="secondary">Inactive</Badge>
                      )}
                      {!user.emailConfirmed && (
                        <Badge variant="outline">Email Unconfirmed</Badge>
                      )}
                    </CardTitle>
                    <CardDescription>
                      @{user.username} • {user.email}
                    </CardDescription>
                  </div>
                  <div className="flex items-center gap-2">
                    <PermissionGate permissions={[PERMISSIONS.VIEW_USERS]}>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => {
                          setSelectedUser(user);
                          setShowViewDialog(true);
                        }}
                      >
                        <Eye className="h-4 w-4" />
                      </Button>
                    </PermissionGate>

                    <PermissionGate permissions={[PERMISSIONS.EDIT_USERS]}>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => {
                          setSelectedUser(user);
                          setShowEditDialog(true);
                        }}
                      >
                        <Pencil className="h-4 w-4" />
                      </Button>
                    </PermissionGate>

                    <PermissionGate permissions={[PERMISSIONS.ASSIGN_ROLES]}>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => {
                          setSelectedUser(user);
                          setShowRoleDialog(true);
                        }}
                      >
                        <Shield className="h-4 w-4" />
                      </Button>
                    </PermissionGate>

                    <PermissionGate permissions={[PERMISSIONS.EDIT_USERS]}>
                      {user.isActive ? (
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleDeactivateUser(user.id)}
                        >
                          <UserX className="h-4 w-4" />
                        </Button>
                      ) : (
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleActivateUser(user.id)}
                        >
                          <UserCheck className="h-4 w-4" />
                        </Button>
                      )}
                    </PermissionGate>
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                <div className="space-y-2">
                  <div>
                    <span className="text-sm font-medium">Roles: </span>
                    {user.roles && user.roles.length > 0 ? (
                      <div className="inline-flex gap-1">
                        {user.roles.map((role) => (
                          <Badge key={role.id} variant="outline">
                            {role.name}
                          </Badge>
                        ))}
                      </div>
                    ) : (
                      <span className="text-sm text-gray-500">
                        No roles assigned
                      </span>
                    )}
                  </div>
                  <div className="text-sm text-gray-600">
                    Created: {new Date(user.createdAt).toLocaleDateString()}
                    {user.lastLoginAt && (
                      <span className="ml-4">
                        Last login:{" "}
                        {new Date(user.lastLoginAt).toLocaleDateString()}
                      </span>
                    )}
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>

        {/* Dialogs */}
        <UserFormDialog
          open={showCreateDialog}
          onOpenChange={setShowCreateDialog}
          onSuccess={handleUserCreated}
        />

        <UserFormDialog
          open={showEditDialog}
          onOpenChange={setShowEditDialog}
          user={selectedUser}
          onSuccess={handleUserUpdated}
        />

        <UserViewDialog
          open={showViewDialog}
          onOpenChange={setShowViewDialog}
          user={selectedUser}
        />

        <UserRoleDialog
          open={showRoleDialog}
          onOpenChange={setShowRoleDialog}
          user={selectedUser}
          onSuccess={handleRolesUpdated}
        />
      </div>
    </AdminLayout>
  );
}
