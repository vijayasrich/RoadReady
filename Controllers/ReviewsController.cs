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
            IReservationRepository reservationRepository, 
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

       
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
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

       
        [HttpGet("ByCar/{carId}")]
        [Authorize(Roles = "Admin,Customer")]
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
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddReview([FromBody] ReviewRequestDTO reviewRequest)
        {
            try
            {
                
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
                    CreatedAt = DateTime.Now 
                };

               
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



       
    }
}

    



