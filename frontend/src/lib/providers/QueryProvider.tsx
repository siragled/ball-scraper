import React from 'react';
import {
    QueryClient,
    QueryClientProvider,
    type DefaultOptions,
} from '@tanstack/react-query';

const queryConfig: DefaultOptions = {
    queries: {
        retry: (failureCount, error: any) => {
            if (error?.code?.startsWith('4')) return false;
            return failureCount < 3;
        },
        retryDelay: (attemptIndex) => Math.min(1000 * 2 ** attemptIndex, 30000),
        staleTime: 5 * 60 * 1000,
        gcTime: 10 * 60 * 1000,
        refetchOnWindowFocus: false,
    },
};

const queryClient = new QueryClient({
    defaultOptions: queryConfig,
});

interface QueryProviderProps {
    children: React.ReactNode;
}

export const QueryProvider: React.FC<QueryProviderProps> = ({ children }) => {
    return (
        <QueryClientProvider client={queryClient}>
            {children}
        </QueryClientProvider>
    );
};