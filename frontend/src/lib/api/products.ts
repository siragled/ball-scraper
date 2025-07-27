import { apiClient } from './client';
import {
    type Product,
    type CreateProduct,
    type PagedProducts,
    type ProductQueryParams,
    ProductSchema,
    CreateProductSchema,
    PagedProductsSchema,
    type PagedProductSnapshots,
    type ProductSnapshotsRequest,
    PagedProductSnapshotsSchema,
} from '../schemas/product';

export const productsAPI = {
    getProducts: async (params: ProductQueryParams): Promise<PagedProducts> => {
        const searchParams = new URLSearchParams();

        Object.entries(params).forEach(([key, value]) => {
            if (value !== undefined && value !== null) {
                const apiKey = key.charAt(0).toUpperCase() + key.slice(1);
                searchParams.append(apiKey, String(value));
            }
        });

        const response = await apiClient.get(`/products?${searchParams.toString()}`);
        return PagedProductsSchema.parse(response.data);
    },

    getProduct: async (id: string): Promise<Product> => {
        const response = await apiClient.get(`/products/${id}`);
        return ProductSchema.parse(response.data);
    },

    createProduct: async (data: CreateProduct): Promise<Product> => {
        const validatedData = CreateProductSchema.parse(data);
        const response = await apiClient.post('/products', validatedData);
        return ProductSchema.parse(response.data);
    },

    getProductSnapshots: async (
        id: string,
        params: ProductSnapshotsRequest
    ): Promise<PagedProductSnapshots> => {
        const searchParams = new URLSearchParams();
        Object.entries(params).forEach(([key, value]) => {
            if (value !== undefined && value !== null) {
                const apiKey = key.charAt(0).toUpperCase() + key.slice(1);
                searchParams.append(apiKey, String(value));
            }
        });

        const response = await apiClient.get(`/products/${id}/snapshots?${searchParams.toString()}`);
        return PagedProductSnapshotsSchema.parse(response.data);
    },
};