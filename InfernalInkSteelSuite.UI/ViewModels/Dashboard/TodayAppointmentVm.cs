namespace InfernalInkSteelSuite.UI.ViewModels.Dashboard
{
    public class TodayAppointmentVm
    {
        public string TimeRange { get; set; }
        public string ClientName { get; set; }
        public string Service { get; set; }
        public string Artist { get; set; }
        public string Status { get; set; }

        public TodayAppointmentVm()
        {
            TimeRange = string.Empty;
            ClientName = string.Empty;
            Service = string.Empty;
            Artist = string.Empty;
            Status = string.Empty;
        }
    }
}
