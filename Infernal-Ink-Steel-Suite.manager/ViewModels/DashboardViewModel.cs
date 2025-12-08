using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Services.Sync;
using System;
using System.Windows;
using System.Windows.Threading;

namespace InfernalInkSteelSuite.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private string _statusMessage = "Ready";
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private bool _isOnline = true;
        public bool IsOnline
        {
            get => _isOnline;
            set { _isOnline = value; OnPropertyChanged(); }
        }

        private bool _isSyncing;
        public bool IsSyncing
        {
            get => _isSyncing;
            set { _isSyncing = value; OnPropertyChanged(); }
        }

        private string _dataModeDisplay = "Local Only";
        public string DataModeDisplay
        {
            get => _dataModeDisplay;
            set { _dataModeDisplay = value; OnPropertyChanged(); }
        }

        private readonly ISyncEngine? _syncEngine;

        public DashboardViewModel(ConnectionSettings settings)
        {
            DataModeDisplay = settings.Mode == DataMode.LocalOnly ? "Local Only" : "Server Sync";

            if (Application.Current.Properties["SyncEngine"] is ISyncEngine engine)
            {
                _syncEngine = engine;
                _syncEngine.SyncStatusChanged += OnSyncStatusChanged;
                IsOnline = true; // Assume online initially if engine exists
                StatusMessage = "Sync Enabled";
            }
            else
            {
                IsOnline = true; // Local mode is "online" locally
                StatusMessage = "Local Mode";
            }
        }

        private void OnSyncStatusChanged(object? sender, SyncStatusEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsSyncing = e.IsSyncing;
                IsOnline = e.IsOnline;
                StatusMessage = e.StatusMessage;
            });
        }
    }
}
