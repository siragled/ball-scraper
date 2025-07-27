import { useMutation, useQuery, useQueryClient, type UseQueryResult } from '@tanstack/react-query';
import { productsAPI } from '../api/products';
import type {
    PagedProducts,
    ProductQueryParams,
    Product,
    CreateProduct,
    PagedProductSnapshots,
    ProductSnapshotsRequest,
} from '../schemas/product';

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

export const useProductSnapshots = (
    id: string,
    params: ProductSnapshotsRequest
): UseQueryResult<PagedProductSnapshots, Error> => {
    return useQuery({
        queryKey: ['products', 'snapshots', id, params],
        queryFn: () => productsAPI.getProductSnapshots(id, params),
        staleTime: 5 * 60 * 1000,
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