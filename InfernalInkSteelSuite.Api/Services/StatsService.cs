using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Api.Services;

public class StatsService(
    IAppointmentRepository appointmentRepository,
    IClientRepository clientRepository,
    IShopSettingsRepository shopSettingsRepository,
    IQuoteRepository quoteRepository,
    IUserRepository userRepository)
{
    private readonly IAppointmentRepository _appointmentRepository = appointmentRepository;
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IShopSettingsRepository _shopSettingsRepository = shopSettingsRepository;
    private readonly IQuoteRepository _quoteRepository = quoteRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public DashboardStatsDto GetOverview()
    {
        var settings = _shopSettingsRepository.LoadSettings();

        // This is inefficient (fetching all), but fits the current "repo" pattern which lacks count methods.
        // Optimally we'd add Count() methods to repositories.
        var appointments = _appointmentRepository.GetAll();
        var clients = _clientRepository.GetAll();
        var quotes = _quoteRepository.GetAllQuotes();

        var now = DateTime.UtcNow;
        var today = now.Date;
        
        var appointmentsToday = appointments.Count(a => a.StartTime.Date == today);
        var upcomingAppointments = appointments.Count(a => a.StartTime > now);
        var openQuotes = quotes.Count; // Assuming all returned quotes are "open" for now

        var recentClients = clients
            .OrderByDescending(c => c.Id)
            .Take(5)
            .Select(c => new ClientSummaryDto(c.Id, $"{c.FirstName} {c.LastName}", c.Email))
            .ToList();

        // Check if shop is open (simplified placeholder)
        bool isOpen = true;

        var activeArtistsCount = _userRepository.GetActiveUsers().Count;

        return new DashboardStatsDto
        {
            AppointmentsToday = appointmentsToday,
            TotalClients = clients.Count,
            RecentClients = recentClients,
            IsShopOpen = isOpen,
            ActiveArtistsCount = activeArtistsCount,
            UpcomingAppointments = upcomingAppointments,
            OpenQuotes = openQuotes
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
    public int UpcomingAppointments { get; set; }
    public int OpenQuotes { get; set; }
}

public record ClientSummaryDto(int Id, string Name, string Email);
public record AppointmentStatDto(DateTime Date, int Count);
