using Infernal_Ink_Steel_Suite.manager.ViewModels;
using InfernalInkSteelSuite.Repositories;
using System.Windows.Controls;

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
