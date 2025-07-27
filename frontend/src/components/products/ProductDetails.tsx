import { formatDate, formatPrice } from '@/lib/utils';
import type { Product } from '@/lib/schemas/product';
import { Calendar, Store, ShoppingCart } from 'lucide-react';

interface ProductDetailsProps {
    product: Product;
}

export function ProductDetails({ product }: ProductDetailsProps) {
    return (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
            <div className="lg:col-span-1">
                <div className="w-full aspect-square bg-muted rounded flex items-center justify-center p-4">
                    {product.imageUrl ? (
                        <img src={product.imageUrl} alt={product.name} className="max-w-full max-h-full object-contain" />
                    ) : (
                        <span className="text-muted-foreground text-sm">No image available</span>
                    )}
                </div>
            </div>

            <div className="lg:col-span-2 space-y-4">
                <div className="flex items-baseline gap-3">
                    <span className={`text-4xl font-bold ${product.isOnSale ? 'text-destructive' : 'text-primary'}`}>
                        {formatPrice(product.lastPrice)}
                    </span>
                    {product.isOnSale && product.usualPrice && (
                        <span className="text-2xl text-muted-foreground line-through">
                            {formatPrice(product.usualPrice)}
                        </span>
                    )}
                </div>

                <p className="text-sm text-muted-foreground leading-relaxed line-clamp-4">
                    {product.description || 'No description available.'}
                </p>

                <div className="grid grid-cols-2 gap-x-4 gap-y-3 pt-2 text-sm">
                    <div className="flex items-center gap-2">
                        <ShoppingCart className="h-4 w-4 text-muted-foreground" />
                        <span className="font-medium">Status:</span>
                        <span className={product.isInStock ? '' : 'text-destructive'}>
                            {product.isInStock ? 'In Stock' : 'Out of Stock'}
                        </span>
                    </div>
                    <div className="flex items-center gap-2">
                        <Store className="h-4 w-4 text-muted-foreground" />
                        <span className="font-medium">Store:</span>
                        <span>{product.storeName}</span>
                    </div>
                    <div className="flex items-center gap-2">
                        <Calendar className="h-4 w-4 text-muted-foreground" />
                        <span className="font-medium">Added:</span>
                        <span>{formatDate(product.createdAt)}</span>
                    </div>
                </div>
            </div>
        </div>
    );
}