﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadReady.Exceptions;
using RoadReady.Models;
using RoadReady.Models.DTO;
using RoadReady.Repositories;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger, IMapper mapper)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
        }

       
        [HttpGet]
        [Authorize(Roles = "Admin")]
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
                    var limitedUsersDto = _mapper.Map<IEnumerable<Models.DTO.UserDTO>>(users)
                        .Select(u => new
                        {
                            u.FirstName,
                            u.LastName,
                            u.Email,
                            u.PhoneNumber
                        });

                    return Ok(limitedUsersDto);
                }

                var usersDto = _mapper.Map<IEnumerable<Models.DTO.UserDTO>>(users);
                return Ok(usersDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        [HttpGet("{id}")]
        [Authorize(Roles = "Customer")]
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
                    var limitedUserDto = new
                    {
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.PhoneNumber
                    };

                    return Ok(limitedUserDto);
                }

                var userDto = _mapper.Map<Models.DTO.UserDTO>(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the user with ID {id}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

       
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateUser([FromBody] Models.DTO.UserDTO userDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = _mapper.Map<User>(userDto);
                await _userRepository.AddUserAsync(user);

                var createdUserDto = _mapper.Map<Models.DTO.UserDTO>(user);
                return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, createdUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new user.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

       
        [HttpPut("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] Models.DTO.UserDTO userDto)
        {
            if (id != userDto.UserId) return BadRequest("User ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existingUser = await _userRepository.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                var updatedUser = _mapper.Map(userDto, existingUser);
                await _userRepository.UpdateUserAsync(updatedUser);

                return Ok(new { message = $"User with ID {id} has been updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the user with ID {id}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}

