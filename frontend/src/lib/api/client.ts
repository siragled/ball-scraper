import axios from 'axios';
import type { ApiError } from '../schemas/common';

const apiClient = axios.create({
    baseURL: '/api',
    timeout: 10000,
});

apiClient.interceptors.request.use((config) => {
    const token = localStorage.getItem('auth_token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        const isAuthCheck = error.config?.url?.includes('/users/me');

        if (error.response?.status === 401 || error.response?.status === 403 && !isAuthCheck) {
            authHelpers.clearToken();
            window.location.href = "/login"
        }

        const apiError: ApiError = {
            message: error.response?.data?.message || error.message || 'An error occurred',
            code: error.response?.status?.toString() || 'NETWORK_ERROR',
        };
        return Promise.reject(apiError);
    }
);

export const authHelpers = {
    setToken: (token: string) => localStorage.setItem('auth_token', token),
    clearToken: () => localStorage.removeItem('auth_token'),
    getToken: () => localStorage.getItem('auth_token'),
};

export { apiClient };