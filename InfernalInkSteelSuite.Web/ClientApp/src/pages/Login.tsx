import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Login = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const { login } = useAuth();
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        try {
            await login(username, password);
            navigate('/');
        } catch (err) {
            setError('Invalid username or password');
        }
    };

    return (
        <div className="flex min-h-screen items-center justify-center bg-app text-text-primary">
            <div className="w-full max-w-md rounded-lg bg-surface p-8 shadow-lg border border-surface-alt">
                <h2 className="mb-6 text-center text-3xl font-bold text-primary">Infernal Ink</h2>
                {error && <div className="mb-4 rounded bg-danger/20 p-3 text-danger border border-danger/50">{error}</div>}
                <form onSubmit={handleSubmit}>
                    <div className="mb-4">
                        <label className="mb-2 block text-sm font-bold text-text-secondary" htmlFor="username">
                            Username
                        </label>
                        <input
                            className="w-full rounded border border-surface-alt bg-surface-alt px-3 py-2 leading-tight text-text-primary focus:border-accent focus:outline-none"
                            id="username"
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                        />
                    </div>
                    <div className="mb-6">
                        <label className="mb-2 block text-sm font-bold text-text-secondary" htmlFor="password">
                            Password
                        </label>
                        <input
                            className="w-full rounded border border-surface-alt bg-surface-alt px-3 py-2 leading-tight text-text-primary focus:border-accent focus:outline-none"
                            id="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>
                    <div className="flex items-center justify-between">
                        <button
                            className="w-full rounded bg-primary px-4 py-2 font-bold text-text-primary hover:bg-primary-hover focus:outline-none focus:shadow-outline transition-colors"
                            type="submit"
                        >
                            Sign In
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default Login;
