import { Link, useParams } from 'react-router-dom';
import { useProduct } from '@/lib/hooks/useProducts';
import { formatPrice, formatDate } from '@/lib/utils';
import { ProductDetailSkeleton } from '@/components/products/ProductDetailSkeleton';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { ArrowLeft, ExternalLink, Calendar, Store } from 'lucide-react';

export default function ProductPage() {
    const { id } = useParams<{ id: string }>();
    const { data: product, isLoading, error } = useProduct(id!);

    if (isLoading) {
        return <ProductDetailSkeleton />;
    }

    if (error || !product) {
        return (
            <div className="p-6 space-y-6">
                <Button asChild variant="ghost" size="sm">
                    <Link to="/products">
                        <ArrowLeft className="h-4 w-4 mr-2" />
                        Back to Products
                    </Link>
                </Button>
                <div className="text-center py-12">
                    <h2 className="text-xl font-semibold mb-2">Product not found</h2>
                    <p className="text-muted-foreground">
                        The product you're looking for doesn't exist or has been removed.
                    </p>
                </div>
            </div>
        );
    }

    return (
        <div className="p-6 space-y-6">
            <div className="flex items-center justify-between">
                <Button asChild variant="ghost" size="sm">
                    <Link to="/products">
                        <ArrowLeft className="h-4 w-4 mr-2" />
                        Back to Products
                    </Link>
                </Button>
                <Button asChild size="sm">
                    <a
                        href={product.sourceUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                    >
                        <ExternalLink className="h-3 w-3 mr-2" />
                        View on Store
                    </a>
                </Button>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="md:col-span-1">
                    <div className="w-full aspect-square bg-muted rounded overflow-hidden max-w-sm">
                        {product.imageUrl ? (
                            <img
                                src={product.imageUrl}
                                alt={product.name}
                                className="w-full h-full object-contain"
                            />
                        ) : (
                            <div className="w-full h-full flex items-center justify-center text-muted-foreground text-sm">
                                No image available
                            </div>
                        )}
                    </div>
                </div>

                <div className="md:col-span-2 space-y-4">
                    <div>
                        {product.brand && (
                            <Badge variant="secondary" className="mb-2">{product.brand}</Badge>
                        )}
                        <h1 className="text-2xl font-bold mb-2">{product.name}</h1>
                        <div className="text-2xl font-bold text-primary mb-3">
                            {formatPrice(product.lastPrice)}
                        </div>
                    </div>

                    {product.description && (
                        <div>
                            <h3 className="font-semibold mb-2">Description</h3>
                            <p className="text-muted-foreground text-sm leading-relaxed">
                                {product.description}
                            </p>
                        </div>
                    )}

                    <div className="space-y-2 text-sm">
                        {product.storeName && (
                            <div className="flex items-center gap-2">
                                <Store className="h-4 w-4 text-muted-foreground" />
                                <span className="text-muted-foreground">Store:</span>
                                <span>{product.storeName}</span>
                            </div>
                        )}
                        <div className="flex items-center gap-2">
                            <Calendar className="h-4 w-4 text-muted-foreground" />
                            <span className="text-muted-foreground">Added:</span>
                            <span>{formatDate(product.createdAt)}</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}