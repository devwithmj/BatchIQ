"use client";

import { ReactNode, useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";
import { NavigationMenu, NavigationMenuList } from "@/components/ui/navigation-menu";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Menu, LogOut, User } from "lucide-react";
import Link from "next/link";
import { useAuth, PermissionGate } from "@/lib/auth-context";
import { PERMISSIONS } from "@/lib/auth-schema";
import EnvBadge from "@/components/EnvBadge";

export default function AdminLayout({ children }: { children: ReactNode }) {
  const [open, setOpen] = useState(false);
  const { user, logout, isAuthenticated, isLoading } = useAuth();
  const router = useRouter();

  // Handle authentication redirect on client side only
  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      router.push("/login");
    }
  }, [isAuthenticated, isLoading, router]);

  // Show loading or redirect to login if not authenticated
  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto"></div>
          <p className="mt-2 text-sm text-gray-600">Loading...</p>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return null; // Will redirect via useEffect
  }

  const handleLogout = async () => {
    await logout();
    router.push("/login");
  };

  return (
    <div className="min-h-screen flex flex-col bg-muted/20">
      {/* Topbar */}
      <header className="h-14 px-4 flex items-center justify-between border-b bg-background">
        <div className="flex items-center">
          <Sheet open={open} onOpenChange={setOpen}>
            <SheetTrigger asChild>
              <button className="md:hidden">
                <Menu className="h-5 w-5" />
              </button>
            </SheetTrigger>
            <SheetContent side="left" className="p-0">
              <Sidebar close={() => setOpen(false)} />
            </SheetContent>
          </Sheet>
          <h1 className="font-semibold tracking-tight pl-2">BatchIQ Portal</h1>
          <EnvBadge />
        </div>

        <div className="flex items-center gap-4">
          {user && (
            <div className="flex items-center gap-2">
              <User className="h-4 w-4" />
              <span className="text-sm font-medium">
                {user.firstName} {user.lastName}
              </span>
              {user.roles && user.roles.length > 0 && (
                <Badge variant="outline" className="text-xs">
                  {user.roles[0].name}
                  {user.roles.length > 1 && ` +${user.roles.length - 1}`}
                </Badge>
              )}
            </div>
          )}
          <Button variant="ghost" size="sm" onClick={handleLogout}>
            <LogOut className="h-4 w-4" />
          </Button>
        </div>
      </header>

      <div className="flex flex-1">
        {/* Desktop sidebar */}
        <aside className="hidden md:block w-64 border-r bg-background">
          <Sidebar />
        </aside>

        {/* Main content */}
        <main className="flex-1 p-4">{children}</main>
      </div>
    </div>
  );
}

function Sidebar({ close }: { close?: () => void }) {
  const Item = ({ href, label, permission }: { href: string; label: string; permission?: string }) => (
    <PermissionGate permissions={permission ? [permission] : []}>
      <Link
        href={href}
        onClick={close}
        className="block px-4 py-2 rounded hover:bg-muted transition-colors"
      >
        {label}
      </Link>
    </PermissionGate>
  );

  return (
    <ScrollArea className="h-full">
      <nav className="py-4">
        <NavigationMenu>
          <NavigationMenuList className="flex flex-col gap-1">
            <Item href="/inventory" label="Inventory" permission={PERMISSIONS.VIEW_INVENTORY} />
            <Item href="/transactions" label="Transactions" permission={PERMISSIONS.VIEW_TRANSACTIONS} />
            <Item href="/locations" label="Locations" />
            
            <Separator className="my-2" />
            
            <Item href="/products" label="Products" permission={PERMISSIONS.VIEW_PRODUCTS} />
            
            <Separator className="my-2" />
            
            <PermissionGate permissions={[PERMISSIONS.VIEW_USERS, PERMISSIONS.SYSTEM_ADMIN]} requireAll={false}>
              <div className="px-4 py-1">
                <span className="text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Administration
                </span>
              </div>
              <Item href="/admin/users" label="User Management" permission={PERMISSIONS.VIEW_USERS} />
              <Item href="/admin/roles" label="Role Management" permission={PERMISSIONS.SYSTEM_ADMIN} />
            </PermissionGate>
          </NavigationMenuList>
        </NavigationMenu>
      </nav>
    </ScrollArea>
  );
}
