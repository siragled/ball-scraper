import { Navigate, useLocation } from "react-router-dom";
import { authAPI } from "@/lib/api/auth";
import type { JSX } from "react";

export const ProtectedRoute = ({ children }: { children: JSX.Element }) => {
    const location = useLocation();
    const isAuthenticated = authAPI.isAuthenticated();

    if (!isAuthenticated) {
        return (
            <Navigate
                to="/login"
                state={{ from: location }}
                replace
            />
        );
    }

    return children;
};