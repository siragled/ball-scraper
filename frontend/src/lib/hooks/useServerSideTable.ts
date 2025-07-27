import { useState, useMemo } from 'react';
import {
    useReactTable,
    getCoreRowModel,
    type ColumnDef,
    type SortingState,
    type PaginationState,
    type Table,
} from '@tanstack/react-table';
import type { UseQueryResult } from '@tanstack/react-query';
import type { PagedResult, PaginationParams } from '../schemas/common';

interface UseServerSideTableProps<TData, TParams extends PaginationParams> {
    columns: ColumnDef<TData, any>[];
    queryHook: (params: TParams) => UseQueryResult<PagedResult<TData>, Error>;
    initialState?: {
        pagination?: PaginationState;
        sorting?: SortingState;
    };
    search?: string;
}

export const useServerSideTable = <TData, TParams extends PaginationParams>({
    columns,
    queryHook,
    initialState = {},
    search,
}: UseServerSideTableProps<TData, TParams>): {
    table: Table<TData>;
    query: UseQueryResult<PagedResult<TData>, Error>;
} => {
    const [sorting, setSorting] = useState<SortingState>(
        initialState.sorting ?? []
    );
    const [pagination, setPagination] = useState<PaginationState>(
        initialState.pagination ?? {
            pageIndex: 0,
            pageSize: 10,
        }
    );

    const params = useMemo(() => {
        const [sort] = sorting;

        return {
            skip: pagination.pageIndex * pagination.pageSize,
            take: pagination.pageSize,
            search: search || undefined,
            sortBy: sort?.id,
            sortDirection: sort ? (sort.desc ? 1 : 0) : undefined,
        } as TParams;
    }, [sorting, pagination, search]);

    const query = queryHook(params);
    const { data } = query;

    const pageCount = data?.totalCount
        ? Math.ceil(data.totalCount / pagination.pageSize)
        : -1;

    const table = useReactTable({
        data: data?.items ?? [],
        columns,
        getCoreRowModel: getCoreRowModel(),
        manualPagination: true,
        manualSorting: true,
        pageCount,
        onPaginationChange: setPagination,
        onSortingChange: setSorting,
        state: {
            pagination,
            sorting,
        },
    });

    return { table, query };
};