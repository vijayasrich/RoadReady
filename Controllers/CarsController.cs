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
    public class CarsController : ControllerBase
    {
        private readonly ICarRepository _carRepository;

        public CarsController(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

       
        [HttpGet]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public async Task<ActionResult<IEnumerable<Car>>> GetAllCars()
        {
            try
            {
                var cars = await _carRepository.GetAllCarsAsync();
                return Ok(cars);
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
        public async Task<ActionResult<Car>> GetCarById(int id)
        {
            try
            {
                var car = await _carRepository.GetCarByIdAsync(id);
                if (car == null)
                    throw new NotFoundException("Car not found.");

                return Ok(car);
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
        public async Task<IActionResult> AddCar([FromBody] Car car)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _carRepository.AddCarAsync(car);
                return CreatedAtAction(nameof(GetCarById), new { id = car.CarId }, car);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] Car car)
        {
            if (id != car.CarId)
                return BadRequest(new { message = "ID in URL does not match ID in body." });

            try
            {
                await _carRepository.UpdateCarAsync(car);
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
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                await _carRepository.DeleteCarAsync(id);
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


