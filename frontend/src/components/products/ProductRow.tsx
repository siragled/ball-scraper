import { Link } from 'react-router-dom';
import { TableCell, TableRow } from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import { ExternalLink } from 'lucide-react';
import type { Product } from '@/lib/schemas/product';

interface ProductRowProps {
    product: Product;
}

export function ProductRow({ product }: ProductRowProps) {
    const formatPrice = (price: number) => {
        return new Intl.NumberFormat('nl-NL', {
            style: 'currency',
            currency: 'EUR',
        }).format(price);
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString();
    };

    const truncateDescription = (description: string, maxLength = 100) => {
        if (description.length <= maxLength) return description;
        return description.substring(0, maxLength).trim() + '...';
    };

    return (
        <TableRow className="group">
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
                        <div className="font-medium">
                            {product.brand && (
                                <span className="text-muted-foreground mr-2">
                                    {product.brand}
                                </span>
                            )}
                            {product.name}
                        </div>
                        {product.description && (
                            <div className="text-sm text-muted-foreground line-clamp-2 mt-1">
                                {truncateDescription(product.description)}
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
    );
}