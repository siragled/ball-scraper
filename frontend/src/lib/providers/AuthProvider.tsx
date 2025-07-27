import { createContext, useContext, useEffect, useState } from 'react';
import { authAPI } from '@/lib/api/auth';
import type { AuthResponse } from '@/lib/schemas/auth';

type AuthContextType = {
    user: AuthResponse | null;
    login: (vars: Parameters<typeof authAPI.login>[0]) => Promise<AuthResponse>;
    register: (vars: Parameters<typeof authAPI.register>[0]) => Promise<AuthResponse>;
    logout: () => void;
    isLoading: boolean;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
    const [user, setUser] = useState<AuthResponse | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const initializeAuth = () => {
            if (authAPI.isAuthenticated()) {
                setUser({} as AuthResponse);
            }
            setIsLoading(false);
        };

        initializeAuth();
    }, []);

    const login = async (vars: Parameters<typeof authAPI.login>[0]) => {
        const userData = await authAPI.login(vars);
        setUser(userData);
        return userData;
    };

    const register = async (vars: Parameters<typeof authAPI.register>[0]) => {
        const userData = await authAPI.register(vars);
        setUser(userData);
        return userData;
    };

    const logout = () => {
        authAPI.logout();
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, login, register, logout, isLoading }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};