import { useState } from 'react';
import { useCreateProduct } from '@/lib/hooks/useProducts';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Plus, Loader2 } from 'lucide-react';

export function AddProductModal() {
    const [isOpen, setIsOpen] = useState(false);
    const [sourceUrl, setSourceUrl] = useState('');
    const [error, setError] = useState('');

    const createProduct = useCreateProduct();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');

        if (!sourceUrl.trim()) {
            setError('Please enter a product URL');
            return;
        }

        try {
            await createProduct.mutateAsync({ sourceUrl });
            setSourceUrl('');
            setIsOpen(false);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to add product');
        }
    };

    const handleOpenChange = (open: boolean) => {
        setIsOpen(open);
        if (!open) {
            setSourceUrl('');
            setError('');
        }
    };

    return (
        <Dialog open={isOpen} onOpenChange={handleOpenChange}>
            <DialogTrigger asChild>
                <Button>
                    <Plus className="h-4 w-4 mr-2" />
                    Add Product
                </Button>
            </DialogTrigger>
            <DialogContent className="sm:max-w-md">
                <DialogHeader>
                    <DialogTitle>Add Product</DialogTitle>
                    <DialogDescription>
                        Enter the URL of the product you want to add to your wishlist.
                    </DialogDescription>
                </DialogHeader>
                <form onSubmit={handleSubmit} className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="sourceUrl">Product URL</Label>
                        <Input
                            id="sourceUrl"
                            type="url"
                            placeholder="https://example.com/product"
                            value={sourceUrl}
                            onChange={(e) => setSourceUrl(e.target.value)}
                            disabled={createProduct.isPending}
                        />
                    </div>

                    {error && (
                        <div className="text-sm text-destructive">
                            {error}
                        </div>
                    )}

                    <div className="flex justify-end space-x-2">
                        <Button
                            type="button"
                            variant="outline"
                            onClick={() => setIsOpen(false)}
                            disabled={createProduct.isPending}
                        >
                            Cancel
                        </Button>
                        <Button
                            type="submit"
                            disabled={createProduct.isPending}
                        >
                            {createProduct.isPending && (
                                <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                            )}
                            Add Product
                        </Button>
                    </div>
                </form>
            </DialogContent>
        </Dialog>
    );
}