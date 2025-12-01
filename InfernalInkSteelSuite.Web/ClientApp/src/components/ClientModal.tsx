import React, { useState, useEffect } from 'react';
import api from '../services/api';

interface ClientModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSave: () => void;
    client?: any;
}

const ClientModal: React.FC<ClientModalProps> = ({ isOpen, onClose, onSave, client }) => {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [email, setEmail] = useState('');
    const [phone, setPhone] = useState('');
    const [notes, setNotes] = useState('');

    useEffect(() => {
        if (isOpen) {
            if (client) {
                setFirstName(client.firstName);
                setLastName(client.lastName);
                setEmail(client.email);
                setPhone(client.phone);
                setNotes(client.notes || '');
            } else {
                resetForm();
            }
        }
    }, [isOpen, client]);

    const resetForm = () => {
        setFirstName('');
        setLastName('');
        setEmail('');
        setPhone('');
        setNotes('');
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const payload = {
            firstName,
            lastName,
            email,
            phone,
            notes
        };

        try {
            if (client) {
                await api.put(`/clients/${client.id}`, { ...payload, id: client.id });
            } else {
                await api.post('/clients', payload);
            }
            onSave();
            onClose();
        } catch (error) {
            console.error('Failed to save client', error);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
            <div className="bg-surface rounded-lg shadow-xl w-full max-w-md border border-surface-alt">
                <div className="flex justify-between items-center p-6 border-b border-surface-alt">
                    <h2 className="text-2xl font-bold text-primary">
                        {client ? 'Edit Client' : 'New Client'}
                    </h2>
                    <button onClick={onClose} className="text-text-muted hover:text-text-primary">
                        <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="p-6 space-y-4">
                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-bold mb-2 text-text-secondary">First Name</label>
                            <input
                                type="text"
                                value={firstName}
                                onChange={(e) => setFirstName(e.target.value)}
                                className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                                required
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-bold mb-2 text-text-secondary">Last Name</label>
                            <input
                                type="text"
                                value={lastName}
                                onChange={(e) => setLastName(e.target.value)}
                                className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                                required
                            />
                        </div>
                    </div>

                    <div>
                        <label className="block text-sm font-bold mb-2 text-text-secondary">Email</label>
                        <input
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                            required
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-bold mb-2 text-text-secondary">Phone</label>
                        <input
                            type="tel"
                            value={phone}
                            onChange={(e) => setPhone(e.target.value)}
                            className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                            required
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-bold mb-2 text-text-secondary">Notes</label>
                        <textarea
                            value={notes}
                            onChange={(e) => setNotes(e.target.value)}
                            className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                            rows={3}
                        />
                    </div>

                    <div className="flex justify-end gap-4 mt-6">
                        <button
                            type="button"
                            onClick={onClose}
                            className="px-4 py-2 bg-surface-alt hover:bg-surface rounded text-text-primary transition-colors border border-surface-alt"
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            className="px-4 py-2 bg-primary hover:bg-primary-hover rounded text-text-primary font-bold transition-colors"
                        >
                            Save Client
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default ClientModal;
