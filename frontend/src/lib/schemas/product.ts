import { z } from 'zod';
import { createPagedResultSchema, PaginationParamsSchema } from './common';

export const ProductSchema = z.object({
    id: z.uuid(),
    name: z.string(),
    description: z.string().nullable(),
    imageUrl: z.string().nullable(),
    brand: z.string().nullable(),
    sourceUrl: z.url(),
    storeName: z.string().nullable(),
    lastPrice: z.number(),
    createdAt: z.iso.datetime(),
});

export const CreateProductSchema = z.object({
    sourceUrl: z.url(),
});

export const ProductFiltersSchema = z.object({
    brand: z.string().optional(),
    minPrice: z.number().min(0).optional(),
    maxPrice: z.number().min(0).optional(),
});

export const ProductQueryParamsSchema = PaginationParamsSchema.extend(ProductFiltersSchema.shape);
export const PagedProductsSchema = createPagedResultSchema(ProductSchema);

export type Product = z.infer<typeof ProductSchema>;
export type CreateProduct = z.infer<typeof CreateProductSchema>;
export type ProductFilters = z.infer<typeof ProductFiltersSchema>;
export type ProductQueryParams = z.infer<typeof ProductQueryParamsSchema>;
export type PagedProducts = z.infer<typeof PagedProductsSchema>;