import { Skeleton } from '@/components/ui/skeleton';
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from '@/components/ui/table';

interface DataTableSkeletonProps {
    columnCount: number;
    rowCount?: number;
}

export function DataTableSkeleton({
    columnCount,
    rowCount = 10,
}: DataTableSkeletonProps) {
    return (
        <div className="space-y-4">
            <div className="border rounded-md">
                <Table>
                    <TableHeader>
                        <TableRow>
                            {Array.from({ length: columnCount }).map((_, i) => (
                                <TableHead key={i}>
                                    <Skeleton className="h-5 w-full" />
                                </TableHead>
                            ))}
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {Array.from({ length: rowCount }).map((_, i) => (
                            <TableRow key={i}>
                                {Array.from({ length: columnCount }).map(
                                    (_, j) => (
                                        <TableCell key={j}>
                                            <Skeleton className="h-5 w-full" />
                                        </TableCell>
                                    )
                                )}
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </div>
            <div className="flex items-center justify-between">
                <Skeleton className="h-8 w-24" />
                <div className="flex items-center space-x-2">
                    <Skeleton className="h-8 w-[70px]" />
                    <Skeleton className="h-8 w-[70px]" />
                </div>
            </div>
        </div>
    );
}