import React, { useState, useEffect } from 'react';
import api from '../services/api';

interface ShopSettings {
    shopName: string;
    taxRate: number;
    depositType: string;
    depositAmount: number;
    bookingBufferMinutes: number;
    cancellationPolicy: string;
    appFontSize: number;
}

const Settings = () => {
    const [settings, setSettings] = useState<ShopSettings>({
        shopName: '',
        taxRate: 0,
        depositType: 'Percentage',
        depositAmount: 0,
        bookingBufferMinutes: 0,
        cancellationPolicy: '',
        appFontSize: 14
    });
    const [message, setMessage] = useState('');

    useEffect(() => {
        fetchSettings();
    }, []);

    const fetchSettings = async () => {
        try {
            const response = await api.get('/settings');
            setSettings(response.data);
        } catch (error) {
            console.error('Failed to fetch settings', error);
        }
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setSettings(prev => ({
            ...prev,
            [name]: name === 'taxRate' || name === 'depositAmount' || name === 'bookingBufferMinutes' || name === 'appFontSize'
                ? parseFloat(value)
                : value
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await api.put('/settings', settings);
            setMessage('Settings saved successfully!');
            setTimeout(() => setMessage(''), 3000);
        } catch (error) {
            console.error('Failed to save settings', error);
            setMessage('Failed to save settings.');
        }
    };

    return (
        <div className="min-h-screen bg-app text-text-primary p-8">
            <div className="max-w-3xl mx-auto">
                <h1 className="text-3xl font-bold text-primary mb-8">Settings</h1>

                {message && (
                    <div className={`mb-4 p-4 rounded ${message.includes('Failed') ? 'bg-danger/20 text-danger' : 'bg-accent/20 text-accent'}`}>
                        {message}
                    </div>
                )}

                <form onSubmit={handleSubmit} className="bg-surface p-6 rounded-lg border border-surface-alt space-y-6">
                    <div>
                        <label className="block text-sm font-bold mb-2 text-text-secondary">Shop Name</label>
                        <input
                            type="text"
                            name="shopName"
                            value={settings.shopName}
                            onChange={handleChange}
                            className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                        />
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div>
                            <label className="block text-sm font-bold mb-2 text-text-secondary">Tax Rate (%)</label>
                            <input
                                type="number"
                                name="taxRate"
                                value={settings.taxRate}
                                onChange={handleChange}
                                step="0.01"
                                className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-bold mb-2 text-text-secondary">Booking Buffer (Minutes)</label>
                            <input
                                type="number"
                                name="bookingBufferMinutes"
                                value={settings.bookingBufferMinutes}
                                onChange={handleChange}
                                className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                            />
                        </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div>
                            <label className="block text-sm font-bold mb-2 text-text-secondary">Deposit Type</label>
                            <select
                                name="depositType"
                                value={settings.depositType}
                                onChange={handleChange}
                                className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                            >
                                <option value="Percentage">Percentage</option>
                                <option value="Fixed">Fixed Amount</option>
                            </select>
                        </div>
                        <div>
                            <label className="block text-sm font-bold mb-2 text-text-secondary">Deposit Amount</label>
                            <input
                                type="number"
                                name="depositAmount"
                                value={settings.depositAmount}
                                onChange={handleChange}
                                step="0.01"
                                className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                            />
                        </div>
                    </div>

                    <div>
                        <label className="block text-sm font-bold mb-2 text-text-secondary">Cancellation Policy</label>
                        <textarea
                            name="cancellationPolicy"
                            value={settings.cancellationPolicy}
                            onChange={handleChange}
                            rows={4}
                            className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                        />
                    </div>

                    <div className="flex justify-end">
                        <button
                            type="submit"
                            className="px-6 py-2 bg-primary hover:bg-primary-hover rounded text-text-primary font-bold transition-colors"
                        >
                            Save Settings
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default Settings;
