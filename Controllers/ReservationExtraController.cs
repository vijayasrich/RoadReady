/*using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadReady.Models;
using RoadReady.Repositories;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationExtraController : ControllerBase
    {
        private readonly IReservationExtraRepository _reservationExtraRepository;

        public ReservationExtraController(IReservationExtraRepository reservationExtraRepository)
        {
            _reservationExtraRepository = reservationExtraRepository;
        }

        
        [HttpGet]
        [Authorize(Roles = "Admin,Customer,Agent")]
        
        public IActionResult GetAllReservationExtras()
        {
            try
            {
                var reservationExtras = _reservationExtraRepository.GetAllReservationExtras();
                if (reservationExtras == null || !reservationExtras.Any())
                {
                    return NotFound("No reservation extras found.");
                }
                return Ok(reservationExtras);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving reservation extras.", details = ex.Message });
            }
        }

       
        [HttpPost]
        [Authorize(Roles = "Admin,Agent")]
        public IActionResult AddReservationExtra([FromBody] ReservationExtra reservationExtra)
        {
            if (reservationExtra == null)
            {
                return BadRequest("Reservation extra data is required.");
            }

            try
            {
                _reservationExtraRepository.AddReservationExtra(reservationExtra);
                return CreatedAtAction(nameof(GetAllReservationExtras), new { reservationId = reservationExtra.ReservationId, extraId = reservationExtra.ExtraId }, 
                    new { message = "Reservation extra added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while adding the reservation extra.", details = ex.Message });
            }
        }

        
        [HttpDelete]
        [Authorize(Roles = "Admin,Agent")]
        public IActionResult DeleteReservationExtra([FromQuery] int reservationId, [FromQuery] int extraId)
        {
            try
            {
                var reservationExtra = _reservationExtraRepository.GetAllReservationExtras()
                                        .FirstOrDefault(re => re.ReservationId == reservationId && re.ExtraId == extraId);

                if (reservationExtra == null)
                {
                    return NotFound($"No reservation extra found with Reservation ID {reservationId} and Extra ID {extraId}.");
                }

                _reservationExtraRepository.DeleteReservationExtra(reservationId, extraId);
                return Ok(new { message = $"Reservation Extra with Reservation ID {reservationId} and Extra ID {extraId} has been deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while deleting the reservation extra.", details = ex.Message });
            }
        }
    }
}
*/
