using InfernalInkSteelSuite.ViewModels;
using InfernalInkSteelSuite.Repositories;
using System.Windows.Controls;
using System;

namespace InfernalInkSteelSuite.Views
{
    public partial class StatsView : UserControl
    {
        public StatsView(string connectionString)
        {
            InitializeComponent();
            DataContext = new StatsViewModel(new AppointmentRepository(connectionString), new ShopSettingsRepository(connectionString));
        }
    }
}
