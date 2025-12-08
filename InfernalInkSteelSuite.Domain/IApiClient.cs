using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace InfernalInkSteelSuite.Domain
{
    public interface IApiClient
    {
        // Clients
        [Get("/api/clients")]
        Task<List<Client>> GetClientsAsync();

        [Get("/api/clients/{id}")]
        Task<Client> GetClientAsync(int id);

        [Post("/api/clients")]
        Task<Client> CreateClientAsync([Body] Client client);

        [Put("/api/clients/{id}")]
        Task UpdateClientAsync(int id, [Body] Client client);

        [Delete("/api/clients/{id}")]
        Task DeleteClientAsync(int id);


        // Appointments
        [Get("/api/appointments")]
        Task<List<Appointment>> GetAppointmentsAsync();

        [Post("/api/appointments")]
        Task<Appointment> CreateAppointmentAsync([Body] Appointment appointment);

        [Put("/api/appointments/{id}")]
        Task UpdateAppointmentAsync(int id, [Body] Appointment appointment);

        [Delete("/api/appointments/{id}")]
        Task DeleteAppointmentAsync(int id);

        // Add more endpoints as needed for other entities
    }
}
