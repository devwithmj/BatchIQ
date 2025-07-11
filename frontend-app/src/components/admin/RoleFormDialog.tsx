"use client";

import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Checkbox } from "@/components/ui/checkbox";
import { Role, Permission } from "@/lib/auth-schema";
import { roleApi, permissionApi } from "@/lib/auth-api";
import { toast } from "sonner";
import * as z from "zod";
import { zodResolver } from "@hookform/resolvers/zod";

const roleFormSchema = z.object({
  name: z.string().min(1, "Role name is required"),
  description: z.string().optional(),
  isActive: z.boolean().optional(),
  permissionIds: z.array(z.number()).optional(),
});

type RoleFormData = z.infer<typeof roleFormSchema>;

interface RoleFormDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  role?: Role | null;
  onSuccess: () => void;
}

export function RoleFormDialog({ open, onOpenChange, role, onSuccess }: RoleFormDialogProps) {
  const [isLoading, setIsLoading] = useState(false);
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const isEditing = !!role;

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    watch,
  } = useForm<RoleFormData>({
    resolver: zodResolver(roleFormSchema),
    defaultValues: {
      name: "",
      description: "",
      isActive: true,
      permissionIds: [],
    },
  });

  const selectedPermissionIds = watch("permissionIds") || [];

  useEffect(() => {
    if (open) {
      fetchPermissions();
      if (isEditing && role) {
        setValue("name", role.name);
        setValue("description", role.description || "");
        setValue("permissionIds", role.permissions?.map(p => p.id) || []);
      } else {
        reset({
          name: "",
          description: "",
          permissionIds: [],
        });
      }
    }
  }, [open, role, isEditing, setValue, reset]);

  const fetchPermissions = async () => {
    try {
      const fetchedPermissions = await permissionApi.getPermissions();
      setPermissions(fetchedPermissions.filter(p => p.isActive));
    } catch {
      toast.error("Failed to fetch permissions");
    }
  };

  const onSubmit = async (data: RoleFormData) => {
    setIsLoading(true);
    try {
      if (isEditing && role) {
        await roleApi.updateRole(role.id, data);
        toast.success("Role updated successfully");
      } else {
        await roleApi.createRole({
          ...data,
          isActive: data.isActive ?? true, // Default to true if not specified
        });
        toast.success("Role created successfully");
      }
      onSuccess();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Operation failed");
    } finally {
      setIsLoading(false);
    }
  };

  const handlePermissionToggle = (permissionId: number, checked: boolean) => {
    if (checked) {
      setValue("permissionIds", [...selectedPermissionIds, permissionId]);
    } else {
      setValue("permissionIds", selectedPermissionIds.filter((id: number) => id !== permissionId));
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

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>
            {isEditing ? "Edit Role" : "Create Role"}
          </DialogTitle>
          <DialogDescription>
            {isEditing 
              ? "Update role information and permissions." 
              : "Create a new role with specific permissions."
            }
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="name">Role Name</Label>
            <Input
              id="name"
              {...register("name")}
              disabled={isLoading}
              placeholder="Enter role name"
            />
            {errors.name && (
              <p className="text-sm text-red-600">{errors.name.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="description">Description</Label>
            <Input
              id="description"
              {...register("description")}
              disabled={isLoading}
              placeholder="Enter role description (optional)"
            />
          </div>

          <div className="space-y-4">
            <Label>Permissions</Label>
            {Object.entries(groupedPermissions).map(([category, categoryPermissions]) => (
              <div key={category} className="space-y-2">
                <h4 className="text-sm font-medium text-gray-700">{category}</h4>
                <div className="grid grid-cols-2 gap-2 pl-4">
                  {categoryPermissions.map((permission) => (
                    <div key={permission.id} className="flex items-center space-x-2">
                      <Checkbox
                        id={`permission-${permission.id}`}
                        checked={selectedPermissionIds.includes(permission.id)}
                        onCheckedChange={(checked) => 
                          handlePermissionToggle(permission.id, checked as boolean)
                        }
                      />
                      <Label htmlFor={`permission-${permission.id}`} className="text-sm">
                        {permission.name}
                        {permission.description && (
                          <span className="text-gray-500 block text-xs">
                            {permission.description}
                          </span>
                        )}
                      </Label>
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
              disabled={isLoading}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? "Saving..." : isEditing ? "Update" : "Create"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
