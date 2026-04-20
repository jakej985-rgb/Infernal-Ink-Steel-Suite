using InfernalInkSteelSuite.ViewModels.Settings;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Data;
using System.Collections.ObjectModel;
using InfernalInkSteelSuite.Domain;
using System;

namespace InfernalInkSteelSuite.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly User _user;
        private readonly IUserRepository _userRepository;
        private readonly IShopSettingsRepository _shopSettingsRepository;

        private ObservableCollection<SettingsTabViewModel> _tabs;
        public ObservableCollection<SettingsTabViewModel> Tabs
        {
            get { return _tabs; }
            set
            {
                _tabs = value;
                OnPropertyChanged();
            }
        }

        private SettingsTabViewModel _selectedTab;
        public SettingsTabViewModel SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;
                OnPropertyChanged();
            }
        }

        public SettingsViewModel(AppDbContext db, User user)
        {
            _user = user;
            _shopSettingsRepository = new ShopSettingsRepository(db);
            _userRepository = new UserRepository(db, new Repositories.Services.PasswordHasher());
            _tabs = [];
            _selectedTab = null!;

            Tabs.Add(new UserTabViewModel(_userRepository, _shopSettingsRepository, _user));
            Tabs.Add(new LinkedAccountsTabViewModel(_shopSettingsRepository));
            Tabs.Add(new NotificationSettingsTabViewModel(_shopSettingsRepository));
            Tabs.Add(new BackupDataTabViewModel(_shopSettingsRepository));
            Tabs.Add(new AccessibilityTabViewModel(_userRepository, _user));

            var role = _user.Role ?? string.Empty;
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                Tabs.Add(new UserManagementTabViewModel(_userRepository));
                Tabs.Add(new ShopHoursTabViewModel(_shopSettingsRepository));
                Tabs.Add(new PricingTabViewModel(_shopSettingsRepository));
                Tabs.Add(new ShopProfileTabViewModel(_shopSettingsRepository));
            }

            SelectedTab = Tabs[0];
        }
    }
}
