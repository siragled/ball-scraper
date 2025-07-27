import { Skeleton } from '@/components/ui/skeleton';

export function ProductDetailSkeleton() {
    return (
        <div className="p-6 space-y-6 animate-pulse">
            <div className="flex items-center justify-between">
                <Skeleton className="h-9 w-40" />
                <Skeleton className="h-9 w-32" />
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="md:col-span-1">
                    <Skeleton className="w-full aspect-square rounded-lg" />
                </div>

                <div className="md:col-span-2 space-y-6">
                    <div className="space-y-3">
                        <Skeleton className="h-6 w-24" />
                        <Skeleton className="h-8 w-3/4" />
                        <Skeleton className="h-8 w-1/3" />
                    </div>

                    <div className="space-y-3">
                        <Skeleton className="h-5 w-32" />
                        <Skeleton className="h-4 w-full" />
                        <Skeleton className="h-4 w-full" />
                        <Skeleton className="h-4 w-5/6" />
                    </div>

                    <div className="space-y-3 pt-2">
                        <Skeleton className="h-5 w-48" />
                        <Skeleton className="h-5 w-48" />
                    </div>
                </div>
            </div>
        </div>
    );
}