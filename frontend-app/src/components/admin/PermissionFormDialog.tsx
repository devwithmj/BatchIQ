"use client";

import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Permission, PERMISSION_CATEGORIES } from "@/lib/auth-schema";
import { permissionApi } from "@/lib/auth-api";
import { toast } from "sonner";
import * as z from "zod";
import { zodResolver } from "@hookform/resolvers/zod";

const permissionFormSchema = z.object({
  name: z.string().min(1, "Permission name is required"),
  description: z.string().optional(),
  category: z.string().optional(),
  isActive: z.boolean().optional(),
});

type PermissionFormData = z.infer<typeof permissionFormSchema>;

interface PermissionFormDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  permission?: Permission | null;
  onSuccess: () => void;
}

export function PermissionFormDialog({ open, onOpenChange, permission, onSuccess }: PermissionFormDialogProps) {
  const [isLoading, setIsLoading] = useState(false);
  const isEditing = !!permission;

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    watch,
  } = useForm<PermissionFormData>({
    resolver: zodResolver(permissionFormSchema),
    defaultValues: {
      name: "",
      description: "",
      category: undefined,
      isActive: true,
    },
  });

  const selectedCategory = watch("category");

  useEffect(() => {
    if (open) {
      if (isEditing && permission) {
        setValue("name", permission.name);
        setValue("description", permission.description || "");
        setValue("category", permission.category || undefined);
        setValue("isActive", permission.isActive);
      } else {
        reset({
          name: "",
          description: "",
          category: undefined,
          isActive: true,
        });
      }
    }
  }, [open, permission, isEditing, setValue, reset]);

  const onSubmit = async (data: PermissionFormData) => {
    setIsLoading(true);
    try {
      if (isEditing && permission) {
        await permissionApi.updatePermission(permission.id, {
          ...data,
          isActive: data.isActive ?? true,
        });
        toast.success("Permission updated successfully");
      } else {
        await permissionApi.createPermission({
          ...data,
          isActive: data.isActive ?? true,
        });
        toast.success("Permission created successfully");
      }
      onSuccess();
    } catch (error) {
      if (error instanceof Error && error.message.includes("not yet implemented")) {
        toast.error(error.message);
      } else {
        toast.error(error instanceof Error ? error.message : "Operation failed");
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>
            {isEditing ? "Edit Permission" : "Create Permission"}
          </DialogTitle>
          <DialogDescription>
            {isEditing 
              ? "Update permission information." 
              : "Create a new permission for the system."
            }
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="name">Permission Name</Label>
            <Input
              id="name"
              {...register("name")}
              disabled={isLoading}
              placeholder="Enter permission name"
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
              placeholder="Enter permission description (optional)"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="category">Category</Label>
            <Select 
              value={selectedCategory || undefined} 
              onValueChange={(value) => setValue("category", value === "none" ? undefined : value)}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select a category (optional)" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="none">No Category</SelectItem>
                {Object.values(PERMISSION_CATEGORIES).map((category) => (
                  <SelectItem key={category} value={category}>
                    {category}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
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
