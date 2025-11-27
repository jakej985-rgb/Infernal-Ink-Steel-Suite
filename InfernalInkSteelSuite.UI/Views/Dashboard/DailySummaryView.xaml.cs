using InfernalInkSteelSuite.ViewModels.Dashboard;
using System.Windows;

namespace InfernalInkSteelSuite.UI.Views.Dashboard
{
    public partial class DailySummaryView : Window
    {
        public DailySummaryView(DailySummaryViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.RequestClose += (s, e) => Close();
        }
    }
}
