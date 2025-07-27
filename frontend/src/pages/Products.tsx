import { useState } from 'react';
import { useProducts } from '@/lib/hooks/useProducts';
import { useDebounce } from '@/lib/hooks/useDebounce';
import { useServerSideTable } from '@/lib/hooks/useServerSideTable';
import { AddProductModal } from '@/components/products/AddProductModal';
import { DataTable } from '@/components/ui/data-table/data-table';
import { DataTablePagination } from '@/components/ui/data-table/data-table-pagination';
import { DataTableSkeleton } from '@/components/ui/data-table/data-table-skeleton';
import { productsColumns } from '@/components/products/ProductColumns';
import { Input } from '@/components/ui/input';
import { Search } from 'lucide-react';

export default function Products() {
    const [search, setSearch] = useState('');
    const debouncedSearch = useDebounce(search, 300);

    const { table, query } = useServerSideTable({
        columns: productsColumns,
        queryHook: useProducts,
        search: debouncedSearch,
    });

    const { data, isLoading } = query;
    const columnCount = table.getAllColumns().length;

    return (
        <div className="p-6 space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold">Products</h1>
                    <p className="text-muted-foreground">
                        {data?.totalCount ? `${data.totalCount} products` : '...'}
                    </p>
                </div>
                <AddProductModal />
            </div>

            <div className="relative max-w-sm">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                    placeholder="Search products..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    className="pl-9"
                />
            </div>

            {isLoading && !data ? (
                <DataTableSkeleton columnCount={columnCount} />
            ) : (
                <div className="space-y-4">
                    <DataTable table={table} />
                    <DataTablePagination
                        table={table}
                        isPlaceholderData={query.isPlaceholderData}
                    />
                </div>
            )}
        </div>
    );
}