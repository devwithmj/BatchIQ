"use client";

import AdminLayout from "@/components/layout/AdminLayout";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { useAuth } from "@/lib/auth-context";

export default function Home() {
  const { user, permissions } = useAuth();

  return (
    <AdminLayout>
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-bold">Dashboard</h1>
          <p className="text-gray-600">Welcome to BatchIQ Manufacturing Management System</p>
        </div>

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          <Card>
            <CardHeader>
              <CardTitle className="text-lg">Welcome Back!</CardTitle>
              <CardDescription>
                {user ? `${user.firstName} ${user.lastName}` : "User"}
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-2">
                <div>
                  <span className="text-sm font-medium">Username: </span>
                  <span className="text-sm">{user?.username}</span>
                </div>
                <div>
                  <span className="text-sm font-medium">Email: </span>
                  <span className="text-sm">{user?.email}</span>
                </div>
                <div>
                  <span className="text-sm font-medium">Status: </span>
                  <Badge variant={user?.isActive ? "default" : "secondary"}>
                    {user?.isActive ? "Active" : "Inactive"}
                  </Badge>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className="text-lg">Your Roles</CardTitle>
              <CardDescription>Assigned roles and permissions</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-2">
                {user?.roles && user.roles.length > 0 ? (
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
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className="text-lg">System Access</CardTitle>
              <CardDescription>Your permission summary</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-2">
                <div>
                  <span className="text-sm font-medium">Permissions: </span>
                  <span className="text-sm">{permissions.length} total</span>
                </div>
                {permissions.length > 0 && (
                  <div className="flex flex-wrap gap-1 max-h-20 overflow-y-auto">
                    {permissions.slice(0, 3).map((permission) => (
                      <Badge key={permission} variant="secondary" className="text-xs">
                        {permission}
                      </Badge>
                    ))}
                    {permissions.length > 3 && (
                      <Badge variant="secondary" className="text-xs">
                        +{permissions.length - 3} more
                      </Badge>
                    )}
                  </div>
                )}
              </div>
            </CardContent>
          </Card>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Quick Actions</CardTitle>
            <CardDescription>Commonly used features</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
              <a
                href="/inventory"
                className="p-4 border rounded-lg hover:bg-muted transition-colors"
              >
                <h3 className="font-medium">Inventory</h3>
                <p className="text-sm text-gray-600">Manage stock levels</p>
              </a>
              <a
                href="/transactions"
                className="p-4 border rounded-lg hover:bg-muted transition-colors"
              >
                <h3 className="font-medium">Transactions</h3>
                <p className="text-sm text-gray-600">View inventory movements</p>
              </a>
              <a
                href="/products"
                className="p-4 border rounded-lg hover:bg-muted transition-colors"
              >
                <h3 className="font-medium">Products</h3>
                <p className="text-sm text-gray-600">Manage product catalog</p>
              </a>
              <a
                href="/admin/users"
                className="p-4 border rounded-lg hover:bg-muted transition-colors"
              >
                <h3 className="font-medium">User Management</h3>
                <p className="text-sm text-gray-600">Manage users and roles</p>
              </a>
            </div>
          </CardContent>
        </Card>
      </div>
    </AdminLayout>
  );
}
