import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Dashboard = () => {
    const { user, logout } = useAuth();

    return (
        <div className="min-h-screen bg-app text-text-primary p-8">
            <div className="max-w-7xl mx-auto">
                <div className="flex justify-between items-center mb-8">
                    <h1 className="text-3xl font-bold text-primary">Dashboard</h1>
                    <div className="flex items-center gap-4">
                        <span className="text-text-muted">Welcome, {user?.username}</span>
                        <button
                            onClick={logout}
                            className="px-4 py-2 bg-surface-alt hover:bg-surface rounded text-sm text-text-primary transition-colors border border-surface-alt"
                        >
                            Logout
                        </button>
                    </div>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                    <Link to="/appointments" className="bg-surface p-6 rounded-lg border border-surface-alt hover:border-primary transition-colors cursor-pointer">
                        <h3 className="text-xl font-bold text-accent mb-2">Appointments</h3>
                        <p className="text-text-muted">Manage your schedule</p>
                    </Link>
                    <Link to="/clients" className="bg-surface p-6 rounded-lg border border-surface-alt hover:border-primary transition-colors cursor-pointer">
                        <h3 className="text-xl font-bold text-accent mb-2">Clients</h3>
                        <p className="text-text-muted">View client database</p>
                    </Link>
                    <Link to="/settings" className="bg-surface p-6 rounded-lg border border-surface-alt hover:border-primary transition-colors cursor-pointer">
                        <h3 className="text-xl font-bold text-accent mb-2">Settings</h3>
                        <p className="text-text-muted">Configure application</p>
                    </Link>
                </div>
            </div>
        </div>
    );
};

export default Dashboard;
