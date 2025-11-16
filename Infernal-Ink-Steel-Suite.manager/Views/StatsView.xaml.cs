using InfernalInkSteelSuite.ViewModels;
using InfernalInkSteelSuite.Repositories;
using System.Windows.Controls;
using System;

namespace InfernalInkSteelSuite.Views
{
    public partial class StatsView : UserControl
    {
        private StatsViewModel _viewModel;

        public StatsView()
        {
            InitializeComponent();
            string connectionString = @"Data Source=shop_manager.db";
            _viewModel = new StatsViewModel(new AppointmentRepository(connectionString), new ShopSettingsRepository(connectionString));
            DataContext = _viewModel;
            YearSelector.ItemsSource = _viewModel.Years;
            YearSelector.SelectedItem = _viewModel.SelectedYear;
            UpdateCharts();
        }

        private void YearSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YearSelector.SelectedItem != null)
            {
                _viewModel.LoadData((int)YearSelector.SelectedItem);
                UpdateCharts();
            }
        }

        private void UpdateCharts()
        {
            IncomeChart.Plot.Clear();
            IncomeChart.Plot.AddBar(_viewModel.IncomeData);
            IncomeChart.Plot.XAxis.Label("Month");
            IncomeChart.Plot.YAxis.Label("Income");
            IncomeChart.Plot.Title($"Monthly Income - {_viewModel.SelectedYear}");
            IncomeChart.Refresh();

            VisitsChart.Plot.Clear();
            VisitsChart.Plot.AddSignal(_viewModel.VisitsData);
            VisitsChart.Plot.XAxis.Label("Month");
            VisitsChart.Plot.YAxis.Label("Visits");
            VisitsChart.Plot.Title($"Monthly Visits - {_viewModel.SelectedYear}");
            VisitsChart.Refresh();

            HoursChart.Plot.Clear();
            HoursChart.Plot.AddSignal(_viewModel.HoursData);
            HoursChart.Plot.XAxis.Label("Month");
            HoursChart.Plot.YAxis.Label("Hours");
            HoursChart.Plot.Title($"Monthly Tattoo Hours - {_viewModel.SelectedYear}");
            HoursChart.Refresh();
        }
    }
}
