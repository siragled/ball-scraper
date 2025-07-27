import { useState } from 'react';
import {
    QueryClient,
    QueryClientProvider,
    QueryCache,
    MutationCache
} from '@tanstack/react-query';
import { useAuth } from './AuthProvider';
import type { ApiError } from '../schemas/common';

export const QueryProvider = ({ children }: { children: React.ReactNode }) => {
    const { logout } = useAuth();

    const [queryClient] = useState(() =>
        new QueryClient({
            queryCache: new QueryCache({
                onError: (error) => {
                    const apiError = error as ApiError;
                    if (apiError.code === '401' || apiError.code === '403') {
                        logout();
                    }
                },
            }),
            mutationCache: new MutationCache({
                onError: (error) => {
                    const apiError = error as ApiError;
                    if (apiError.code === '401' || apiError.code === '403') {
                        logout();
                    }
                },
            }),
            defaultOptions: {
                queries: {
                    staleTime: 1000 * 60 * 5,
                    retry: (failureCount, error) => {
                        const apiError = error as ApiError;
                        if (apiError.code === '401' || apiError.code === '403') {
                            return false;
                        }
                        return failureCount < 2;
                    },
                },
            },
        })
    );

    return (
        <QueryClientProvider client={queryClient}>
            {children}
        </QueryClientProvider>
    );
};