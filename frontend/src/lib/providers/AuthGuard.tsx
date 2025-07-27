import { Navigate } from 'react-router-dom';
import { useAuth } from '@/lib/providers/AuthProvider';
import { Loader } from 'lucide-react';

export const AuthGuard = ({ children }: { children: React.ReactNode }) => {
    const { user, isLoading } = useAuth();

    if (isLoading) {
        return (
            <div className="flex items-center justify-center h-screen">
                <Loader className="w-8 h-8 animate-spin text-black" />
            </div>
        );
    }

    return user ? <>{children}</> : <Navigate to="/login" replace />;
};