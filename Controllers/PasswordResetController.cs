using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadReady.Models;
using RoadReady.Repositories;
using Microsoft.AspNetCore.Authorization;
using RoadReady.Exceptions;
using System;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class PasswordResetController : ControllerBase
{
    private readonly IPasswordResetRepository _passwordResetRepository;

    public PasswordResetController(IPasswordResetRepository passwordResetRepository)
    {
        _passwordResetRepository = passwordResetRepository;
    }

    
    [HttpGet("{token}")]
    public async Task<IActionResult> GetPasswordResetByToken(string token)
    {
        try
        {
            var resetRequest = await _passwordResetRepository.GetPasswordResetByTokenAsync(token);
            if (resetRequest == null)
                return NotFound(new { message = "Password reset request not found." });

            return Ok(resetRequest);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }

 
    [HttpPost]
    public async Task<IActionResult> AddPasswordResetRequest([FromBody] PasswordReset resetRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            resetRequest.Token = Guid.NewGuid().ToString(); 
            resetRequest.RequestTime = DateTime.UtcNow;     
            await _passwordResetRepository.AddPasswordResetAsync(resetRequest);

            return CreatedAtAction(nameof(GetPasswordResetByToken), new { token = resetRequest.Token }, resetRequest);
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

   
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePasswordReset(int id, [FromBody] PasswordReset resetRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            resetRequest.ResetId = id;
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

    
    [HttpDelete("{token}")]
    public async Task<IActionResult> DeletePasswordReset(string token)
    {
        try
        {
            var resetRequest = await _passwordResetRepository.GetPasswordResetByTokenAsync(token);
            if (resetRequest == null)
                return NotFound(new { message = "Password reset request not found." });

            await _passwordResetRepository.DeletePasswordResetAsync(token);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }
}
