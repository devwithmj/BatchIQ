"use client";

import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { User, Role } from "@/lib/auth-schema";
import { userApi, roleApi } from "@/lib/auth-api";
import { toast } from "sonner";

interface UserRoleDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  user: User | null;
  onSuccess: () => void;
}

export function UserRoleDialog({ open, onOpenChange, user, onSuccess }: UserRoleDialogProps) {
  const [isLoading, setIsLoading] = useState(false);
  const [roles, setRoles] = useState<Role[]>([]);
  const [selectedRoleIds, setSelectedRoleIds] = useState<number[]>([]);

  useEffect(() => {
    if (open && user) {
      fetchRoles();
      setSelectedRoleIds(user.roles?.map(r => r.id) || []);
    }
  }, [open, user]);

  const fetchRoles = async () => {
    try {
      const fetchedRoles = await roleApi.getRoles();
      setRoles(fetchedRoles.filter(r => r.isActive));
    } catch (err) {
      console.error('Failed to fetch roles:', err);
      toast.error("Failed to fetch roles");
    }
  };

  const handleRoleToggle = (roleId: number, checked: boolean) => {
    if (checked) {
      setSelectedRoleIds(prev => [...prev, roleId]);
    } else {
      setSelectedRoleIds(prev => prev.filter(id => id !== roleId));
    }
  };

  const handleSubmit = async () => {
    if (!user) return;

    setIsLoading(true);
    try {
      await userApi.assignRoles({
        userId: user.id,
        roleIds: selectedRoleIds,
      });
      toast.success("Roles updated successfully");
      onSuccess();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to update roles");
    } finally {
      setIsLoading(false);
    }
  };

  if (!user) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle>Manage User Roles</DialogTitle>
          <DialogDescription>
            Assign or remove roles for {user.firstName} {user.lastName}
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          <div>
            <h4 className="text-sm font-medium mb-2">Current Roles</h4>
            {user.roles && user.roles.length > 0 ? (
              <div className="flex flex-wrap gap-1">
                {user.roles.map((role) => (
                  <Badge key={role.id} variant="outline">
                    {role.name}
                  </Badge>
                ))}
              </div>
            ) : (
              <p className="text-sm text-gray-500">No roles assigned</p>
            )}
          </div>

          <div>
            <h4 className="text-sm font-medium mb-2">Available Roles</h4>
            <div className="space-y-3 max-h-64 overflow-y-auto">
              {roles.map((role) => (
                <div key={role.id} className="flex items-start space-x-3">
                  <Checkbox
                    id={`role-${role.id}`}
                    checked={selectedRoleIds.includes(role.id)}
                    onCheckedChange={(checked) => 
                      handleRoleToggle(role.id, checked as boolean)
                    }
                  />
                  <div className="flex-1">
                    <Label htmlFor={`role-${role.id}`} className="text-sm font-medium">
                      {role.name}
                    </Label>
                    {role.description && (
                      <p className="text-xs text-gray-500 mt-1">{role.description}</p>
                    )}
                    {role.permissions && role.permissions.length > 0 && (
                      <div className="flex flex-wrap gap-1 mt-2">
                        {role.permissions.slice(0, 3).map((permission) => (
                          <Badge key={permission.id} variant="secondary" className="text-xs">
                            {permission.name}
                          </Badge>
                        ))}
                        {role.permissions.length > 3 && (
                          <Badge variant="secondary" className="text-xs">
                            +{role.permissions.length - 3} more
                          </Badge>
                        )}
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={isLoading}
          >
            Cancel
          </Button>
          <Button onClick={handleSubmit} disabled={isLoading}>
            {isLoading ? "Updating..." : "Update Roles"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
