import { useState, useEffect } from 'react';
import api from '../services/api';
import ClientModal from '../components/ClientModal';

interface Client {
    id: number;
    firstName: string;
    lastName: string;
    phone: string;
    email: string;
    notes: string;
}

const Clients = () => {
    const [clients, setClients] = useState<Client[]>([]);
    const [filteredClients, setFilteredClients] = useState<Client[]>([]);
    const [search, setSearch] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedClient, setSelectedClient] = useState<Client | null>(null);

    useEffect(() => {
        fetchClients();
        const interval = setInterval(fetchClients, 30000); // Poll every 30 seconds
        return () => clearInterval(interval);
    }, []);

    useEffect(() => {
        const lowerSearch = search.toLowerCase();
        setFilteredClients(
            clients.filter(c =>
                c.firstName.toLowerCase().includes(lowerSearch) ||
                c.lastName.toLowerCase().includes(lowerSearch) ||
                c.email.toLowerCase().includes(lowerSearch) ||
                c.phone.includes(search)
            )
        );
    }, [search, clients]);

    const fetchClients = async () => {
        try {
            const response = await api.get('/clients');
            setClients(response.data);
        } catch (error) {
            console.error('Failed to fetch clients', error);
        }
    };

    const handleEdit = (client: Client) => {
        setSelectedClient(client);
        setIsModalOpen(true);
    };

    const handleNewClient = () => {
        setSelectedClient(null);
        setIsModalOpen(true);
    };

    return (
        <div className="min-h-screen bg-app text-text-primary p-8">
            <div className="max-w-7xl mx-auto">
                <div className="flex justify-between items-center mb-8">
                    <h1 className="text-3xl font-bold text-primary">Clients</h1>
                    <button
                        onClick={handleNewClient}
                        className="px-4 py-2 bg-accent hover:bg-accent/80 rounded text-app font-bold transition-colors"
                    >
                        New Client
                    </button>
                </div>

                <div className="mb-6">
                    <input
                        type="text"
                        placeholder="Search clients..."
                        className="w-full max-w-md bg-surface border border-surface-alt rounded px-4 py-2 text-text-primary focus:border-accent focus:outline-none"
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                    />
                </div>

                <div className="bg-surface rounded-lg border border-surface-alt overflow-hidden">
                    <table className="w-full text-left">
                        <thead className="bg-surface-alt text-text-muted">
                            <tr>
                                <th className="p-4">Name</th>
                                <th className="p-4">Email</th>
                                <th className="p-4">Phone</th>
                                <th className="p-4">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredClients.map(client => (
                                <tr key={client.id} className="border-t border-surface-alt hover:bg-surface-alt/50 transition-colors">
                                    <td className="p-4 font-medium">{client.firstName} {client.lastName}</td>
                                    <td className="p-4 text-text-muted">{client.email}</td>
                                    <td className="p-4 text-text-muted">{client.phone}</td>
                                    <td className="p-4">
                                        <button
                                            onClick={() => handleEdit(client)}
                                            className="text-accent hover:text-accent/80 mr-4"
                                        >
                                            Edit
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
            <ClientModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onSave={fetchClients}
                client={selectedClient}
            />
        </div>
    );
};

export default Clients;
