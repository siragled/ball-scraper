import { BrowserRouter, Routes, Route } from "react-router-dom"
import Dashboard from "@/pages/Dashboard"
import Products from "@/pages/Products"
import Notifications from "@/pages/Notifications"
import SettingsPage from "@/pages/Settings"
import { Sidebar } from "@/components/layout/Sidebar"

export default function App() {
  return (
    <BrowserRouter>
      <div className="flex min-h-screen">
        <Sidebar />
        <main className="flex-1 p-6 bg-background text-foreground">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/products" element={<Products />} />
            <Route path="/notifications" element={<Notifications />} />
            <Route path="/settings" element={<SettingsPage />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  )
}