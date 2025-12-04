using Hospital.Application.DTO.SupportTicket;
using Hospital.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportTicketController : ControllerBase
    {
        private readonly ISupportTicketService _ticketService;
        private readonly IPatientService _patientService;
        private readonly ILogger<SupportTicketController> _logger;

        public SupportTicketController(ISupportTicketService ticketService , IPatientService patientService, ILogger<SupportTicketController> logger)
        {
            _ticketService = ticketService;
            _patientService = patientService;
            _logger = logger;
        }

        // -----------------------------
        // Patient creates a ticket
        // -----------------------------
        [HttpPost("create")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Create([FromBody] CreateSupportTicketDto dto)
        {
            _logger.LogInformation("create Support Ticket called at {time}", DateTime.Now);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdTicket = await _ticketService.CreateAsync(dto);
                return Ok(createdTicket);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        // -----------------------------
        // Admin gets all tickets
        // -----------------------------
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("get all Support Ticket called at {time}", DateTime.Now);

            var tickets = await _ticketService.GetAllAsync();
            return Ok(tickets);
        }

        // -----------------------------
        // Get a single ticket by id
        // -----------------------------
        [HttpGet("{ticketId}")]
        [Authorize(Roles = "Admin, Patient")]
        public async Task<IActionResult> GetById(int ticketId)
        {
            _logger.LogInformation("get  Support by id Ticket called at {time}", DateTime.Now);

            var ticket = await _ticketService.GetByIdAsync(ticketId);
            if (ticket == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Patient can only view their own tickets
            //if (User.IsInRole("Patient") && ticket.PatientId != userId)
                //return Forbid();

            return Ok(ticket);
        }

        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<ActionResult<IEnumerable<SupportTicketDto>>> GetAllByPatientId(int patientId)
        {
            _logger.LogInformation("get all Support Ticket  by patient id " +
                "called at {time}", DateTime.Now);

            if (patientId==null)
                return BadRequest("PatientId is required.");
            var patient = await _patientService.GetPatientByIdAsync(patientId);
            if (patient == null)
                return NotFound("Patient not found.");

            var tickets = await _ticketService.GetAllByUserAsync(patient.UserId); // service maps patientId -> userId
            return Ok(tickets);
        }

        // -----------------------------
        // Admin updates ticket (response/status)
        // -----------------------------
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateSupportTicketDto dto)
        {
            _logger.LogInformation("update Support Ticket called at {time}", DateTime.Now);

            var updated = await _ticketService.UpdateAsync(dto);
            return Ok(updated);
        }

        // -----------------------------
        // Admin deletes a ticket
        // -----------------------------
        [HttpDelete("{ticketId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int ticketId)
        {
            _logger.LogInformation("delete Support Ticket called at {time}", DateTime.Now);

            var success = await _ticketService.DeleteAsync(ticketId);
            if (!success) return NotFound();

            return Ok("deleted Successfully");
        }
    }
}
