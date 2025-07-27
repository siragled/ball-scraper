import { formatPrice, formatDate } from '@/lib/utils';
import type { ProductSnapshot } from '@/lib/schemas/product';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Clock, CheckCircle, XCircle } from 'lucide-react';

interface ProductHistoryProps {
    snapshots: ProductSnapshot[] | undefined;
    isLoading: boolean;
}

export function ProductHistory({ snapshots, isLoading }: ProductHistoryProps) {
    if (!isLoading && (!snapshots || snapshots.length === 0)) {
        return null;
    }

    return (
        <div className="space-y-4">
            <h2 className="text-xl font-bold flex items-center gap-2">
                <Clock className="h-5 w-5" />
                Recent History
            </h2>
            <div className="border rounded-md">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>Date</TableHead>
                            <TableHead>Price</TableHead>
                            <TableHead className="text-center">Sale</TableHead>
                            <TableHead className="text-center">Stock</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {snapshots?.map((snapshot) => (
                            <TableRow key={snapshot.id}>
                                <TableCell className="font-medium">{formatDate(snapshot.createdAt)}</TableCell>
                                <TableCell>
                                    <div className="font-medium">
                                        <span>{formatPrice(snapshot.price)}</span>
                                        {snapshot.isOnSale && snapshot.usualPrice && (
                                            <span className="text-sm text-muted-foreground line-through ml-2">
                                                {formatPrice(snapshot.usualPrice)}
                                            </span>
                                        )}
                                    </div>
                                </TableCell>
                                <TableCell className="text-center">
                                    {snapshot.isOnSale ? <CheckCircle className="h-5 w-5 text-green-500 mx-auto" /> : <XCircle className="h-5 w-5 text-muted-foreground mx-auto" />}
                                </TableCell>
                                <TableCell className="text-center">
                                    {snapshot.isInStock ? <CheckCircle className="h-5 w-5 text-green-500 mx-auto" /> : <XCircle className="h-5 w-5 text-destructive mx-auto" />}
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </div>
        </div>
    );
}