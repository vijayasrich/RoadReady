using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadReady.Models;
using RoadReady.Repositories;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] 
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewRepository reviewRepository, ILogger<ReviewsController> logger)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
        }

       
        [HttpGet]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllReviews()
        {
            try
            {
                var reviews = await _reviewRepository.GetAllReviewsAsync();
                if (reviews == null || !reviews.Any())
                {
                    return NotFound("No reviews found.");
                }
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all reviews.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("ByCar/{carId}")]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public async Task<ActionResult<Review>> GetReviewByCarId(int carId)
        {
            try
            {
                var review = await _reviewRepository.GetReviewByCarIdAsync(carId); // Update method call
                if (review == null)
                {
                    return NotFound($"Review for Car ID {carId} not found.");
                }
                return Ok(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the review for Car ID {carId}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


       
        [HttpPost]
        [Authorize(Roles = "Customer")]
        
        public async Task<IActionResult> AddReview([FromBody] Review review)
        {
            if (review == null)
            {
                return BadRequest("Review data is required.");
            }

            if (review.CarId <= 0)
            {
                return BadRequest("A valid CarId is required to add a review.");
            }

            try
            {
                await _reviewRepository.AddReviewAsync(review);
                return CreatedAtAction(nameof(GetReviewByCarId), new { carId = review.CarId }, review); // Redirect to GetReviewByCarId
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new review.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPut("ByCar/{carId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> UpdateReview(int carId, [FromBody] Review review)
        {
            if (review == null)
            {
                return BadRequest("Review data is required.");
            }

            if (carId != review.CarId)
            {
                return BadRequest("Car ID mismatch.");
            }

            try
            {
                // Retrieve the review by carId and reviewId
                var existingReview = await _reviewRepository.GetReviewByCarIdAsync(carId);

                if (existingReview == null)
                {
                    return NotFound($"Review for car ID {carId} with review ID {review.ReviewId} not found.");
                }

                // Update the review
                await _reviewRepository.UpdateReviewAsync(review);
                return Ok(new { message = $"Review for car ID {carId} has been updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the review for car ID {carId}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




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



