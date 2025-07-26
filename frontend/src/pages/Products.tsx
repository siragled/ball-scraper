import { useState } from 'react';
import { useProducts } from '@/lib/hooks/useProducts';
import { useDebounce } from '@/lib/hooks/useDebounce';
import { ProductsTable } from '@/components/products/ProductsTable';
import { AddProductModal } from '@/components/products/AddProductModal';
import { Input } from '@/components/ui/input';
import { Search } from 'lucide-react';

export default function Products() {
    const [search, setSearch] = useState('');
    const debouncedSearch = useDebounce(search, 300);

    const { data, isLoading } = useProducts({
        skip: 0,
        take: 100,
        search: debouncedSearch || undefined,
    });

    if (isLoading) {
        return (
            <div className="p-6">
                <h1 className="text-3xl font-bold mb-2">Products</h1>
                <div>Loading...</div>
            </div>
        );
    }

    return (
        <div className="p-6 space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold">Products</h1>
                    <p className="text-muted-foreground">
                        {data?.totalCount || 0} products
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

            <ProductsTable products={data?.items || []} />
        </div>
    );
}