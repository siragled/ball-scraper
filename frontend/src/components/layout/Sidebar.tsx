import {
    LayoutDashboard,
    Bell,
    PackageSearch,
    Settings,
    LogOut,
} from "lucide-react"
import { Link, useLocation } from "react-router-dom"
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { useAuth } from "@/lib/providers/AuthProvider"

export function Sidebar() {
    const { logout, user } = useAuth();
    const location = useLocation()
    const pathname = location.pathname

    const navItems = [
        { to: "/", label: "Dashboard", icon: LayoutDashboard },
        { to: "/products", label: "Products", icon: PackageSearch },
        {
            to: "/notifications",
            label: "Notifications",
            icon: Bell,
            badge: 3,
        },
        { to: "/settings", label: "Settings", icon: Settings },
    ]

    const getInitial = (name: string | undefined) => {
        return name ? name.charAt(0).toUpperCase() : "U";
    };

    return (
        <aside className="h-screen w-64 bg-background text-foreground flex flex-col justify-between border-r border-border">
            <div className="p-4">
                <div className="mb-6 text-lg font-semibold">Ball Scraper</div>

                <nav className="space-y-1">
                    {navItems.map(({ to, label, icon: Icon, badge }) => (
                        <Link
                            key={to}
                            to={to}
                            className={cn(
                                "flex items-center justify-between px-3 py-2 rounded-md text-sm transition-all",
                                pathname === to
                                    ? "bg-accent text-accent-foreground"
                                    : "hover:bg-accent text-muted-foreground hover:text-foreground"
                            )}
                        >
                            <div className="flex items-center gap-3">
                                <Icon className="w-4 h-4" />
                                {label}
                            </div>
                            {badge !== undefined && (
                                <Badge className="text-xs" variant="secondary">
                                    {badge}
                                </Badge>
                            )}
                        </Link>
                    ))}
                </nav>
            </div>

            <div className="p-4 border-t border-border">
                <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                        {user ? (
                            <div className="w-8 h-8 flex items-center justify-center rounded-full bg-accent text-accent-foreground font-medium">
                                {getInitial(user.username)}
                            </div>
                        ) : (
                            <div className="w-8 h-8 rounded-full bg-muted animate-pulse"></div>
                        )}
                        <div className="text-sm">
                            {user ? (
                                <div className="font-medium leading-none">
                                    {user.username}
                                </div>
                            ) : (
                                <div className="w-20 h-4 bg-muted rounded animate-pulse"></div>
                            )}
                        </div>
                    </div>
                    <Button
                        variant="ghost"
                        size="icon"
                        className="text-muted-foreground hover:text-foreground"
                        onClick={logout}
                    >
                        <LogOut className="w-4 h-4" />
                    </Button>
                </div>
            </div>
        </aside>
    )
}