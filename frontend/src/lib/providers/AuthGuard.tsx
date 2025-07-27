import { Navigate } from 'react-router-dom';
import { useAuth } from '@/lib/providers/AuthProvider';

export const AuthGuard = ({ children }: { children: React.ReactNode }) => {
    const { user } = useAuth();

    return user ? <>{children}</> : <Navigate to="/login" replace />;
};