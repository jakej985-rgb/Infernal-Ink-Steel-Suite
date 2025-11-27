using InfernalInkSteelSuite.ViewModels;
using InfernalInkSteelSuite.Repositories;
using System.Windows.Controls;
using System;

namespace InfernalInkSteelSuite.UI.Views
{
    public partial class StatsView : UserControl
    {
        public StatsView()
        {
            InitializeComponent();
            string connectionString = @"Data Source=shop_manager.db";
            DataContext = new StatsViewModel(new AppointmentRepository(connectionString), new ShopSettingsRepository(connectionString));
        }
    }
}
