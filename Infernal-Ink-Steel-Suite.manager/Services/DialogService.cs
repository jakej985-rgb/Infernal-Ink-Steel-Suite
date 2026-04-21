using System.Windows;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Views;
using InfernalInkSteelSuite.Views.Dashboard;
using InfernalInkSteelSuite.ViewModels.Dashboard;
using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite.Services
{
    public interface IDialogService
    {
        bool? ShowAppointmentDialog(AppDbContext db, Appointment appointment);
        void ShowAddEditClientView(IClientRepository clientRepo, Client client);
        void ShowDailySummaryView(IAppointmentRepository apptRepo);
    }

    public class DialogService : IDialogService
    {
        public bool? ShowAppointmentDialog(AppDbContext db, Appointment appointment)
        {
            var dialog = new AppointmentDialog(db, appointment)
            {
                Owner = Application.Current.MainWindow
            };
            return dialog.ShowDialog();
        }

        public void ShowAddEditClientView(IClientRepository clientRepo, Client client)
        {
            var vm = new ViewModels.AddEditClientViewModel(clientRepo, client);
            var view = new AddEditClientView(vm)
            {
                Owner = Application.Current.MainWindow
            };
            view.ShowDialog();
        }

        public void ShowDailySummaryView(IAppointmentRepository apptRepo)
        {
            var vm = new DailySummaryViewModel(apptRepo);
            var view = new DailySummaryView(vm)
            {
                Owner = Application.Current.MainWindow
            };
            view.ShowDialog();
        }
    }
}
