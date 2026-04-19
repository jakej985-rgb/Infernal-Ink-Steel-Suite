using InfernalInkSteelSuite.ViewModels;
using InfernalInkSteelSuite.Repositories;
using System.Windows.Controls;
using System;
using InfernalInkSteelSuite.Data;

namespace InfernalInkSteelSuite.Views
{
    public partial class StatsView : UserControl
    {
        public StatsView(AppDbContext db)
        {
            InitializeComponent();
            DataContext = new StatsViewModel(new AppointmentRepository(db), new ShopSettingsRepository(db));
        }
    }
}
