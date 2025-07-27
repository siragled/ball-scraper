import { z } from 'zod';

export const RegisterSchema = z.object({
    email: z.email(),
    username: z.string().min(3),
    password: z.string().min(8),
    confirmPassword: z.string().min(8),
}).refine((data) => data.password === data.confirmPassword, {
    message: "Passwords don't match",
    path: ["confirmPassword"],
});

export const LoginSchema = z.object({
    username: z.string(), // can be email or username
    password: z.string().min(1),
});

export const AuthResponseSchema = z.object({
    token: z.string(),
    email: z.email(),
    username: z.string(),
    userId: z.uuid(),
});

export type RegisterDto = z.infer<typeof RegisterSchema>;
export type LoginDto = z.infer<typeof LoginSchema>;
export type AuthResponse = z.infer<typeof AuthResponseSchema>;