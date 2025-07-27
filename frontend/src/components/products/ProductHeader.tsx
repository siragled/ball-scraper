import { Link } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { ArrowLeft, ExternalLink } from 'lucide-react';
import type { Product } from '@/lib/schemas/product';

interface ProductHeaderProps {
    product: Product;
}

export function ProductHeader({ product }: ProductHeaderProps) {
    return (
        <div className="flex items-start justify-between gap-4">
            <div>
                <Button asChild variant="ghost" size="sm" className="-ml-3 mb-2">
                    <Link to="/products">
                        <ArrowLeft className="h-4 w-4 mr-2" />
                        Back to Products
                    </Link>
                </Button>
                <h1 className="text-3xl font-bold">
                    {product.brand && (
                        <span className="font-medium text-muted-foreground mr-2">
                            {product.brand}
                        </span>
                    )}
                    <span>{product.name}</span>
                </h1>
            </div>
            <Button asChild>
                <a href={product.sourceUrl} target="_blank" rel="noopener noreferrer">
                    <ExternalLink className="h-4 w-4 mr-2" />
                    View on Store
                </a>
            </Button>
        </div>
    );
}