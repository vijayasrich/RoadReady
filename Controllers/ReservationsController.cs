using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadReady.Models;
using RoadReady.Repositories;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] 
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(IReservationRepository reservationRepository, ILogger<ReservationsController> logger)
        {
            _reservationRepository = reservationRepository;
            _logger = logger;
        }

       
        [HttpGet]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetAllReservations()
        {
            try
            {
                var reservations = await _reservationRepository.GetAllReservationsAsync();
                if (reservations == null || !reservations.Any())
                {
                    return NotFound("No reservations found.");
                }
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all reservations.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public async Task<ActionResult<Reservation>> GetReservationById(int id)
        {
            try
            {
                var reservation = await _reservationRepository.GetReservationByIdAsync(id);
                if (reservation == null)
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the reservation with ID {id}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        [HttpPost]
        [Authorize(Roles = "Customer,Agent")]
        public async Task<IActionResult> AddReservation([FromBody] Reservation reservation)
        {
            if (reservation == null)
            {
                return BadRequest("Reservation data is required.");
            }

            try
            {
                await _reservationRepository.AddReservationAsync(reservation);
                return CreatedAtAction(nameof(GetReservationById), new { id = reservation.ReservationId }, reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new reservation.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

      
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation reservation)
        {
            if (reservation == null)
            {
                return BadRequest("Reservation data is required.");
            }

            if (id != reservation.ReservationId)
            {
                return BadRequest("Reservation ID mismatch.");
            }

            try
            {
                var existingReservation = await _reservationRepository.GetReservationByIdAsync(id);
                if (existingReservation == null)
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }

                await _reservationRepository.UpdateReservationAsync(reservation);
                return Ok(new { message = $"ID {id} has been updated." });
            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the reservation with ID {id}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

       
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                var reservation = await _reservationRepository.GetReservationByIdAsync(id);
                if (reservation == null)
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }

                await _reservationRepository.DeleteReservationAsync(id);
                return Ok(new { message = $"ID {id} has been deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the reservation with ID {id}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}




