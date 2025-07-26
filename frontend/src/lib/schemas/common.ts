import { z } from 'zod';

export const PaginationParamsSchema = z.object({
    skip: z.number().min(0).default(0),
    take: z.number().min(1).max(100).default(20),
    search: z.string().optional(),
    sortBy: z.string().optional(),
    sortDirection: z.number().int().min(0).max(1).optional()
});

export const createPagedResultSchema = <T extends z.ZodTypeAny>(itemSchema: T) =>
    z.object({
        items: z.array(itemSchema),
        totalCount: z.number().int(),
        skip: z.number().int(),
        take: z.number().int(),
        hasNext: z.boolean(),
        hasPrevious: z.boolean(),
    });

export const ApiErrorSchema = z.object({
    message: z.string(),
    code: z.string().optional(),
    details: z.record(z.any(), z.any()).optional(),
});

export type PaginationParams = z.infer<typeof PaginationParamsSchema>;
export type PagedResult<T> = {
    items: T[];
    totalCount: number;
    skip: number;
    take: number;
    hasNext: boolean;
    hasPrevious: boolean;
};
export type ApiError = z.infer<typeof ApiErrorSchema>;