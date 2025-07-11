"use client";

import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Badge } from "@/components/ui/badge";
import { Permission } from "@/lib/auth-schema";

interface PermissionViewDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  permission: Permission | null;
}

export function PermissionViewDialog({ open, onOpenChange, permission }: PermissionViewDialogProps) {
  if (!permission) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Permission Details</DialogTitle>
          <DialogDescription>
            View permission information
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6">
          <div>
            <h4 className="text-sm font-medium text-gray-500">Permission Name</h4>
            <p className="text-lg font-semibold">{permission.name}</p>
          </div>

          <div>
            <h4 className="text-sm font-medium text-gray-500">Description</h4>
            <p className="text-sm">{permission.description || "No description provided"}</p>
          </div>

          <div>
            <h4 className="text-sm font-medium text-gray-500">Category</h4>
            <p className="text-sm">{permission.category || "No category assigned"}</p>
          </div>

          <div>
            <h4 className="text-sm font-medium text-gray-500">Status</h4>
            <Badge variant={permission.isActive ? "default" : "secondary"}>
              {permission.isActive ? "Active" : "Inactive"}
            </Badge>
          </div>

          {permission.descriptionFa && (
            <div>
              <h4 className="text-sm font-medium text-gray-500">Description (Farsi)</h4>
              <p className="text-sm">{permission.descriptionFa}</p>
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}
