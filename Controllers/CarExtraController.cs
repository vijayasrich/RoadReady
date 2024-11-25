using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadReady.Models;
using RoadReady.Repositories;
using Microsoft.AspNetCore.Authorization;
using RoadReady.Exceptions;

namespace RoadReady.Controllers
{
   

    [Route("api/[controller]")]
    [ApiController]
    public class CarExtraController : ControllerBase
    {
        private readonly ICarExtraRepository _carExtraRepository;

        public CarExtraController(ICarExtraRepository carExtraRepository)
        {
            _carExtraRepository = carExtraRepository;
        }

        
        [HttpGet]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public IActionResult GetAllCarExtras()
        {
            try
            {
                var carExtras = _carExtraRepository.GetAllCarExtras();
                return Ok(carExtras);
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
        [Authorize(Roles = "Admin,Customer,Agent")]
        public IActionResult GetCarExtraById(int id)
        {
            try
            {
                var carExtra = _carExtraRepository.GetCarExtraById(id);
                if (carExtra == null)
                    throw new NotFoundException("CarExtra not found.");

                return Ok(carExtra);
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
        [Authorize(Roles = "Admin,Agent")]
        public IActionResult AddCarExtra([FromBody] CarExtra carExtra)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _carExtraRepository.AddCarExtra(carExtra);
                return CreatedAtAction(nameof(GetCarExtraById), new { id = carExtra.ExtraId }, carExtra);
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
        [Authorize(Roles = "Admin,Agent")]
        public IActionResult UpdateCarExtra(int id, [FromBody] CarExtra carExtra)
        {
            if (id != carExtra.ExtraId)
                return BadRequest(new { message = "ID in URL does not match ID in body." });

            try
            {
                _carExtraRepository.UpdateCarExtra(carExtra);
                return Ok(new { message = $"ID {id} has been updated." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Agent")]
        public IActionResult DeleteCarExtra(int id)
        {
            try
            {
                _carExtraRepository.DeleteCarExtra(id);
                return Ok(new { message = $"ID {id} has been deleted." });
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

