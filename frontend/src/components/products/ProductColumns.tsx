import { Link } from 'react-router-dom';
import type { ColumnDef } from '@tanstack/react-table';
import { DataTableColumnHeader } from '@/components/ui/data-table/data-table-column-header';
import { Button } from '@/components/ui/button';
import { ExternalLink } from 'lucide-react';
import type { Product } from '@/lib/schemas/product';
import { formatDate, formatPrice } from '@/lib/utils';

export const productsColumns: ColumnDef<Product>[] = [
    {
        accessorKey: 'imageUrl',
        header: '',
        cell: ({ row }) => (
            <Link to={`/products/${row.original.id}`}>
                <div className="w-12 h-12 bg-muted rounded overflow-hidden">
                    {row.original.imageUrl && (
                        <img
                            src={row.original.imageUrl}
                            alt={row.original.name}
                            className="w-full h-full object-contain"
                        />
                    )}
                </div>
            </Link>
        ),
        enableSorting: false,
    },
    {
        accessorKey: 'name',
        header: ({ column }) => (
            <DataTableColumnHeader column={column} title="Product" />
        ),
        cell: ({ row }) => (
            <Link to={`/products/${row.original.id}`} className="block hover:underline">
                <div>
                    <div className="font-medium flex items-center">
                        {row.original.brand && (
                            <span className="text-muted-foreground mr-2">
                                {row.original.brand}
                            </span>
                        )}
                        <span className="block max-w-[400px] whitespace-nowrap overflow-hidden text-ellipsis">
                            {row.original.name}
                        </span>
                    </div>
                </div>
            </Link>
        ),
    },
    {
        accessorKey: 'lastPrice',
        header: ({ column }) => (
            <DataTableColumnHeader column={column} title="Price" />
        ),
        cell: ({ row }) => {
            const product = row.original;
            return (
                <div className="font-medium">
                    <span className={product.isOnSale ? 'text-destructive' : ''}>
                        {formatPrice(product.lastPrice)}
                    </span>
                    {product.isOnSale && product.usualPrice && (
                        <span className="text-sm text-muted-foreground line-through ml-2">
                            {formatPrice(product.usualPrice)}
                        </span>
                    )}
                </div>
            );
        },
    },
    {
        accessorKey: 'createdAt',
        header: ({ column }) => (
            <DataTableColumnHeader column={column} title="Last Updated" />
        ),
        cell: ({ row }) => (
            <div className="text-sm">
                {formatDate(row.original.createdAt)}
            </div>
        ),
    },
    {
        id: 'actions',
        cell: ({ row }) => (
            <Button asChild size="sm" variant="outline">
                <a href={row.original.sourceUrl} target="_blank" rel="noopener noreferrer">
                    <ExternalLink className="h-3 w-3" />
                </a>
            </Button>
        ),
        enableSorting: false,
    },
];