using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadReady.Models;
using RoadReady.Repositories;
using RoadReady.Models.DTO;
using AutoMapper;
using RoadReady.Exceptions;
using System;
using System.Threading.Tasks;


[Route("api/[controller]")]
[ApiController]
public class PasswordResetController : ControllerBase
{
    private readonly IPasswordResetRepository _passwordResetRepository;
    private readonly IMapper _mapper;

    public PasswordResetController(IPasswordResetRepository passwordResetRepository, IMapper mapper)
    {
        _passwordResetRepository = passwordResetRepository;
        _mapper = mapper;
    }

    // GET: api/PasswordReset/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPasswordResetById(int id)
    {
        try
        {
            var resetRequest = await _passwordResetRepository.GetPasswordResetByIdAsync(id);
            if (resetRequest == null)
                return NotFound(new { message = "Password reset request not found." });

            var resetRequestDTO = _mapper.Map<PasswordResetDTO>(resetRequest);
            return Ok(resetRequestDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }

    // POST: api/PasswordReset
    [HttpPost]
    public async Task<IActionResult> AddPasswordResetRequest([FromBody] PasswordResetCreateDTO resetRequestDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var resetRequest = _mapper.Map<PasswordReset>(resetRequestDTO);
            resetRequest.Token = Guid.NewGuid().ToString(); // Generate token
            resetRequest.RequestTime = DateTime.UtcNow;

            await _passwordResetRepository.AddPasswordResetAsync(resetRequest);

            var responseDTO = _mapper.Map<PasswordResetResponseDTO>(resetRequest);
            return CreatedAtAction(nameof(GetPasswordResetById), new { id = resetRequest.ResetId }, responseDTO);
        }
        catch (DuplicateResourceException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }

    // PUT: api/PasswordReset/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePasswordReset(int id, [FromBody] PasswordResetUpdateDTO resetRequestDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var resetRequest = await _passwordResetRepository.GetPasswordResetByIdAsync(id);
            if (resetRequest == null)
                return NotFound(new { message = "Password reset request not found." });

            _mapper.Map(resetRequestDTO, resetRequest); // Map updated values from DTO to entity
            await _passwordResetRepository.UpdatePasswordResetAsync(resetRequest);

            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }

    // DELETE: api/PasswordReset/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePasswordReset(int id)
    {
        try
        {
            var resetRequest = await _passwordResetRepository.GetPasswordResetByIdAsync(id);
            if (resetRequest == null)
                return NotFound(new { message = "Password reset request not found." });

            await _passwordResetRepository.DeletePasswordResetAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }
}

