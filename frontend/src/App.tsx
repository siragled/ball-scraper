import { BrowserRouter, Routes, Route } from "react-router-dom";
import Dashboard from "@/pages/Dashboard";
import Products from "@/pages/products/Products";
import Product from "@/pages/products/Product";
import Notifications from "@/pages/Notifications";
import SettingsPage from "@/pages/Settings";
import { Sidebar } from "@/components/layout/Sidebar";
import { QueryProvider } from "@/lib/providers/QueryProvider";
import { AuthProvider } from '@/lib/providers/AuthProvider';
import { AuthGuard } from '@/lib/providers/AuthGuard';
import { LoginPage } from '@/pages/auth/Login';

export default function App() {
  return (
    <AuthProvider>
      <QueryProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route
              path="*"
              element={
                <AuthGuard>
                  <Layout />
                </AuthGuard>
              }
            />
          </Routes>
        </BrowserRouter>
      </QueryProvider>
    </AuthProvider>
  );
}

function Layout() {
  return (
    <div className="flex h-screen overflow-hidden">
      <div className="sticky top-0 h-screen">
        <Sidebar />
      </div>
      <main className="flex-1 overflow-y-auto p-6 bg-background text-foreground">
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/products" element={<Products />} />
          <Route path="/products/:id" element={<Product />} />
          <Route path="/notifications" element={<Notifications />} />
          <Route path="/settings" element={<SettingsPage />} />
        </Routes>
      </main>
    </div>
  );
}