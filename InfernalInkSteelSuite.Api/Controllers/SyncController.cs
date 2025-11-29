using InfernalInkSteelSuite.Api.Models;
using InfernalInkSteelSuite.Api.Services;
using InfernalInkSteelSuite.Domain.Sync;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfernalInkSteelSuite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SyncController : ControllerBase
    {
        private readonly ISyncService _syncService;

        public SyncController(ISyncService syncService)
        {
            _syncService = syncService;
        }

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
             await _syncService.ProcessClientBatchAsync(batch);
             return Ok();
        }

        [HttpPost("appointments/batch-sync")]
        public async Task<ActionResult> BatchSyncAppointments([FromBody] SyncBatchRequestDto<Appointment> batch)
        {
             await _syncService.ProcessAppointmentBatchAsync(batch);
             return Ok();
        }

        [HttpPost("documents/batch-sync")]
        public async Task<ActionResult> BatchSyncDocuments([FromBody] SyncBatchRequestDto<Document> batch)
        {
             await _syncService.ProcessDocumentBatchAsync(batch);
             return Ok();
        }
    }
}
