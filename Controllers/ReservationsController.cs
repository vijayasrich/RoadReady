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
    [Authorize(Roles = "Admin,Customer,Agent")]
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
        /*// Get reservations by UserId
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservationsByUserId(int userId)
        {
            try
            {
                // Retrieve reservations for the given userId
                var reservations = await _reservationRepository.GetReservationsByUserIdAsync(userId);
                if (!reservations.Any())
                {
                    return NotFound(new { message = $"No reservations found for user with ID {userId}." });
                }

                // Map the reservations to DTOs
                var reservationDTOs = _mapper.Map<List<ReservationDTO>>(reservations);
                return Ok(reservationDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving reservations for user with ID {userId}.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }*/
        [HttpGet("all/{userId}")]
        [Authorize(Roles = "Customer")]
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

        /*[HttpGet("all/{userId}")]
        [Authorize(Roles ="Customer")]
        public async Task<IActionResult> GetReservationsByUserId(int userId)
        {
            try
            {
                var reservations = await _context.Reservations
                    .Where(r => r.UserId == userId)
                    .ToListAsync();

                // Filter for completed reservations based on DropOffDate
                var completedReservations = reservations.Where(r => r.DropoffDate < DateTime.Now).ToList();
                var ongoingReservations = reservations.Where(r => r.DropoffDate >= DateTime.Now).ToList();

                var response = new
                {
                    completedReservations = completedReservations,
                    ongoingReservations = ongoingReservations
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        */

        [HttpGet]
        [Authorize(Roles = "Admin,Agent")] // Only Admin and Agent can access all reservations
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

        /*// Get all reservations
        [HttpGet]
        [Authorize(Roles = "Admin,Agent")] // Only Admin and Agent can access all reservations
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetAllReservations()
        {
            try
            {
                var reservations = await _reservationRepository.GetAllReservationsAsync();
                if (!reservations.Any())
                {
                    return NotFound(new { message = "No reservations found." });
                }

                var reservationDTOs = _mapper.Map<List<ReservationDTO>>(reservations);
                return Ok(reservationDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving reservations.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }*/

        [HttpGet("{id}")]
        [Authorize(Roles ="Customer")]
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
        [Authorize(Roles = "Customer")] // Only Customer can add reservations
        public async Task<IActionResult> AddReservation([FromBody] CreateReservationDTO reservationDTO)
        {
            if (reservationDTO == null)
            {
                return BadRequest(new { message = "Reservation data is required." });
            }

            try
            {
                // Check if the CarExtraIds are provided and fetch the related CarExtras
                List<CarExtra> carExtras = new List<CarExtra>();

                // If CarExtraIds is not null and not empty, fetch CarExtras based on the provided CarExtraIds
                if (reservationDTO.CarExtraIds != null && reservationDTO.CarExtraIds.Any())
                {
                    // Fetch the CarExtras based on the provided CarExtraIds
                    carExtras = await _carExtraRepository.GetCarExtrasByIdsAsync(reservationDTO.CarExtraIds);

                    // If some CarExtras are missing, return an error
                    if (carExtras.Count != reservationDTO.CarExtraIds.Count)
                    {
                        return BadRequest(new { message = "Some CarExtras were not found." });
                    }
                }

                // Map CreateReservationDTO to Reservation entity
                var reservation = _mapper.Map<Reservation>(reservationDTO);

                // Associate the CarExtras with the reservation (empty or with extras)
                reservation.Extras = carExtras;

                // Save the reservation to the database
                await _reservationRepository.AddReservationAsync(reservation);

                // Map the saved reservation back to ReservationDTO to return the result
                var createdDTO = _mapper.Map<ReservationDTO>(reservation);

                // Return a 201 Created response with the location of the newly created resource
                return CreatedAtAction(nameof(GetReservationById), new { id = createdDTO.ReservationId }, createdDTO);
            }
            catch (Exception ex)
            {
                // Log the error and return a 500 internal server error
                _logger.LogError(ex, "An error occurred while adding a reservation.");
                return StatusCode(500, new { message = "An error occurred while saving the reservation." });
            }
        }





        // Update an existing reservation
        /*[HttpPut("{id}")]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] ReservationDTO reservationDTO)
        {
            if (reservationDTO == null || id != reservationDTO.ReservationId)
            {
                return BadRequest(new { message = "Invalid reservation data or ID mismatch." });
            }

            try
            {
                var existingReservation = await _reservationRepository.GetReservationByIdAsync(id);
                if (existingReservation == null)
                {
                    return NotFound(new { message = $"Reservation with ID {id} not found." });
                }

                var reservation = _mapper.Map(reservationDTO, existingReservation);
                await _reservationRepository.UpdateReservationAsync(reservation);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the reservation with ID {id}.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }*/

        // Delete a reservation
        /*[HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Agent")]
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
        }*/
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Restricting this action only to Admin role
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







