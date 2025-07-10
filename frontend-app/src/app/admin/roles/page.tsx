"use client";

import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { PermissionGate } from "@/lib/auth-context";
import { Role } from "@/lib/auth-schema";
import { roleApi } from "@/lib/auth-api";
import { PERMISSIONS } from "@/lib/auth-schema";
import { RoleFormDialog } from "@/components/admin/RoleFormDialog";
import { RoleViewDialog } from "@/components/admin/RoleViewDialog";
import { toast } from "sonner";
import { Pencil, Eye } from "lucide-react";

export default function AdminRolesPage() {
  const [roles, setRoles] = useState<Role[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [selectedRole, setSelectedRole] = useState<Role | null>(null);
  const [showCreateDialog, setShowCreateDialog] = useState(false);
  const [showEditDialog, setShowEditDialog] = useState(false);
  const [showViewDialog, setShowViewDialog] = useState(false);

  useEffect(() => {
    fetchRoles();
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

  const handleRoleCreated = () => {
    setShowCreateDialog(false);
    fetchRoles();
  };

  const handleRoleUpdated = () => {
    setShowEditDialog(false);
    setSelectedRole(null);
    fetchRoles();
  };

  if (isLoading) {
    return (
      <div className="p-6">
        <div className="flex items-center justify-center h-64">
          <div className="text-lg">Loading roles...</div>
        </div>
      </div>
    );
  }

  return (
    <div className="p-6 space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold">Role Management</h1>
          <p className="text-gray-600">Manage roles and permissions</p>
        </div>
        <PermissionGate permissions={[PERMISSIONS.SYSTEM_ADMIN]}>
          <Button onClick={() => setShowCreateDialog(true)}>
            Add Role
          </Button>
        </PermissionGate>
      </div>

      <div className="grid gap-6">
        {roles.map((role) => (
          <Card key={role.id}>
            <CardHeader>
              <div className="flex items-center justify-between">
                <div>
                  <CardTitle className="flex items-center gap-2">
                    {role.name}
                    {!role.isActive && (
                      <Badge variant="secondary">Inactive</Badge>
                    )}
                  </CardTitle>
                  <CardDescription>
                    {role.description || "No description provided"}
                  </CardDescription>
                </div>
                <div className="flex items-center gap-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => {
                      setSelectedRole(role);
                      setShowViewDialog(true);
                    }}
                  >
                    <Eye className="h-4 w-4" />
                  </Button>
                  
                  <PermissionGate permissions={[PERMISSIONS.SYSTEM_ADMIN]}>
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => {
                        setSelectedRole(role);
                        setShowEditDialog(true);
                      }}
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                  </PermissionGate>
                </div>
              </div>
            </CardHeader>
            <CardContent>
              <div className="space-y-2">
                <div>
                  <span className="text-sm font-medium">Permissions: </span>
                  {role.permissions && role.permissions.length > 0 ? (
                    <div className="inline-flex gap-1 flex-wrap">
                      {role.permissions.slice(0, 5).map((permission) => (
                        <Badge key={permission.id} variant="outline" className="text-xs">
                          {permission.name}
                        </Badge>
                      ))}
                      {role.permissions.length > 5 && (
                        <Badge variant="outline" className="text-xs">
                          +{role.permissions.length - 5} more
                        </Badge>
                      )}
                    </div>
                  ) : (
                    <span className="text-sm text-gray-500">No permissions assigned</span>
                  )}
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Dialogs */}
      <RoleFormDialog
        open={showCreateDialog}
        onOpenChange={setShowCreateDialog}
        onSuccess={handleRoleCreated}
      />

      <RoleFormDialog
        open={showEditDialog}
        onOpenChange={setShowEditDialog}
        role={selectedRole}
        onSuccess={handleRoleUpdated}
      />

      <RoleViewDialog
        open={showViewDialog}
        onOpenChange={setShowViewDialog}
        role={selectedRole}
      />
    </div>
  );
}
