"use client";

import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { CreateUserDto, UpdateUserDto, User, createUserResolver, updateUserResolver } from "@/lib/auth-schema";
import { userApi, roleApi } from "@/lib/auth-api";
import { toast } from "sonner";
import { Checkbox } from "@/components/ui/checkbox";
import { Role } from "@/lib/auth-schema";

interface UserFormDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  user?: User | null;
  onSuccess: () => void;
}

function CreateUserForm({ onSuccess, onCancel }: { onSuccess: () => void; onCancel: () => void }) {
  const [isLoading, setIsLoading] = useState(false);
  const [roles, setRoles] = useState<Role[]>([]);

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    watch,
  } = useForm<CreateUserDto>({
    resolver: createUserResolver,
    defaultValues: {
      firstName: "",
      lastName: "",
      firstNameFa: "",
      lastNameFa: "",
      username: "",
      email: "",
      password: "",
      roleIds: [],
    },
  });

  const selectedRoleIds = watch("roleIds") || [];

  useEffect(() => {
    fetchRoles();
  }, []);

  const fetchRoles = async () => {
    try {
      const fetchedRoles = await roleApi.getRoles();
      setRoles(fetchedRoles.filter(r => r.isActive));
    } catch (err) {
      console.error('Failed to fetch roles:', err);
      toast.error("Failed to fetch roles");
    }
  };

  const onSubmit = async (data: CreateUserDto) => {
    setIsLoading(true);
    try {
      await userApi.createUser(data);
      toast.success("User created successfully");
      onSuccess();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Operation failed");
    } finally {
      setIsLoading(false);
    }
  };

  const handleRoleToggle = (roleId: number, checked: boolean) => {
    if (checked) {
      setValue("roleIds", [...selectedRoleIds, roleId]);
    } else {
      setValue("roleIds", selectedRoleIds.filter(id => id !== roleId));
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="firstName">First Name</Label>
          <Input
            id="firstName"
            {...register("firstName")}
            disabled={isLoading}
          />
          {errors.firstName && (
            <p className="text-sm text-red-600">{errors.firstName.message}</p>
          )}
        </div>

        <div className="space-y-2">
          <Label htmlFor="lastName">Last Name</Label>
          <Input
            id="lastName"
            {...register("lastName")}
            disabled={isLoading}
          />
          {errors.lastName && (
            <p className="text-sm text-red-600">{errors.lastName.message}</p>
          )}
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="firstNameFa">First Name (Farsi)</Label>
          <Input
            id="firstNameFa"
            {...register("firstNameFa")}
            disabled={isLoading}
          />
        </div>

        <div className="space-y-2">
          <Label htmlFor="lastNameFa">Last Name (Farsi)</Label>
          <Input
            id="lastNameFa"
            {...register("lastNameFa")}
            disabled={isLoading}
          />
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="username">Username</Label>
        <Input
          id="username"
          {...register("username")}
          disabled={isLoading}
        />
        {errors.username && (
          <p className="text-sm text-red-600">{errors.username.message}</p>
        )}
      </div>

      <div className="space-y-2">
        <Label htmlFor="email">Email</Label>
        <Input
          id="email"
          type="email"
          {...register("email")}
          disabled={isLoading}
        />
        {errors.email && (
          <p className="text-sm text-red-600">{errors.email.message}</p>
        )}
      </div>

      <div className="space-y-2">
        <Label htmlFor="password">Password</Label>
        <Input
          id="password"
          type="password"
          {...register("password")}
          disabled={isLoading}
        />
        {errors.password && (
          <p className="text-sm text-red-600">{errors.password.message}</p>
        )}
      </div>

      <div className="space-y-2">
        <Label>Roles</Label>
        <div className="space-y-2 max-h-32 overflow-y-auto">
          {roles.map((role) => (
            <div key={role.id} className="flex items-center space-x-2">
              <Checkbox
                id={`role-${role.id}`}
                checked={selectedRoleIds.includes(role.id)}
                onCheckedChange={(checked) => 
                  handleRoleToggle(role.id, checked as boolean)
                }
              />
              <Label htmlFor={`role-${role.id}`} className="text-sm">
                {role.name}
                {role.description && (
                  <span className="text-gray-500 ml-1">- {role.description}</span>
                )}
              </Label>
            </div>
          ))}
        </div>
      </div>

      <DialogFooter>
        <Button
          type="button"
          variant="outline"
          onClick={onCancel}
          disabled={isLoading}
        >
          Cancel
        </Button>
        <Button type="submit" disabled={isLoading}>
          {isLoading ? "Creating..." : "Create"}
        </Button>
      </DialogFooter>
    </form>
  );
}

