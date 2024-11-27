using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadReady.Repositories;
using RoadReady.Models;
using AutoMapper;
using System;
using System.Threading.Tasks;
using RoadReady.Authentication;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;

namespace RoadReady.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase
    {
        private readonly IEmailRepository _emailRepository;
        private readonly RoadReadyContext _dbContext;
        private readonly IMapper _mapper;

        public PasswordResetController(IEmailRepository emailRepository, RoadReadyContext dbContext, IMapper mapper)
        {
            _emailRepository = emailRepository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // Endpoint to send a password reset email
        [HttpPost("send-reset-email")]
        public async Task<IActionResult> SendPasswordResetEmail(string email)
        {
            try
            {
                // Find user by email
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return NotFound("User with the provided email does not exist.");
                }

                // Generate reset token and expiration date
                var resetToken = Guid.NewGuid().ToString();
                var expirationDate = DateTime.UtcNow.AddHours(1);

                // Create and save PasswordReset entity
                var passwordReset = new PasswordReset
                {
                    UserId = user.UserId,
                    ResetToken = resetToken,
                    ExpirationDate = expirationDate,
                    IsUsed = false
                };

                _dbContext.PasswordResets.Add(passwordReset);
                await _dbContext.SaveChangesAsync();

                // Map to DTO and send email
                var resetDto = _mapper.Map<PasswordResetDTO>(passwordReset);
                var subject = "Password Reset Request";
                var body = $"Click the link below to reset your password:\n" +
                           $"https://roadready.com/reset-password?token={resetToken}\n" +
                           $"Token expires at {expirationDate}.";
                try
                {
                    await _emailRepository.SendEmailAsync(email, subject, body);
                    return Ok("Reset email sent successfully.");
                }
                catch (SmtpException smtpEx)
                {
                    // Log the detailed SMTP exception error
                    return StatusCode(500, $"SMTP error: {smtpEx.Message} - Inner Exception: {smtpEx.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    // Log any general exception
                    return StatusCode(500, $"Error sending email: {ex.Message} - Inner Exception: {ex.InnerException?.Message}");
                }

                // await _emailRepository.SendEmailAsync(user.Email, subject, body);

                // return Ok("Password reset email sent successfully.");
            }

            catch (SmtpException smtpEx)
            {
                // Log the SMTP exception for debugging
                return StatusCode(500, $"SMTP error: {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                // Log other general exceptions for debugging
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string resetToken, string newPassword)
        {
            // Validate the reset token
            var passwordReset = await _dbContext.PasswordResets
                .Include(pr => pr.User)
                .FirstOrDefaultAsync(pr => pr.ResetToken == resetToken && !pr.IsUsed && pr.ExpirationDate > DateTime.UtcNow);

            if (passwordReset == null)
            {
                return BadRequest("Invalid or expired reset token.");
            }

            // Hash the new password (make sure to use a secure password hashing method)
            var passwordHasher = new PasswordHasher<User>();
            passwordReset.User.Password = passwordHasher.HashPassword(passwordReset.User, newPassword);

            passwordReset.IsUsed = true;

            await _dbContext.SaveChangesAsync();

            return Ok("Password has been reset successfully.");
        }


        // Endpoint to get all password reset records (for admin or debugging purposes)
        [HttpGet("all-resets")]
        public async Task<IActionResult> GetAllPasswordResets()
        {
            var resets = await _dbContext.PasswordResets
                .Include(pr => pr.User)
                .ToListAsync();

            var resetDtos = _mapper.Map<List<PasswordResetDTO>>(resets);
            return Ok(resetDtos);
        }

        // Endpoint to get a single password reset record by token
        [HttpGet("reset/{resetToken}")]
        public async Task<IActionResult> GetPasswordResetByToken(string resetToken)
        {
            var passwordReset = await _dbContext.PasswordResets
                .Include(pr => pr.User)
                .FirstOrDefaultAsync(pr => pr.ResetToken == resetToken);

            if (passwordReset == null)
            {
                return NotFound("Password reset record not found.");
            }

            var resetDto = _mapper.Map<PasswordResetDTO>(passwordReset);
            return Ok(resetDto);
        }
    }
}



