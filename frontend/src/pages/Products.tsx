import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useProducts } from '@/lib/hooks/useProducts';
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from '@/components/ui/table';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Search, ExternalLink, Plus } from 'lucide-react';
import { useDebounce } from '@/lib/hooks/useDebounce';

export default function Products() {
    const [search, setSearch] = useState('');
    const debouncedSearch = useDebounce(search, 300);

    const { data, isLoading } = useProducts({
        skip: 0,
        take: 100,
        search: debouncedSearch || undefined,
    });

    const formatPrice = (price: number) => {
        return new Intl.NumberFormat('nl-NL', {
            style: 'currency',
            currency: 'EUR',
        }).format(price);
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString();
    };

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
            {/* Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold">Products</h1>
                    <p className="text-muted-foreground">
                        {data?.totalCount || 0} products
                    </p>
                </div>
                <Button asChild>
                    <Link to="/products/new">
                        <Plus className="h-4 w-4 mr-2" />
                        Add Product
                    </Link>
                </Button>
            </div>

            {/* Search */}
            <div className="relative max-w-sm">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                    placeholder="Search products..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    className="pl-9"
                />
            </div>

            {/* Products Table */}
            <div className="border rounded-md">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead className="w-16"></TableHead>
                            <TableHead>Product</TableHead>
                            <TableHead>Price</TableHead>
                            <TableHead>Last Updated</TableHead>
                            <TableHead className="w-20"></TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {data?.items.length === 0 ? (
                            <TableRow>
                                <TableCell colSpan={5} className="h-24 text-center">
                                    No products found.
                                </TableCell>
                            </TableRow>
                        ) : (
                            data?.items.map((product) => (
                                <TableRow key={product.id} className="group">
                                    <TableCell>
                                        <Link to={`/products/${product.id}`}>
                                            <div className="w-12 h-12 bg-muted rounded overflow-hidden">
                                                {product.imageUrl && (
                                                    <img
                                                        src={product.imageUrl}
                                                        alt={product.name}
                                                        className="w-full h-full object-cover"
                                                    />
                                                )}
                                            </div>
                                        </Link>
                                    </TableCell>
                                    <TableCell>
                                        <Link
                                            to={`/products/${product.id}`}
                                            className="block hover:underline"
                                        >
                                            <div>
                                                <div className="font-medium">{product.name}</div>
                                                {product.description && (
                                                    <div className="text-sm text-muted-foreground line-clamp-2 mt-1">
                                                        {product.description}
                                                    </div>
                                                )}
                                            </div>
                                        </Link>
                                    </TableCell>
                                    <TableCell className="font-medium">
                                        {formatPrice(product.lastPrice)}
                                    </TableCell>
                                    <TableCell className="text-sm">
                                        {formatDate(product.createdAt)}
                                    </TableCell>
                                    <TableCell>
                                        <Button asChild size="sm" variant="outline">
                                            <a
                                                href={product.sourceUrl}
                                                target="_blank"
                                                rel="noopener noreferrer"
                                            >
                                                <ExternalLink className="h-3 w-3" />
                                            </a>
                                        </Button>
                                    </TableCell>
                                </TableRow>
                            ))
                        )}
                    </TableBody>
                </Table>
            </div>
        </div>
    );
}