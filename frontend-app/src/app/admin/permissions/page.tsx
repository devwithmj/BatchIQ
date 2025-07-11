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
import { Permission } from "@/lib/auth-schema";
import { permissionApi } from "@/lib/auth-api";
import { PERMISSIONS } from "@/lib/auth-schema";
import { toast } from "sonner";
import { Shield, Plus, Edit, Trash2, Eye, Info } from "lucide-react";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { PermissionViewDialog } from "@/components/admin/PermissionViewDialog";
import { PermissionFormDialog } from "@/components/admin/PermissionFormDialog";
import AdminLayout from "@/components/layout/AdminLayout";

export default function AdminPermissionsPage() {
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [editingPermission, setEditingPermission] = useState<Permission | null>(
    null
  );
  const [viewingPermission, setViewingPermission] = useState<Permission | null>(
    null
  );

  useEffect(() => {
    fetchPermissions();
  }, []);

  const fetchPermissions = async () => {
    try {
      setIsLoading(true);
      const fetchedPermissions = await permissionApi.getPermissions();
      setPermissions(fetchedPermissions);
    } catch (error) {
      toast.error("Failed to fetch permissions");
      console.error("Error fetching permissions:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreatePermission = async () => {
    try {
      await fetchPermissions();
      setIsCreateDialogOpen(false);
    } catch {
      // Error is already shown in the dialog
    }
  };

  const handleUpdatePermission = async () => {
    try {
      await fetchPermissions();
      setEditingPermission(null);
    } catch {
      // Error is already shown in the dialog
    }
  };

  const handleDeletePermission = async (permissionId: number) => {
    if (!confirm("Are you sure you want to delete this permission?")) return;

    try {
      await permissionApi.deletePermission(permissionId);
      await fetchPermissions();
      toast.success("Permission deleted successfully");
    } catch (error) {
      if (
        error instanceof Error &&
        error.message.includes("not yet implemented")
      ) {
        toast.error(
          "Permission deletion is not yet available. Backend endpoint needs to be implemented."
        );
      } else {
        toast.error("Failed to delete permission");
      }
      console.error("Error deleting permission:", error);
    }
  };

  // Group permissions by category
  const groupedPermissions = permissions.reduce((acc, permission) => {
    const category = permission.category || "Other";
    if (!acc[category]) {
      acc[category] = [];
    }
    acc[category].push(permission);
    return acc;
  }, {} as Record<string, Permission[]>);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-[200px]">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto"></div>
          <p className="mt-2 text-sm text-gray-600">Loading permissions...</p>
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
                Permission Management
              </h1>
              <p className="text-muted-foreground">
                View system permissions. Editing capabilities require backend
                implementation.
              </p>
            </div>
            <Button onClick={() => setIsCreateDialogOpen(true)}>
              <Plus className="mr-2 h-4 w-4" />
              Create Permission
            </Button>
          </div>

          <Alert variant="warning">
            <Info className="h-4 w-4" />
            <AlertDescription>
              <strong>Backend Implementation Required:</strong> Permission
              editing and creation requires the following API endpoints to be
              implemented:
              <code className="block mt-1 text-xs">
                POST /api/permissions, PUT /api/permissions/&#123;id&#125;,
                DELETE /api/permissions/&#123;id&#125;
              </code>
              Currently showing permissions extracted from existing roles.
            </AlertDescription>
          </Alert>

          {Object.entries(groupedPermissions).map(
            ([category, categoryPermissions]) => (
              <div key={category} className="space-y-4">
                <h2 className="text-xl font-semibold">{category}</h2>
                <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                  {categoryPermissions.map((permission) => (
                    <Card key={permission.id} className="relative">
                      <CardHeader>
                        <div className="flex items-center justify-between">
                          <CardTitle className="flex items-center gap-2">
                            <Shield className="h-4 w-4" />
                            {permission.name}
                          </CardTitle>
                          <Badge
                            variant={
                              permission.isActive ? "default" : "secondary"
                            }
                          >
                            {permission.isActive ? "Active" : "Inactive"}
                          </Badge>
                        </div>
                        <CardDescription>
                          {permission.description || "No description available"}
                        </CardDescription>
                      </CardHeader>
                      <CardContent>
                        <div className="flex items-center gap-2 pt-2">
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => setViewingPermission(permission)}
                          >
                            <Eye className="mr-1 h-3 w-3" />
                            View
                          </Button>
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => setEditingPermission(permission)}
                          >
                            <Edit className="mr-1 h-3 w-3" />
                            Edit
                          </Button>
                          <Button
                            variant="destructive"
                            size="sm"
                            onClick={() =>
                              handleDeletePermission(permission.id)
                            }
                          >
                            <Trash2 className="mr-1 h-3 w-3" />
                            Delete
                          </Button>
                        </div>
                      </CardContent>
                    </Card>
                  ))}
                </div>
              </div>
            )
          )}

          {permissions.length === 0 && !isLoading && (
            <Card>
              <CardContent className="flex flex-col items-center justify-center py-12">
                <Shield className="h-12 w-12 text-muted-foreground mb-4" />
                <h3 className="text-lg font-semibold mb-2">
                  No Permissions Found
                </h3>
                <p className="text-muted-foreground text-center mb-4">
                  No permissions have been created in the system yet.
                </p>
                <Button onClick={() => setIsCreateDialogOpen(true)}>
                  <Plus className="mr-2 h-4 w-4" />
                  Create First Permission
                </Button>
              </CardContent>
            </Card>
          )}

          {/* Dialogs */}
          <PermissionFormDialog
            open={isCreateDialogOpen}
            onOpenChange={setIsCreateDialogOpen}
            onSuccess={handleCreatePermission}
          />

          <PermissionFormDialog
            open={!!editingPermission}
            onOpenChange={(open: boolean) =>
              !open && setEditingPermission(null)
            }
            onSuccess={handleUpdatePermission}
            permission={editingPermission}
          />

          <PermissionViewDialog
            permission={viewingPermission}
            open={!!viewingPermission}
            onOpenChange={(open: boolean) =>
              !open && setViewingPermission(null)
            }
          />
        </div>
      </PermissionGate>
    </AdminLayout>
  );
}
