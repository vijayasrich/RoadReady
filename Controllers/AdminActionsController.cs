using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadReady.Exceptions;
using RoadReady.Models;
using RoadReady.Repositories;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminActionsController : ControllerBase
    {
        private readonly IAdminActionsRepository _adminActionsRepository;

        public AdminActionsController(IAdminActionsRepository adminActionsRepository)
        {
            _adminActionsRepository = adminActionsRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<ActionResult<IEnumerable<AdminActions>>> GetAllAdminActions()
        {
            try
            {
                var adminActions = await _adminActionsRepository.GetAllAdminActionsAsync();
                return Ok(adminActions);
            }
            catch (InternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<ActionResult<AdminActions>> GetAdminActionById(int id)
        {
            try
            {
                var adminAction = await _adminActionsRepository.GetAdminActionByIdAsync(id);
                if (adminAction == null)
                    throw new NotFoundException($"Admin action with ID {id} not found.");

                return Ok(adminAction);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAdminAction([FromBody] AdminActions adminAction)
        {
            try
            {
                await _adminActionsRepository.AddAdminActionAsync(adminAction);
                return CreatedAtAction(nameof(GetAdminActionById), new { id = adminAction.ActionId }, adminAction);
            }
            catch (DuplicateResourceException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdminAction(int id, [FromBody] AdminActions adminAction)
        {
            try
            {
                if (id != adminAction.ActionId)
                    throw new BadRequestException("ID in the URL does not match the ID in the body.");

                await _adminActionsRepository.UpdateAdminActionAsync(adminAction);
                return Ok(new { message = $"ID {id} has been updated." });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminAction(int id)
        {
            try
            {
                await _adminActionsRepository.DeleteAdminActionAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }

}

