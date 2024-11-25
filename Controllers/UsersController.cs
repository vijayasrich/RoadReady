using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadReady.Exceptions;
using RoadReady.Models;
using RoadReady.Repositories;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]  
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                if (users == null || !users.Any())
                {
                    return NotFound("No users found.");
                }
                if (User.IsInRole("Agent"))
                {
                    // Filter data for agents
                    var limitedUsers = users.Select(u => new
                    {
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.PhoneNumber
                    });
                    return Ok(limitedUsers);
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }
                if (User.IsInRole("Agent"))
                {
                    //return limited data for agents
                    return Ok(new
                    {
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.PhoneNumber
                    });

                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the user with ID {id}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _userRepository.AddUserAsync(user);
                return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new user.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.UserId) return BadRequest("User ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _userRepository.UpdateUserAsync(user);
                return Ok(new { message = $"User with ID {id} has been updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the user with ID {id}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                // Fetch the user by ID
                var user = await _userRepository.GetUserByIdAsync(id);

                // Handle the case where user doesn't exist
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} was not found.");
                    return NotFound(new { message = $"User with ID {id} was not found." });
                }

                // Proceed with deletion
                await _userRepository.DeleteUserAsync(id);

                _logger.LogInformation($"User with ID {id} successfully deleted.");
                return Ok(new { message = $"User with ID {id} has been deleted." }); ; // Return 204 No Content
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(new { message = ex.Message }); // Return 404 if user is not found
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, $"Database update error while deleting the user with ID {id}.");

                return StatusCode(500, new { message = "Database error occurred.", details = dbEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while deleting the user with ID {id}.");
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

    }
}
