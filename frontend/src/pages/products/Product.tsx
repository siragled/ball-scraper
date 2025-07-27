import { useParams } from 'react-router-dom';
import { useProduct, useProductSnapshots } from '@/lib/hooks/useProducts';
import { ProductDetailSkeleton } from '@/components/products/ProductDetailSkeleton';
import { ProductHeader } from '@/components/products/ProductHeader';
import { ProductDetails } from '@/components/products/ProductDetails';
import { ProductHistory } from '@/components/products/ProductHistory';

export default function ProductPage() {
    const { id } = useParams<{ id: string }>();
    if (!id) return null;

    const { data: product, isLoading: isProductLoading, error } = useProduct(id);
    const { data: historyData, isLoading: isHistoryLoading } = useProductSnapshots(id, {
        skip: 0,
        take: 5,
    });

    if (isProductLoading) return <ProductDetailSkeleton />;

    if (error || !product) {
        return (
            <div className="p-6 text-center">
                <h2 className="text-xl font-semibold mb-2">Product not found</h2>
                <p className="text-muted-foreground">The product you're looking for doesn't exist.</p>
            </div>
        );
    }

    return (
        <div className="p-4 md:p-6 space-y-6">
            <ProductHeader product={product} />
            <ProductDetails product={product} />
            <ProductHistory
                snapshots={historyData?.items}
                isLoading={isHistoryLoading}
            />
        </div>
    );
}