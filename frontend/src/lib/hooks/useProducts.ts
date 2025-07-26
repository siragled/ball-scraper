import { useMutation, useQuery, useQueryClient, type UseQueryResult } from '@tanstack/react-query';
import { productsAPI } from '../api/products';
import type { PagedProducts, ProductQueryParams, Product, CreateProduct } from '../schemas/product';

export const useProducts = (
    params: ProductQueryParams
): UseQueryResult<PagedProducts, Error> => {
    return useQuery({
        queryKey: ['products', 'list', params],
        queryFn: () => productsAPI.getProducts(params),
        staleTime: 5 * 60 * 1000,
        placeholderData: (previousData) => previousData,
    });
};

export const useProduct = (id: string): UseQueryResult<Product, Error> => {
    return useQuery({
        queryKey: ['products', 'detail', id],
        queryFn: () => productsAPI.getProduct(id),
        enabled: !!id,
    });
};

export const useCreateProduct = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateProduct) => productsAPI.createProduct(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['products', 'list'] });
        },
    });
};