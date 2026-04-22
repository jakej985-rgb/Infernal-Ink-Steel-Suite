using InfernalInkSteelSuite.Api.Models;
using InfernalInkSteelSuite.Api.Services;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Domain.Sync;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfernalInkSteelSuite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SyncController(ISyncService syncService) : ControllerBase
    {
        private readonly ISyncService _syncService = syncService;

        [HttpGet("ping")]
        public ActionResult Ping()
        {
            return Ok(new { status = "ok", serverTimeUtc = DateTime.UtcNow });
        }

        [HttpGet("clients/changed-since")]
        public async Task<ActionResult<List<Client>>> GetClientsChangedSince([FromQuery] DateTime sinceUtc)
        {
            var result = await _syncService.GetClientsChangedSinceAsync(sinceUtc);
            return Ok(result);
        }

        [HttpGet("appointments/changed-since")]
        public async Task<ActionResult<List<Appointment>>> GetAppointmentsChangedSince([FromQuery] DateTime sinceUtc)
        {
            var result = await _syncService.GetAppointmentsChangedSinceAsync(sinceUtc);
            return Ok(result);
        }

        [HttpGet("documents/changed-since")]
        public async Task<ActionResult<List<Document>>> GetDocumentsChangedSince([FromQuery] DateTime sinceUtc)
        {
            var result = await _syncService.GetDocumentsChangedSinceAsync(sinceUtc);
            return Ok(result);
        }

        [HttpPost("clients/batch-sync")]
        public async Task<ActionResult> BatchSyncClients([FromBody] SyncBatchRequestDto<Client> batch)
        {
            // (M6) Validation: Ensure EntityId matches Payload.SyncId
            foreach (var entry in batch.Changes)
            {
                if (entry.EntityId == Guid.Empty || entry.EntityId != entry.Payload.SyncId)
                    return BadRequest($"Inconsistent client batch entry: {entry.EntityId} vs {entry.Payload.SyncId}");
            }
            await _syncService.ProcessClientBatchAsync(batch);
            return Ok();
        }

        [HttpPost("appointments/batch-sync")]
        public async Task<ActionResult> BatchSyncAppointments([FromBody] SyncBatchRequestDto<Appointment> batch)
        {
            foreach (var entry in batch.Changes)
            {
                if (entry.EntityId == Guid.Empty || entry.EntityId != entry.Payload.SyncId)
                    return BadRequest($"Inconsistent appointment batch entry: {entry.EntityId} vs {entry.Payload.SyncId}");
            }
            await _syncService.ProcessAppointmentBatchAsync(batch);
            return Ok();
        }

        [HttpPost("documents/batch-sync")]
        public async Task<ActionResult> BatchSyncDocuments([FromBody] SyncBatchRequestDto<Document> batch)
        {
            foreach (var entry in batch.Changes)
            {
                if (entry.EntityId == Guid.Empty || entry.EntityId != entry.Payload.SyncId)
                    return BadRequest($"Inconsistent document batch entry: {entry.EntityId} vs {entry.Payload.SyncId}");
            }
            await _syncService.ProcessDocumentBatchAsync(batch);
            return Ok();
        }
    }
}
