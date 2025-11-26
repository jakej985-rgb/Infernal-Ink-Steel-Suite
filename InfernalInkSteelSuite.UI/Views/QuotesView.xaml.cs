using InfernalInkSteelSuite.Repositories;
using System.Windows.Controls;
using InfernalInkSteelSuite.UI.ViewModels;

namespace InfernalInkSteelSuite.UI.Views
{
    public partial class QuotesView : UserControl
    {
        public QuotesView(IAppointmentRepository appointmentRepository)
        {
            InitializeComponent();
            DataContext = new QuotesViewModel(appointmentRepository);
        }
    }
}
