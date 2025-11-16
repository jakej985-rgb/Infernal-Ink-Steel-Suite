using InfernalInkSteelSuite.Repositories;
using System.Windows.Controls;
using InfernalInkSteelSuite.ViewModels;

namespace InfernalInkSteelSuite.Views
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
