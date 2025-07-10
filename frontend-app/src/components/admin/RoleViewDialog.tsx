"use client";

import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Badge } from "@/components/ui/badge";
import { Role, Permission } from "@/lib/auth-schema";

interface RoleViewDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  role: Role | null;
}

export function RoleViewDialog({ open, onOpenChange, role }: RoleViewDialogProps) {
  if (!role) return null;

  // Group permissions by category
  const groupedPermissions = role.permissions?.reduce((acc, permission) => {
    const category = permission.category || "Other";
    if (!acc[category]) {
      acc[category] = [];
    }
    acc[category].push(permission);
    return acc;
  }, {} as Record<string, Permission[]>) || {};

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Role Details</DialogTitle>
          <DialogDescription>
            View role information and permissions
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6">
          <div>
            <h4 className="text-sm font-medium text-gray-500">Role Name</h4>
            <p className="text-lg font-semibold">{role.name}</p>
          </div>

          <div>
            <h4 className="text-sm font-medium text-gray-500">Description</h4>
            <p className="text-sm">{role.description || "No description provided"}</p>
          </div>

          <div>
            <h4 className="text-sm font-medium text-gray-500">Status</h4>
            <Badge variant={role.isActive ? "default" : "secondary"}>
              {role.isActive ? "Active" : "Inactive"}
            </Badge>
          </div>

          <div>
            <h4 className="text-sm font-medium text-gray-500 mb-3">Permissions</h4>
            {Object.keys(groupedPermissions).length > 0 ? (
              <div className="space-y-4">
                {Object.entries(groupedPermissions).map(([category, permissions]) => (
                  <div key={category}>
                    <h5 className="text-sm font-medium text-gray-700 mb-2">{category}</h5>
                    <div className="grid grid-cols-2 gap-2 pl-4">
                      {permissions.map((permission) => (
                        <div key={permission.id} className="space-y-1">
                          <Badge variant="outline" className="text-xs">
                            {permission.name}
                          </Badge>
                          {permission.description && (
                            <p className="text-xs text-gray-500">
                              {permission.description}
                            </p>
                          )}
                        </div>
                      ))}
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-sm text-gray-400">No permissions assigned</p>
            )}
          </div>

          {(role.descriptionFa) && (
            <div>
              <h4 className="text-sm font-medium text-gray-500">Description (Farsi)</h4>
              <p className="text-sm">{role.descriptionFa}</p>
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}
