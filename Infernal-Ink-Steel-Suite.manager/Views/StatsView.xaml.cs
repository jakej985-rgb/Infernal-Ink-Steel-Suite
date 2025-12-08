using InfernalInkSteelSuite.ViewModels;
using InfernalInkSteelSuite.Repositories;
using System.Windows.Controls;
using System;

namespace InfernalInkSteelSuite.Views
{
    public partial class StatsView : UserControl
    {
        public StatsView(InfernalInkSteelSuite.Domain.IDataProvider dataProvider)
        {
            InitializeComponent();
            DataContext = new StatsViewModel(dataProvider.Appointments, dataProvider.ShopSettings);
        }
    }
}
