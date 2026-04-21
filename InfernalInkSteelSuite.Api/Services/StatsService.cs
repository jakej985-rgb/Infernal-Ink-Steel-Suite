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
        // H6 fix: Use efficient count queries instead of loading entire tables
        var appointmentsToday = _appointmentRepository.CountByDate(DateTime.UtcNow);
        var upcomingAppointments = _appointmentRepository.CountUpcoming();
        var totalClients = _clientRepository.Count();
        var openQuotes = _quoteRepository.GetAllQuotes().Count; // TODO: Add CountAll to IQuoteRepository

        var recentClients = _clientRepository.GetRecent(5)
            .Select(c => new ClientSummaryDto(c.Id, $"{c.FirstName} {c.LastName}", c.Email))
            .ToList();

        // Check if shop is open (simplified placeholder)
        bool isOpen = true;

        var activeArtistsCount = _userRepository.GetActiveUsers().Count;

        return new DashboardStatsDto
        {
            AppointmentsToday = appointmentsToday,
            TotalClients = totalClients,
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
