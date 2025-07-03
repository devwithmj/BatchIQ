"use client";

import { ReactNode, useState } from "react";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";
import { NavigationMenu, NavigationMenuList } from "@/components/ui/navigation-menu";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { Menu } from "lucide-react";
import Link from "next/link";

export default function AdminLayout({ children }: { children: ReactNode }) {
  const [open, setOpen] = useState(false);

  return (
    <div className="min-h-screen flex flex-col bg-muted/20">
      {/* Topbar */}
      <header className="h-14 px-4 flex items-center border-b bg-background">
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
  const Item = ({ href, label }: { href: string; label: string }) => (
    <Link
      href={href}
      onClick={close}
      className="block px-4 py-2 rounded hover:bg-muted transition-colors"
    >
      {label}
    </Link>
  );

  return (
    <ScrollArea className="h-full">
      <nav className="py-4">
        <NavigationMenu>
          <NavigationMenuList className="flex flex-col gap-1">
            <Item href="/inventory"        label="Inventory" />
            <Item href="/transactions"     label="Transactions" />
            <Item href="/locations"        label="Locations" />
            <Separator className="my-2" />
            <Item href="/settings/products" label="Products" />
            <Item href="/settings/users"    label="Users & Roles" />
          </NavigationMenuList>
        </NavigationMenu>
      </nav>
    </ScrollArea>
  );
}
