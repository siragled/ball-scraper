import { apiClient, authHelpers } from './client';
import {
    type LoginDto,
    type RegisterDto,
    type AuthResponse,
    LoginSchema,
    RegisterSchema,
    AuthResponseSchema,
} from '../schemas/auth';

export const authAPI = {
    register: async (data: RegisterDto): Promise<AuthResponse> => {
        const validatedData = RegisterSchema.parse(data);
        const response = await apiClient.post('/users', validatedData);
        const authResponse = AuthResponseSchema.parse(response.data);
        authHelpers.setUser(authResponse);
        return authResponse;
    },

    login: async (data: LoginDto): Promise<AuthResponse> => {
        const validatedData = LoginSchema.parse(data);
        const response = await apiClient.post('/users/login', validatedData);
        const authResponse = AuthResponseSchema.parse(response.data);
        authHelpers.setUser(authResponse);
        return authResponse;
    },

    me: async (): Promise<AuthResponse> => {
        const response = await apiClient.get('/users/me');
        const authResponse = AuthResponseSchema.parse(response.data);
        authHelpers.setUser(authResponse);
        return authResponse;
    },

    logout: () => {
        authHelpers.clearUser();
    },

    isAuthenticated: (): boolean => {
        return !!authHelpers.getUser();
    },
};