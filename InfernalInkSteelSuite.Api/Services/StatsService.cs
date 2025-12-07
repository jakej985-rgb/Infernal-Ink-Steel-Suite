using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Api.Services;

public class StatsService(
    IAppointmentRepository appointmentRepository,
    IClientRepository clientRepository,
    IShopSettingsRepository shopSettingsRepository)
{
    private readonly IAppointmentRepository _appointmentRepository = appointmentRepository;
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IShopSettingsRepository _shopSettingsRepository = shopSettingsRepository;

    public DashboardStatsDto GetOverview()
    {
        var settings = _shopSettingsRepository.LoadSettings();

        // This is inefficient (fetching all), but fits the current "repo" pattern which lacks count methods.
        // Optimally we'd add Count() methods to repositories.
        var appointments = _appointmentRepository.GetAll();
        var clients = _clientRepository.GetAll();

        var today = DateTime.UtcNow.Date;
        var appointmentsToday = appointments.Count(a => a.StartTime.Date == today);

        var recentClients = clients
            .OrderByDescending(c => c.Id) // Assuming higher ID is newer, or we need CreatedAt which Client might not have
            .Take(5)
            .Select(c => new ClientSummaryDto(c.Id, $"{c.FirstName} {c.LastName}", c.Email))
            .ToList();

        // Check if shop is open
        bool isOpen = false;
        if (settings != null)
        {
            var now = DateTime.Now; // Local time for shop logic usually
            var dayOfWeek = now.DayOfWeek.ToString();
            // Simple check logic placeholder
            isOpen = true;
        }

        return new DashboardStatsDto
        {
            AppointmentsToday = appointmentsToday,
            TotalClients = clients.Count,
            RecentClients = recentClients,
            IsShopOpen = isOpen,
            ActiveArtistsCount = 1 // Placeholder until we have UserRepo connected
        };
    }

    public List<AppointmentStatDto> GetAppointmentsByDay(DateTime from, DateTime to)
    {
        var appointments = _appointmentRepository.GetAll();
        return
        [
            .. appointments
            .Where(a => a.StartTime >= from && a.StartTime <= to)
            .GroupBy(a => a.StartTime.Date)
            .Select(g => new AppointmentStatDto(g.Key, g.Count()))
            .OrderBy(x => x.Date)
        ];
    }
}

public class DashboardStatsDto
{
    public int AppointmentsToday { get; set; }
    public int TotalClients { get; set; }
    public List<ClientSummaryDto> RecentClients { get; set; } = [];
    public bool IsShopOpen { get; set; }
    public int ActiveArtistsCount { get; set; }
}

public record ClientSummaryDto(int Id, string Name, string Email);
public record AppointmentStatDto(DateTime Date, int Count);
