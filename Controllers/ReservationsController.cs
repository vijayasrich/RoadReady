using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Models.DTO;
using RoadReady.Repositories;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin,customer")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ICarExtraRepository _carExtraRepository;
        private readonly ILogger<ReservationsController> _logger;
        private readonly IMapper _mapper;
        private readonly RoadReadyContext _context;

        public ReservationsController(IReservationRepository reservationRepository, ICarExtraRepository carExtraRepository,ILogger<ReservationsController> logger, IMapper mapper, RoadReadyContext context)
        {
            _reservationRepository = reservationRepository;
            _carExtraRepository = carExtraRepository;
            _logger = logger;
            _mapper = mapper;
            _context=context;
        }
        
        [HttpGet("all/{userId}")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> GetReservationsByUserId(int userId)
        {
            try
            {
                // Eager load the related CarExtra data
                var reservations = await _context.Reservations
                    .Where(r => r.UserId == userId)
                    .Include(r => r.Extras) // Include the related CarExtra entities
                    .ToListAsync();

                // Filter for completed and ongoing reservations based on DropoffDate
                var completedReservations = reservations
                    .Where(r => r.DropoffDate < DateTime.Now)
                    .Select(r => new
                    {
                        r.ReservationId,
                        r.UserId,
                        r.CarId,
                        r.PickupDate,
                        r.DropoffDate,
                        r.TotalPrice,
                        r.Status,
                        CarExtraIds = r.Extras.Select(e => e.ExtraId).ToList(),
                        Extras = r.Extras.Select(e => new
                        {
                            e.ExtraId,
                            e.Name,
                            e.Description,
                            e.Price
                        }).ToList()
                    })
                    .ToList();

                var ongoingReservations = reservations
                    .Where(r => r.DropoffDate >= DateTime.Now)
                    .Select(r => new
                    {
                        r.ReservationId,
                        r.UserId,
                        r.CarId,
                        r.PickupDate,
                        r.DropoffDate,
                        r.TotalPrice,
                        r.Status,
                        CarExtraIds = r.Extras.Select(e => e.ExtraId).ToList(),
                        Extras = r.Extras.Select(e => new
                        {
                            e.ExtraId,
                            e.Name,
                            e.Description,
                            e.Price
                        }).ToList()
                    })
                    .ToList();

                var response = new
                {
                    completedReservations,
                    ongoingReservations
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

       

        [HttpGet]
        [Authorize(Roles = "admin")] 
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetAllReservations()
        {
            var reservations = await _context.Reservations
            .Include(r => r.Extras) // Include the related CarExtra entities
            .ToListAsync();

            // Map to DTO or return directly
            var reservationDTOs = reservations.Select(r => new ReservationDTO
            {
                ReservationId = r.ReservationId,
                UserId = r.UserId,
                CarId = r.CarId,
                PickupDate = r.PickupDate,
                DropoffDate = r.DropoffDate,
                TotalPrice = r.TotalPrice,
                Status = r.Status,
                CarExtraIds = r.Extras.Select(e => e.ExtraId).ToList(), // Map the related ExtraIds
                Extras = r.Extras.Select(e => new CarExtraDTO
                {
                    ExtraId = e.ExtraId,
                    Name = e.Name,
                    Price = (decimal)e.Price
                }).ToList() // Map the related CarExtras to DTOs if needed
            }).ToList();

            return Ok(reservationDTOs);
        }



        [HttpGet("{id}")]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult<ReservationDTO>> GetReservationById(int id)
        {
            try
            {
                var reservation = await _reservationRepository.GetReservationByIdAsync(id);
                if (reservation == null)
                {
                    return NotFound(new { message = $"Reservation with ID {id} not found." });
                }

                // Map to DTO with CarExtras
                var reservationDTO = _mapper.Map<ReservationDTO>(reservation);
                return Ok(reservationDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the reservation with ID {id}.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpPost]  
        [Authorize(Roles = "customer")] // Only Customer can add reservations
        public async Task<IActionResult> AddReservation([FromBody] CreateReservationDTO reservationDTO)
        {
            if (reservationDTO == null)
            {
                return BadRequest(new { message = "Reservation data is required." });
            }

            try
            {
                // Check if CarExtraIds are provided and fetch the related CarExtras
                List<CarExtra> carExtras = new List<CarExtra>();

                if (reservationDTO.CarExtraIds != null && reservationDTO.CarExtraIds.Any())
                {
                    // Fetch the CarExtras based on the provided CarExtraIds
                    carExtras = await _carExtraRepository.GetCarExtrasByIdsAsync(reservationDTO.CarExtraIds);

                    // If there are CarExtras that don't exist in the database, handle this case
                    if (carExtras.Count != reservationDTO.CarExtraIds.Count)
                    {
                        var missingIds = reservationDTO.CarExtraIds.Except(carExtras.Select(ce => ce.ExtraId)).ToList();
                        return NotFound(new { message = "Some CarExtraIds were not found.", missingIds });
                    }
                }

                // Map the DTO to the Reservation model
                var reservation = _mapper.Map<Reservation>(reservationDTO);

                // Add the related CarExtras to the reservation
                reservation.Extras = carExtras;

                // Set the CreatedAt and UpdatedAt fields
                reservation.CreatedAt = DateTime.UtcNow;
                reservation.UpdatedAt = DateTime.UtcNow;

                // Add the reservation
                await _reservationRepository.AddReservationAsync(reservation);

                // Return the created reservation data
                var createdReservationDTO = _mapper.Map<ReservationDTO>(reservation);
                return CreatedAtAction(nameof(GetReservationById), new { id = reservation.ReservationId }, createdReservationDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating reservation");

                // Return a more detailed error response
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }




        [HttpPut("{id}/status")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateReservationStatus(int id, [FromBody] UpdateReservationStatusDTO statusDTO)
        {
            try
            {
                // Validate input data
                if (statusDTO == null || id != statusDTO.ReservationId)
                {
                    return BadRequest(new { message = "Invalid reservation data or ID mismatch." });
                }

                // Call the service/repository to update the reservation status
                var result = await _reservationRepository.UpdateReservationStatusAsync(id, statusDTO.Status);

                if (result)
                    return Ok(new { message = "Reservation status updated successfully." });

                return NotFound(new { message = "Reservation not found." });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                _logger.LogError($"Error updating reservation status: {ex.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
        
        [HttpDelete("cancel/{id}")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("userId")?.Value);

                var success = await _reservationRepository.CancelReservationAsync(id, userId);

                if (!success)
                {
                    return NotFound(new { message = "Reservation not found or you do not have permission to cancel this reservation." });
                }

                return Ok(new { message = "Reservation canceled successfully." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Invalid operation error while canceling reservation {id}: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while canceling reservation {id}: {ex.Message}, StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

    



    [HttpDelete("{id}")]
        [Authorize(Roles = "admin,customer")] 
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                var reservation = await _reservationRepository.GetReservationByIdAsync(id);
                if (reservation == null)
                {
                    return NotFound(new { message = $"Reservation with ID {id} not found." });
                }

                await _reservationRepository.DeleteReservationAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the reservation with ID {id}.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

    }
}







