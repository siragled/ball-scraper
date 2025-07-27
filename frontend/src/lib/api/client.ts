import axios from 'axios';
import type { ApiError } from '../schemas/common';
import type { AuthResponse } from '../schemas/auth';

export const authHelpers = {
    setUser: (user: AuthResponse) => {
        localStorage.setItem('auth_user', JSON.stringify(user));
    },
    getUser: (): AuthResponse | null => {
        const userStr = localStorage.getItem('auth_user');
        if (!userStr) return null;
        try {
            return JSON.parse(userStr) as AuthResponse;
        } catch (error) {
            return null;
        }
    },
    getToken: (): string | null => {
        return authHelpers.getUser()?.token ?? null;
    },
    clearUser: () => {
        localStorage.removeItem('auth_user');
    },
};

export const apiClient = axios.create({
    baseURL: '/api',
    timeout: 10000,
});

apiClient.interceptors.request.use((config) => {
    const token = authHelpers.getToken();
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        const apiError: ApiError = {
            message: error.response?.data?.message || error.message || 'An error occurred',
            code: error.response?.status?.toString() || 'NETWORK_ERROR',
        };
        return Promise.reject(apiError);
    }
);