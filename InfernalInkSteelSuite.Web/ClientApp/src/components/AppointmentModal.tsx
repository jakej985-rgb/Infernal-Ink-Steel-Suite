import React, { useState, useEffect } from 'react';
import DatePicker from 'react-datepicker';
import "react-datepicker/dist/react-datepicker.css";
import api from '../services/api';

interface AppointmentModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSave: () => void;
    appointment?: any;
}

const AppointmentModal: React.FC<AppointmentModalProps> = ({ isOpen, onClose, onSave, appointment }) => {
    const [clients, setClients] = useState<any[]>([]);
    const [clientId, setClientId] = useState('');
    const [serviceType, setServiceType] = useState('');
    const [startTime, setStartTime] = useState(new Date());
    const [endTime, setEndTime] = useState(new Date());
    const [notes, setNotes] = useState('');
    const [price, setPrice] = useState('');

    useEffect(() => {
        if (isOpen) {
            fetchClients();
            if (appointment) {
                setClientId(appointment.clientId);
                setServiceType(appointment.serviceType);
                setStartTime(new Date(appointment.startTime));
                setEndTime(new Date(appointment.endTime));
                setNotes(appointment.notes || '');
                setPrice(appointment.quotedPrice || '');
            } else {
                resetForm();
            }
        }
    }, [isOpen, appointment]);

    const fetchClients = async () => {
        try {
            const response = await api.get('/clients');
            setClients(response.data);
        } catch (error) {
            console.error('Failed to fetch clients', error);
        }
    };

    const resetForm = () => {
        setClientId('');
        setServiceType('');
        setStartTime(new Date());
        setEndTime(new Date());
        setNotes('');
        setPrice('');
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const payload = {
            clientId: parseInt(clientId),
            serviceType,
            startTime,
            endTime,
            notes,
            quotedPrice: parseFloat(price),
            status: 'Pending',
            artistId: 1 // TODO: Get from current user context
        };

        try {
            if (appointment) {
                await api.put(`/appointments/${appointment.id}`, { ...payload, id: appointment.id });
            } else {
                await api.post('/appointments', payload);
            }
            onSave();
            onClose();
        } catch (error) {
            console.error('Failed to save appointment', error);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
            <div className="bg-surface rounded-lg shadow-xl w-full max-w-2xl border border-surface-alt">
                <div className="flex justify-between items-center p-6 border-b border-surface-alt">
                    <h2 className="text-2xl font-bold text-primary">
                        {appointment ? 'Edit Appointment' : 'New Appointment'}
                    </h2>
                    <button onClick={onClose} className="text-text-muted hover:text-text-primary">
                        <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="p-6 space-y-4">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-bold mb-2 text-text-secondary">Client</label>
                            <select
                                value={clientId}
                                onChange={(e) => setClientId(e.target.value)}
                                className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                                required
                            >
                                <option value="">Select Client</option>
                                {clients.map(client => (
                                    <option key={client.id} value={client.id}>
                                        {client.firstName} {client.lastName}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <div>
                            <label className="block text-sm font-bold mb-2 text-text-secondary">Service Type</label>
                            <select
                                value={serviceType}
                                onChange={(e) => setServiceType(e.target.value)}
                                className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                            >
                                <option value="Tattoo">Tattoo</option>
                                <option value="Piercing">Piercing</option>
                                <option value="Consultation">Consultation</option>
                            </select>
                        </div>
                    </div>

                    <div className="flex gap-4">
                        <div className="flex-1">
                            <label className="block text-sm font-bold mb-2 text-text-secondary">Start Time</label>
                            <DatePicker
                                selected={startTime}
                                onChange={(date: Date | null) => date && setStartTime(date)}
                                showTimeSelect
                                dateFormat="Pp"
                                className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                            />
                        </div>
                        <div className="flex-1">
                            <label className="block text-sm font-bold mb-2 text-text-secondary">End Time</label>
                            <DatePicker
                                selected={endTime}
                                onChange={(date: Date | null) => date && setEndTime(date)}
                                showTimeSelect
                                dateFormat="Pp"
                                className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                            />
                        </div>
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

                    <div>
                        <label className="block text-sm font-bold mb-2 text-text-secondary">Price</label>
                        <input
                            type="number"
                            value={price}
                            onChange={(e) => setPrice(e.target.value)}
                            className="w-full bg-surface-alt border border-surface-alt rounded p-2 text-text-primary focus:border-accent focus:outline-none"
                            step="0.01"
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
                            Save Appointment
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default AppointmentModal;
