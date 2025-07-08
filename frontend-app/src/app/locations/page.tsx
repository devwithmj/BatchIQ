"use client";

import { useState, useMemo } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Search, Pencil, Trash2, ChevronRight, ChevronDown } from "lucide-react";
import { toast } from "sonner";

import AdminLayout from "@/components/layout/AdminLayout";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import LocationFormDialog from "@/components/location/LocationFormDialog";

import { api } from "@/lib/api";
import { Location, LocationTypes } from "@/lib/location-schema";

export default function LocationsPage() {
  const [searchTerm, setSearchTerm] = useState("");
  const [expandedNodes, setExpandedNodes] = useState<Set<number>>(new Set());
  const qc = useQueryClient();

  const { data: locations, isLoading } = useQuery<Location[]>({
    queryKey: ["locations"],
    queryFn: async () => (await api.get("/api/locations")).data,
  });

  const deleteMutation = useMutation({
    mutationFn: async (id: number) => {
      await api.delete(`/api/locations/${id}`);
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["locations"] });
      toast("Location Deleted ✅");
    },
    onError: () => {
      toast("Delete Failed ❌");
    },
  });

  // Build hierarchical structure
  const hierarchicalLocations = useMemo(() => {
    if (!locations) return [];
    
    type LocationWithChildren = Location & { children: LocationWithChildren[] };
    const locationMap = new Map<number, LocationWithChildren>();
    const roots: LocationWithChildren[] = [];

    // Initialize all locations with empty children array
    locations.forEach(loc => {
      locationMap.set(loc.id, { ...loc, children: [] });
    });

    // Build hierarchy
    locations.forEach(loc => {
      const locationWithChildren = locationMap.get(loc.id)!;
      if (loc.parentLocationId) {
        const parent = locationMap.get(loc.parentLocationId);
        if (parent) {
          parent.children.push(locationWithChildren);
        } else {
          roots.push(locationWithChildren);
        }
      } else {
        roots.push(locationWithChildren);
      }
    });

    return roots;
  }, [locations]);

  // Filter locations based on search
  const filteredLocations = useMemo(() => {
    if (!searchTerm) return hierarchicalLocations;
    
    type LocationWithChildren = Location & { children: LocationWithChildren[] };
    
    const matchesSearch = (location: LocationWithChildren): boolean => {
      const nameMatch = location.name.toLowerCase().includes(searchTerm.toLowerCase());
      const typeMatch = LocationTypes[location.locationType as keyof typeof LocationTypes]
        .toLowerCase().includes(searchTerm.toLowerCase());
      const childMatch = location.children.some(child => matchesSearch(child as LocationWithChildren));
      
      return nameMatch || typeMatch || childMatch;
    };

    const filterRecursively = (locations: LocationWithChildren[]): LocationWithChildren[] => {
      return locations
        .filter(matchesSearch)
        .map(location => ({
          ...location,
          children: filterRecursively(location.children)
        }));
    };

    return filterRecursively(hierarchicalLocations);
  }, [hierarchicalLocations, searchTerm]);

  const toggleExpanded = (id: number) => {
    const newExpanded = new Set(expandedNodes);
    if (newExpanded.has(id)) {
      newExpanded.delete(id);
    } else {
      newExpanded.add(id);
    }
    setExpandedNodes(newExpanded);
  };

  const handleDelete = (id: number, name: string) => {
    if (confirm(`Are you sure you want to delete "${name}"? This action cannot be undone.`)) {
      deleteMutation.mutate(id);
    }
  };

  const renderLocationNode = (location: Location & { children: (Location & { children: any[] })[] }, level = 0) => {
    const hasChildren = location.children.length > 0;
    const isExpanded = expandedNodes.has(location.id);
    
    return (
      <div key={location.id} className="w-full">
        <div 
          className="flex items-center justify-between p-3 border-b border-border hover:bg-muted/50 transition-colors"
          style={{ paddingLeft: `${level * 24 + 12}px` }}
        >
          <div className="flex items-center space-x-2 flex-1">
            {hasChildren ? (
              <Button
                variant="ghost"
                size="sm"
                className="h-6 w-6 p-0"
                onClick={() => toggleExpanded(location.id)}
              >
                {isExpanded ? (
                  <ChevronDown className="h-4 w-4" />
                ) : (
                  <ChevronRight className="h-4 w-4" />
                )}
              </Button>
            ) : (
              <div className="w-6" />
            )}
            
            <div className="flex flex-col">
              <div className="flex items-center space-x-2">
                <span className="font-medium">{location.name}</span>
                <span className="inline-block bg-primary/10 text-primary px-2 py-1 rounded text-xs font-medium">
                  {LocationTypes[location.locationType as keyof typeof LocationTypes]}
                </span>
              </div>
              {hasChildren && (
                <span className="text-xs text-muted-foreground">
                  {location.children.length} child location{location.children.length !== 1 ? 's' : ''}
                </span>
              )}
            </div>
          </div>

          <div className="flex items-center space-x-1">
            <LocationFormDialog
              defaultValues={{
                id: location.id,
                name: location.name,
                locationType: location.locationType,
                parentLocationId: location.parentLocationId,
              }}
            >
              <Button variant="ghost" size="sm">
                <Pencil className="h-4 w-4" />
              </Button>
            </LocationFormDialog>
            
            <Button
              variant="ghost"
              size="sm"
              onClick={() => handleDelete(location.id, location.name)}
              disabled={deleteMutation.isPending}
            >
              <Trash2 className="h-4 w-4 text-red-500" />
            </Button>
          </div>
        </div>
        
        {hasChildren && isExpanded && (
          <div>
            {location.children.map(child => renderLocationNode(child as any, level + 1))}
          </div>
        )}
      </div>
    );
  };

  if (isLoading) return <p className="p-4">Loading locations...</p>;

  return (
    <AdminLayout>
      {/* Header with search and add button */}
      <div className="flex items-center justify-between mb-6">
        <div className="relative max-w-md flex-1">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground h-4 w-4" />
          <Input
            placeholder="Search locations by name or type..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
        <LocationFormDialog />
      </div>

      {/* Locations Tree */}
      <Card>
        <CardHeader>
          <CardTitle>
            Locations ({locations?.length || 0})
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          {filteredLocations.length === 0 ? (
            <div className="text-center p-8 text-muted-foreground">
              {searchTerm ? "No locations found matching your search." : "No locations available."}
            </div>
          ) : (
            <div className="divide-y divide-border">
              {filteredLocations.map(location => renderLocationNode(location))}
            </div>
          )}
        </CardContent>
      </Card>
    </AdminLayout>
  );
}
