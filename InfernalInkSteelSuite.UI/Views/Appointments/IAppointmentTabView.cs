using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.UI.Views.Appointments
{
    public interface IAppointmentTabView
    {
        Appointment? SelectedAppointment { get; }
    }
}
