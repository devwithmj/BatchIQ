"use client";

import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Badge } from "@/components/ui/badge";
import { User } from "@/lib/auth-schema";

interface UserViewDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  user: User | null;
}

export function UserViewDialog({ open, onOpenChange, user }: UserViewDialogProps) {
  if (!user) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>User Details</DialogTitle>
          <DialogDescription>
            View user information and roles
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <h4 className="text-sm font-medium text-gray-500">First Name</h4>
              <p className="text-sm">{user.firstName}</p>
            </div>
            <div>
              <h4 className="text-sm font-medium text-gray-500">Last Name</h4>
              <p className="text-sm">{user.lastName}</p>
            </div>
          </div>

          {(user.firstNameFa || user.lastNameFa) && (
            <div className="grid grid-cols-2 gap-4">
              <div>
                <h4 className="text-sm font-medium text-gray-500">First Name (Farsi)</h4>
                <p className="text-sm">{user.firstNameFa || "-"}</p>
              </div>
              <div>
                <h4 className="text-sm font-medium text-gray-500">Last Name (Farsi)</h4>
                <p className="text-sm">{user.lastNameFa || "-"}</p>
              </div>
            </div>
          )}

          <div>
            <h4 className="text-sm font-medium text-gray-500">Username</h4>
            <p className="text-sm">{user.username}</p>
          </div>

          <div>
            <h4 className="text-sm font-medium text-gray-500">Email</h4>
            <p className="text-sm">{user.email}</p>
          </div>

          <div>
            <h4 className="text-sm font-medium text-gray-500">Status</h4>
            <div className="flex gap-2">
              <Badge variant={user.isActive ? "default" : "secondary"}>
                {user.isActive ? "Active" : "Inactive"}
              </Badge>
              <Badge variant={user.emailConfirmed ? "default" : "outline"}>
                {user.emailConfirmed ? "Email Confirmed" : "Email Unconfirmed"}
              </Badge>
            </div>
          </div>

          <div>
            <h4 className="text-sm font-medium text-gray-500">Roles</h4>
            {user.roles && user.roles.length > 0 ? (
              <div className="flex flex-wrap gap-1 mt-1">
                {user.roles.map((role) => (
                  <Badge key={role.id} variant="outline">
                    {role.name}
                  </Badge>
                ))}
              </div>
            ) : (
              <p className="text-sm text-gray-400">No roles assigned</p>
            )}
          </div>

          <div>
            <h4 className="text-sm font-medium text-gray-500">Permissions</h4>
            {user.roles && user.roles.some(role => role.permissions && role.permissions.length > 0) ? (
              <div className="flex flex-wrap gap-1 mt-1 max-h-32 overflow-y-auto">
                {Array.from(new Set(
                  user.roles.flatMap(role => 
                    role.permissions?.map(p => p.name) || []
                  )
                )).map((permission) => (
                  <Badge key={permission} variant="secondary" className="text-xs">
                    {permission}
                  </Badge>
                ))}
              </div>
            ) : (
              <p className="text-sm text-gray-400">No permissions</p>
            )}
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <h4 className="text-sm font-medium text-gray-500">Created</h4>
              <p className="text-sm">{new Date(user.createdAt).toLocaleDateString()}</p>
            </div>
            <div>
              <h4 className="text-sm font-medium text-gray-500">Last Login</h4>
              <p className="text-sm">
                {user.lastLoginAt 
                  ? new Date(user.lastLoginAt).toLocaleDateString()
                  : "Never"
                }
              </p>
            </div>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
