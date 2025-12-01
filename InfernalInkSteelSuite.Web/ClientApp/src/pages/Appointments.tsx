import { useState, useEffect } from 'react';
import { Calendar, dateFnsLocalizer, type View } from 'react-big-calendar';
import { format, parse, startOfWeek, getDay } from 'date-fns';
import { enUS } from 'date-fns/locale';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import api from '../services/api';
import AppointmentModal from '../components/AppointmentModal';

const locales = {
    'en-US': enUS,
};

const localizer = dateFnsLocalizer({
    format,
    parse,
    startOfWeek,
    getDay,
    locales,
});

interface Appointment {
    id: number;
    title: string;
    start: Date;
    end: Date;
    resource?: any;
}

const Appointments = () => {
    const [events, setEvents] = useState<Appointment[]>([]);
    const [view, setView] = useState<View>('month');
    const [date, setDate] = useState(new Date());
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedAppointment, setSelectedAppointment] = useState<any>(null);

    useEffect(() => {
        fetchAppointments();
        const interval = setInterval(fetchAppointments, 30000); // Poll every 30 seconds
        return () => clearInterval(interval);
    }, [date, view]);

    const fetchAppointments = async () => {
        try {
            const response = await api.get('/appointments');
            const mappedEvents = response.data.map((appt: any) => ({
                id: appt.id,
                title: `${appt.client.firstName} ${appt.client.lastName} - ${appt.serviceType}`,
                start: new Date(appt.startTime),
                end: new Date(appt.endTime),
                resource: appt
            }));
            setEvents(mappedEvents);
        } catch (error) {
            console.error('Failed to fetch appointments', error);
        }
    };

    const handleSelectEvent = (event: Appointment) => {
        setSelectedAppointment(event.resource);
        setIsModalOpen(true);
    };

    const handleNewAppointment = () => {
        setSelectedAppointment(null);
        setIsModalOpen(true);
    };

    return (
        <div className="min-h-screen bg-app text-text-primary p-8">
            <div className="max-w-7xl mx-auto">
                <div className="flex justify-between items-center mb-8">
                    <h1 className="text-3xl font-bold text-primary">Appointments</h1>
                    <button
                        onClick={handleNewAppointment}
                        className="px-4 py-2 bg-accent hover:bg-accent/80 rounded text-app font-bold transition-colors"
                    >
                        New Appointment
                    </button>
                </div>
                <div className="bg-surface p-6 rounded-lg border border-surface-alt h-[600px]">
                    <Calendar
                        localizer={localizer}
                        events={events}
                        startAccessor="start"
                        endAccessor="end"
                        style={{ height: '100%' }}
                        view={view}
                        onView={setView}
                        date={date}
                        onNavigate={setDate}
                        onSelectEvent={handleSelectEvent}
                        className="text-text-secondary"
                    />
                </div>
            </div>
            <AppointmentModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onSave={fetchAppointments}
                appointment={selectedAppointment}
            />
        </div>
    );
};

export default Appointments;
