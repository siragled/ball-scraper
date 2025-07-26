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

        const response = await apiClient.post('/auth/register', validatedData);
        const authResponse = AuthResponseSchema.parse(response.data);

        authHelpers.setToken(authResponse.token);
        return authResponse;
    },

    login: async (data: LoginDto): Promise<AuthResponse> => {
        const validatedData = LoginSchema.parse(data);

        const response = await apiClient.post('/auth/login', validatedData);
        const authResponse = AuthResponseSchema.parse(response.data);

        authHelpers.setToken(authResponse.token);
        return authResponse;
    },

    logout: () => {
        authHelpers.clearToken();
    },

    isAuthenticated: (): boolean => {
        return !!authHelpers.getToken();
    },
};