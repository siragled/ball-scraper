import { useState } from 'react';
import { Eye, EyeOff, Mail, Lock, User } from 'lucide-react';
import { useAuth } from '@/lib/providers/AuthProvider';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { useNavigate } from 'react-router-dom';

export function LoginPage() {
    const { login, register } = useAuth();
    const navigate = useNavigate();

    const [isRegister, setIsRegister] = useState(false);
    const [showPwd, setShowPwd] = useState(false);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        const fd = new FormData(e.currentTarget);
        const payload = {
            email: fd.get('email') as string,
            username: fd.get('username') as string,
            password: fd.get('password') as string,
            confirmPassword: fd.get('confirmPassword') as string,
        };

        try {
            isRegister ? await register(payload)
                : await login({ username: payload.username, password: payload.password });
            navigate('/');
        } catch (err: any) {
            setError(err.message || 'Something went wrong');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="flex min-h-screen items-center justify-center bg-gradient-to-br from-slate-50 to-slate-100 dark:from-slate-900 dark:to-slate-800">
            <Card className="w-full max-w-sm shadow-xl">
                <CardHeader className="space-y-1">
                    <CardTitle className="text-2xl font-bold text-center">Ball Scraper</CardTitle>
                    <CardDescription className="text-center">
                        {isRegister ? 'Create your account' : 'Welcome back, please sign in'}
                    </CardDescription>
                </CardHeader>

                <CardContent>
                    <form onSubmit={handleSubmit} className="space-y-4">
                        {isRegister && (
                            <div className="space-y-2">
                                <Label htmlFor="email">Email</Label>
                                <div className="relative">
                                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                                    <Input id="email" name="email" type="email" placeholder="you@example.com" required className="pl-10" />
                                </div>
                            </div>
                        )}

                        <div className="space-y-2">
                            <Label htmlFor="username">Username</Label>
                            <div className="relative">
                                <User className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                                <Input id="username" name="username" placeholder="baller" required className="pl-10" />
                            </div>
                        </div>

                        <div className="space-y-2">
                            <Label htmlFor="password">Password</Label>
                            <div className="relative">
                                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                                <Input
                                    id="password"
                                    name="password"
                                    type={showPwd ? 'text' : 'password'}
                                    placeholder="••••••••"
                                    required
                                    className="pl-10"
                                />
                                <button
                                    type="button"
                                    onClick={() => setShowPwd(!showPwd)}
                                    className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
                                >
                                    {showPwd ? <EyeOff size={16} /> : <Eye size={16} />}
                                </button>
                            </div>
                            {isRegister && <p className="text-xs text-muted-foreground">At least 8 characters, one uppercase and one number</p>}
                        </div>

                        {isRegister && (
                            <div className="space-y-2">
                                <Label htmlFor="confirmPassword">Confirm Password</Label>
                                <div className="relative">
                                    <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                                    <Input
                                        id="confirmPassword"
                                        name="confirmPassword"
                                        type="password"
                                        placeholder="••••••••"
                                        required
                                        className="pl-10"
                                    />
                                </div>
                            </div>
                        )}

                        {error && <p className="text-sm text-destructive text-center">{error}</p>}

                        <Button type="submit" className="w-full" disabled={loading}>
                            {loading ? 'Please wait…' : isRegister ? 'Create account' : 'Sign in'}
                        </Button>
                    </form>

                    <div className="mt-4 text-center text-sm">
                        {isRegister ? 'Already have an account?' : "Don't have an account?"}{' '}
                        <button
                            type="button"
                            onClick={() => {
                                setIsRegister(!isRegister);
                                setError(null);
                            }}
                            className="font-semibold underline underline-offset-4 hover:text-primary"
                        >
                            {isRegister ? 'Sign in' : 'Sign up'}
                        </button>
                    </div>
                </CardContent>
            </Card>
        </div>
    );
}