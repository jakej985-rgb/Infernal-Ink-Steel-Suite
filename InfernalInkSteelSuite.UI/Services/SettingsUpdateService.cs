using System;

namespace InfernalInkSteelSuite.UI.Services
{
    public static class SettingsUpdateService
    {
        public static event Action? OnSettingsChanged;

        public static void NotifySettingsChanged()
        {
            OnSettingsChanged?.Invoke();
        }
    }
}
