/*using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadReady.Models;
using RoadReady.Repositories;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]  
    public class UserAuditController : ControllerBase
    {
        private readonly IUserAuditRepository _userAuditRepository;
        private readonly ILogger<UserAuditController> _logger;

        public UserAuditController(IUserAuditRepository userAuditRepository, ILogger<UserAuditController> logger)
        {
            _userAuditRepository = userAuditRepository;
            _logger = logger;
        }

        
        [HttpGet]
        [Authorize(Roles = "Admin")]  
        public async Task<ActionResult<IEnumerable<UserAudit>>> GetAllUserAudits()
        {
            try
            {
                var audits = await _userAuditRepository.GetAllAuditsAsync();
                if (audits == null || !audits.Any())
                {
                    return NotFound("No user audit records found.");
                }
                return Ok(audits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all user audit records.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserAudit>> AddUserAudit([FromBody] UserAudit audit)
        {
            if (audit == null)
            {
                return BadRequest("Audit data is required.");
            }

            try
            {
                // Call the AddAuditAsync method from the repository to add the new audit record
                await _userAuditRepository.AddAuditAsync(audit);

                // Return a 201 Created status with the created audit object
                return CreatedAtAction(nameof(GetUserAuditById), new { id = audit.AuditId }, audit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the user audit.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]  
        public async Task<ActionResult<UserAudit>> GetUserAuditById(int id)
        {
            try
            {
                var userAudit = await _userAuditRepository.GetAuditByIdAsync(id);
                if (userAudit == null)
                {
                    return NotFound($"User audit with ID {id} not found.");
                }
                return Ok(userAudit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the user audit with ID {id}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}*/


