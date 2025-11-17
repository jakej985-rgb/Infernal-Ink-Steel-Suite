using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Views.Appointments
{
    public interface IAppointmentTabView
    {
        Appointment? SelectedAppointment { get; }
    }
}