function EditUserForm({ user, onSuccess, onCancel }: { user: User; onSuccess: () => void; onCancel: () => void }) {
  const [isLoading, setIsLoading] = useState(false);
  const [roles, setRoles] = useState<Role[]>([]);

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    watch,
  } = useForm<UpdateUserDto>({
    resolver: updateUserResolver,
    defaultValues: {
      firstName: user.firstName,
      lastName: user.lastName,
      firstNameFa: user.firstNameFa || "",
      lastNameFa: user.lastNameFa || "",
      isActive: user.isActive,
      roleIds: user.roles?.map(r => r.id) || [],
    },
  });

  const selectedRoleIds = watch("roleIds") || [];

  useEffect(() => {
    fetchRoles();
  }, []);

  const fetchRoles = async () => {
    try {
      const fetchedRoles = await roleApi.getRoles();
      setRoles(fetchedRoles.filter(r => r.isActive));
    } catch (err) {
      console.error('Failed to fetch roles:', err);
      toast.error("Failed to fetch roles");
    }
  };

  const onSubmit = async (data: UpdateUserDto) => {
    setIsLoading(true);
    try {
      await userApi.updateUser(user.id, data);
      toast.success("User updated successfully");
      onSuccess();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Operation failed");
    } finally {
      setIsLoading(false);
    }
  };

  const handleRoleToggle = (roleId: number, checked: boolean) => {
    if (checked) {
      setValue("roleIds", [...selectedRoleIds, roleId]);
    } else {
      setValue("roleIds", selectedRoleIds.filter(id => id !== roleId));
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="firstName">First Name</Label>
          <Input
            id="firstName"
            {...register("firstName")}
            disabled={isLoading}
          />
          {errors.firstName && (
            <p className="text-sm text-red-600">{errors.firstName.message}</p>
          )}
        </div>

        <div className="space-y-2">
          <Label htmlFor="lastName">Last Name</Label>
          <Input
            id="lastName"
            {...register("lastName")}
            disabled={isLoading}
          />
          {errors.lastName && (
            <p className="text-sm text-red-600">{errors.lastName.message}</p>
          )}
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="firstNameFa">First Name (Farsi)</Label>
          <Input
            id="firstNameFa"
            {...register("firstNameFa")}
            disabled={isLoading}
          />
        </div>

        <div className="space-y-2">
          <Label htmlFor="lastNameFa">Last Name (Farsi)</Label>
          <Input
            id="lastNameFa"
            {...register("lastNameFa")}
            disabled={isLoading}
          />
        </div>
      </div>

      <div className="flex items-center space-x-2">
        <Checkbox
          id="isActive"
          checked={watch("isActive")}
          onCheckedChange={(checked) => setValue("isActive", checked as boolean)}
        />
        <Label htmlFor="isActive">Active</Label>
      </div>

      <div className="space-y-2">
        <Label>Roles</Label>
        <div className="space-y-2 max-h-32 overflow-y-auto">
          {roles.map((role) => (
            <div key={role.id} className="flex items-center space-x-2">
              <Checkbox
                id={`role-${role.id}`}
                checked={selectedRoleIds.includes(role.id)}
                onCheckedChange={(checked) => 
                  handleRoleToggle(role.id, checked as boolean)
                }
              />
              <Label htmlFor={`role-${role.id}`} className="text-sm">
                {role.name}
                {role.description && (
                  <span className="text-gray-500 ml-1">- {role.description}</span>
                )}
              </Label>
            </div>
          ))}
        </div>
      </div>

      <DialogFooter>
        <Button
          type="button"
          variant="outline"
          onClick={onCancel}
          disabled={isLoading}
        >
          Cancel
        </Button>
        <Button type="submit" disabled={isLoading}>
          {isLoading ? "Updating..." : "Update"}
        </Button>
      </DialogFooter>
    </form>
  );
}

export function UserFormDialog({ open, onOpenChange, user, onSuccess }: UserFormDialogProps) {
  const isEditing = !!user;

  const handleCancel = () => {
    onOpenChange(false);
  };

  const handleSuccess = () => {
    onOpenChange(false);
    onSuccess();
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>
            {isEditing ? "Edit User" : "Create User"}
          </DialogTitle>
          <DialogDescription>
            {isEditing 
              ? "Update user information and roles." 
              : "Create a new user account."
            }
          </DialogDescription>
        </DialogHeader>

        {isEditing && user ? (
          <EditUserForm user={user} onSuccess={handleSuccess} onCancel={handleCancel} />
        ) : (
          <CreateUserForm onSuccess={handleSuccess} onCancel={handleCancel} />
        )}
      </DialogContent>
    </Dialog>
  );
}
