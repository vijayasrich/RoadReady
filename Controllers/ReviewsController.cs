using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Models.DTO;
using RoadReady.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly ILogger<ReviewsController> _logger;
        private readonly IMapper _mapper;
        private readonly RoadReadyContext _roadReadycontext;

        public ReviewsController(
            IReviewRepository reviewRepository,
            IReservationRepository reservationRepository, // Assuming you have a Reservation repository
            ILogger<ReviewsController> logger,
            IMapper mapper,
            RoadReadyContext roadReadycontext)
        {
            _reviewRepository = reviewRepository;
            _reservationRepository = reservationRepository;
            _logger = logger;
            _mapper = mapper;
            _roadReadycontext = roadReadycontext;
        }

        // GET: api/Reviews
        [HttpGet]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public async Task<ActionResult<IEnumerable<ReviewsDTO>>> GetAllReviews()
        {
            try
            {
                var reviews = await _reviewRepository.GetAllReviewsAsync();
                if (reviews == null || !reviews.Any())
                {
                    return NotFound("No reviews found.");
                }

                var reviewsDto = _mapper.Map<IEnumerable<ReviewsDTO>>(reviews);
                return Ok(reviewsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all reviews.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Reviews/ByCar/{carId}
        [HttpGet("ByCar/{carId}")]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public async Task<ActionResult<ReviewsDTO>> GetReviewByCarId(int carId)
        {
            try
            {
                var review = await _reviewRepository.GetReviewByCarIdAsync(carId);
                if (review == null)
                {
                    return NotFound($"Review for Car ID {carId} not found.");
                }

                var reviewDto = _mapper.Map<ReviewsDTO>(review);
                return Ok(reviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the review for Car ID {carId}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("addReview")]
        public async Task<IActionResult> AddReview([FromBody] ReviewRequestDTO reviewRequest)
        {
            try
            {
                // Check if the user has a completed reservation for the car
                var completedReservation = await _reservationRepository.GetCompletedReservationAsync(reviewRequest.UserId, reviewRequest.CarId);

                if (completedReservation == null)
                {
                    return BadRequest("You can only review cars you have completed reservations for.");
                }

   
                var review = new Review
                {
                    UserId = reviewRequest.UserId,
                    CarId = reviewRequest.CarId,
                    Rating = reviewRequest.Rating,
                    ReviewText = reviewRequest.ReviewText,
                    CreatedAt = DateTime.Now // Set the review creation time to now
                };

                // Add the review to the database
                _roadReadycontext.Reviews.Add(review);
                await _roadReadycontext.SaveChangesAsync();

                return Ok("Review added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the review.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        /*// POST: api/Reviews
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddReview([FromBody] ReviewsDTO reviewDto)
        {
            if (reviewDto == null)
            {
                return BadRequest("Review data is required.");
            }

            if (reviewDto.CarId <= 0)
            {
                return BadRequest("A valid CarId is required to add a review.");
            }

            try
            {
                var review = _mapper.Map<Review>(reviewDto);
                await _reviewRepository.AddReviewAsync(review);
                var createdReviewDto = _mapper.Map<ReviewsDTO>(review);

                return CreatedAtAction(nameof(GetReviewByCarId), new { carId = review.CarId }, createdReviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new review.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }*/

        // PUT: api/Reviews/ByCar/{carId}
        /*[HttpPut("ByCar/{carId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> UpdateReview(int carId, [FromBody] ReviewsDTO reviewDto)
        {
            if (reviewDto == null)
            {
                return BadRequest("Review data is required.");
            }

            if (carId != reviewDto.CarId)
            {
                return BadRequest("Car ID mismatch.");
            }

            try
            {
                var existingReview = await _reviewRepository.GetReviewByCarIdAsync(carId);

                if (existingReview == null)
                {
                    return NotFound($"Review for Car ID {carId} not found.");
                }

                var updatedReview = _mapper.Map(reviewDto, existingReview);
                await _reviewRepository.UpdateReviewAsync(updatedReview);

                return Ok(new { message = $"Review for Car ID {carId} has been updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the review for Car ID {carId}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }*/





        [HttpDelete("ByCar/{carId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReview(int carId)
        {
            try
            {
                // Get the review associated with the carId
                var review = await _reviewRepository.GetReviewByCarIdAsync(carId);
                if (review == null)
                {
                    return NotFound($"Review for Car ID {carId} not found.");
                }

                // Delete the review associated with the carId
                await _reviewRepository.DeleteReviewByCarIdAsync(carId);
                return Ok(new { message = $"Review for Car ID {carId} has been deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the review for Car ID {carId}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

    



