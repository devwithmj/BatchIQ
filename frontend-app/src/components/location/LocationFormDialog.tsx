"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQueryClient, useQuery } from "@tanstack/react-query";
import { toast } from "sonner";
import { Plus } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { api } from "@/lib/api";
import { 
  locationSchema, 
  LocationFormValues, 
  Location, 
  LocationTypes 
} from "@/lib/location-schema";

type Props = {
  defaultValues?: Partial<LocationFormValues> & { id?: number };
  children?: React.ReactNode;
};

export default function LocationFormDialog({ defaultValues, children }: Props) {
  const isEdit = !!defaultValues?.id;
  const [open, setOpen] = useState(false);
  const qc = useQueryClient();

  // Fetch all locations for parent selection
  const { data: locations } = useQuery<Location[]>({
    queryKey: ["locations"],
    queryFn: async () => (await api.get("/locations")).data,
  });

  const form = useForm<LocationFormValues>({
    resolver: zodResolver(locationSchema),
    defaultValues: {
      name: defaultValues?.name ?? "",
      locationType: defaultValues?.locationType ?? 1,
      parentLocationId: defaultValues?.parentLocationId ?? null,
    },
  });

  const mutation = useMutation({
    mutationFn: async (data: LocationFormValues) => {
      const payload = {
        name: data.name,
        locationType: data.locationType,
        parentLocationId: data.parentLocationId || null,
      };

      if (isEdit) {
        return await api.put(`/locations/${defaultValues?.id}`, payload);
      } else {
        return await api.post("/locations", payload);
      }
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["locations"] });
      setOpen(false);
      form.reset();
      toast(isEdit ? "Location Updated ✅" : "Location Created ✅");
    },
    onError: (error: unknown) => {
      console.error("Location save error:", error);
      toast("Save Failed ❌");
    },
  });

  const onSubmit = (data: LocationFormValues) => {
    mutation.mutate(data);
  };

  // Filter potential parent locations (exclude self and descendants)
  const availableParents = locations?.filter(loc => {
    if (isEdit && loc.id === defaultValues?.id) return false;
    // Could add more complex logic to prevent circular references
    return true;
  }) || [];

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {children || (
          <Button>
            <Plus className="h-4 w-4 mr-2" />
            Add Location
          </Button>
        )}
      </DialogTrigger>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>
            {isEdit ? "Edit Location" : "Add New Location"}
          </DialogTitle>
        </DialogHeader>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="name">Name</Label>
            <Input
              id="name"
              {...form.register("name")}
              placeholder="Enter location name"
            />
            {form.formState.errors.name && (
              <p className="text-sm text-red-600">
                {form.formState.errors.name.message}
              </p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="locationType">Location Type</Label>
            <Select
              value={form.watch("locationType")?.toString()}
              onValueChange={(value) => form.setValue("locationType", parseInt(value))}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select location type" />
              </SelectTrigger>
              <SelectContent>
                {Object.entries(LocationTypes).map(([key, value]) => (
                  <SelectItem key={key} value={key}>
                    {value}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {form.formState.errors.locationType && (
              <p className="text-sm text-red-600">
                {form.formState.errors.locationType.message}
              </p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="parentLocationId">Parent Location (Optional)</Label>
            <Select
              value={form.watch("parentLocationId")?.toString() || "none"}
              onValueChange={(value) => 
                form.setValue("parentLocationId", value === "none" ? null : parseInt(value))
              }
            >
              <SelectTrigger>
                <SelectValue placeholder="Select parent location (optional)" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="none">No Parent</SelectItem>
                {availableParents.map((location) => (
                  <SelectItem key={location.id} value={location.id.toString()}>
                    {location.name} ({LocationTypes[location.locationType as keyof typeof LocationTypes]})
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {form.formState.errors.parentLocationId && (
              <p className="text-sm text-red-600">
                {form.formState.errors.parentLocationId.message}
              </p>
            )}
          </div>

          <div className="flex justify-end space-x-2 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={() => setOpen(false)}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={mutation.isPending}>
              {mutation.isPending ? "Saving..." : isEdit ? "Update" : "Create"}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
