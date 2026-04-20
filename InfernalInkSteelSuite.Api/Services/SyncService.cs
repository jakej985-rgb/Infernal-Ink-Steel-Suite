using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Domain.Sync;
using Microsoft.EntityFrameworkCore;

namespace InfernalInkSteelSuite.Api.Services
{
    public interface ISyncService
    {
        Task<List<Client>> GetClientsChangedSinceAsync(DateTime sinceUtc);
        Task<List<Appointment>> GetAppointmentsChangedSinceAsync(DateTime sinceUtc);
        Task<List<Document>> GetDocumentsChangedSinceAsync(DateTime sinceUtc);

        Task ProcessClientBatchAsync(SyncBatchRequestDto<Client> batch);
        Task ProcessAppointmentBatchAsync(SyncBatchRequestDto<Appointment> batch);
        Task ProcessDocumentBatchAsync(SyncBatchRequestDto<Document> batch);
    }

    public class SyncService(AppDbContext context, ILogger<SyncService> logger) : ISyncService
    {
        private readonly ILogger<SyncService> _logger = logger;


        public async Task<List<Client>> GetClientsChangedSinceAsync(DateTime sinceUtc)
        {
            return await context.Clients
                .Where(c => c.LastModifiedUtc > sinceUtc)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsChangedSinceAsync(DateTime sinceUtc)
        {
            return await context.Appointments
                .Where(a => a.LastModifiedUtc > sinceUtc)
                .ToListAsync();
        }

        public async Task<List<Document>> GetDocumentsChangedSinceAsync(DateTime sinceUtc)
        {
            return await context.Documents
                .Where(d => d.LastModifiedUtc > sinceUtc)
                .ToListAsync();
        }

        public async Task ProcessClientBatchAsync(SyncBatchRequestDto<Client> batch)
        {
            foreach (var change in batch.Changes)
            {
                var payload = change.Payload;
                var existing = await context.Clients
                    .FirstOrDefaultAsync(c => c.SyncId == change.EntityId);

                if (change.Operation == "Create")
                {
                    if (existing == null)
                    {
                        payload.SyncId = change.EntityId;
                        payload.Id = 0; // Let DB generate ID
                        context.Clients.Add(payload);
                    }
                    else
                    {
                        UpdateClient(existing, payload);
                    }
                }
                else if (change.Operation == "Update")
                {
                    if (existing != null)
                    {
                        UpdateClient(existing, payload);
                    }
                }
                else if (change.Operation == "Delete")
                {
                    if (existing != null)
                    {
                        existing.IsDeleted = true;
                        existing.LastModifiedUtc = DateTime.UtcNow;
                    }
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task ProcessAppointmentBatchAsync(SyncBatchRequestDto<Appointment> batch)
        {
            foreach (var change in batch.Changes)
            {
                var payload = change.Payload;
                var existing = await context.Appointments
                    .FirstOrDefaultAsync(a => a.SyncId == change.EntityId);

                // Resolve Foreign Key for Client if possible
                if (payload.ClientSyncId.HasValue && payload.ClientSyncId != Guid.Empty)
                {
                    var client = await context.Clients
                        .AsNoTracking() // Just need ID
                        .FirstOrDefaultAsync(c => c.SyncId == payload.ClientSyncId.Value);

                    if (client != null)
                    {
                        payload.ClientId = client.Id;
                    }
                    else
                    {
                        _logger.LogError("Sync error: Could not resolve client SyncId {SyncId} for appointment {ApptSyncId}. Skipping.", 
                            payload.ClientSyncId, change.EntityId);
                        continue; 
                    }
                }
                else if (change.Operation == "Create" || change.Operation == "Update")
                {
                    if (payload.ClientId == 0)
                    {
                         _logger.LogWarning("Sync warning: Appointment {ApptSyncId} has no client link. Skipping.", change.EntityId);
                         continue;
                    }
                }

                if (change.Operation == "Create")
                {
                    if (existing == null)
                    {
                        payload.SyncId = change.EntityId;
                        payload.Id = 0;
                        context.Appointments.Add(payload);
                    }
                    else
                    {
                        UpdateAppointment(existing, payload);
                    }
                }
                else if (change.Operation == "Update")
                {
                    if (existing != null)
                    {
                        UpdateAppointment(existing, payload);
                    }
                }
                else if (change.Operation == "Delete")
                {
                    if (existing != null)
                    {
                        existing.IsDeleted = true;
                        existing.LastModifiedUtc = DateTime.UtcNow;
                    }
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task ProcessDocumentBatchAsync(SyncBatchRequestDto<Document> batch)
        {
            foreach (var change in batch.Changes)
            {
                var payload = change.Payload;
                var existing = await context.Documents
                    .FirstOrDefaultAsync(d => d.SyncId == change.EntityId);

                // Resolve Foreign Key for Client if possible
                if (payload.ClientSyncId.HasValue && payload.ClientSyncId != Guid.Empty)
                {
                    var client = await context.Clients
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.SyncId == payload.ClientSyncId.Value);

                    if (client != null)
                    {
                        payload.ClientId = client.Id;
                    }
                    else
                    {
                        _logger.LogError("Sync error: Could not resolve client SyncId {SyncId} for document {DocSyncId}. Skipping.", 
                            payload.ClientSyncId, change.EntityId);
                        continue;
                    }
                }
                else if (change.Operation == "Create" || change.Operation == "Update")
                {
                    if (payload.ClientId == 0)
                    {
                         _logger.LogWarning("Sync warning: Document {DocSyncId} has no client link. Skipping.", change.EntityId);
                         continue;
                    }
                }

                if (change.Operation == "Create")
                {
                    if (existing == null)
                    {
                        payload.SyncId = change.EntityId;
                        payload.Id = 0;
                        context.Documents.Add(payload);
                    }
                    else
                    {
                        UpdateDocument(existing, payload);
                    }
                }
                else if (change.Operation == "Update")
                {
                    if (existing != null)
                    {
                        UpdateDocument(existing, payload);
                    }
                }
                else if (change.Operation == "Delete")
                {
                    if (existing != null)
                    {
                        existing.IsDeleted = true;
                        existing.LastModifiedUtc = DateTime.UtcNow;
                    }
                }
            }
            await context.SaveChangesAsync();
        }

        private static void UpdateClient(Client existing, Client payload)
        {
            existing.FirstName = payload.FirstName;
            existing.LastName = payload.LastName;
            existing.Phone = payload.Phone;
            existing.Email = payload.Email;

            existing.LastModifiedUtc = DateTime.UtcNow;
            existing.IsDeleted = payload.IsDeleted;
        }

        private static void UpdateAppointment(Appointment existing, Appointment payload)
        {
            existing.StartTime = payload.StartTime;
            existing.EndTime = payload.EndTime;
            existing.ServiceType = payload.ServiceType;
            existing.ServiceCategory = payload.ServiceCategory;
            existing.Status = payload.Status;
            existing.QuotedPrice = payload.QuotedPrice;
            existing.FinalPrice = payload.FinalPrice;
            existing.Notes = payload.Notes;
            // Update ClientId if we resolved it?
            if (payload.ClientId > 0) existing.ClientId = payload.ClientId;

            existing.LastModifiedUtc = DateTime.UtcNow;
            existing.IsDeleted = payload.IsDeleted;
        }

        private static void UpdateDocument(Document existing, Document payload)
        {
            existing.Title = payload.Title;
            existing.FilePath = payload.FilePath;

            if (payload.ClientId > 0) existing.ClientId = payload.ClientId;

            existing.LastModifiedUtc = DateTime.UtcNow;
            existing.IsDeleted = payload.IsDeleted;
        }
    }
}
