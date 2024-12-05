using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using RoadReady.Models;
using RoadReady.Models.DTO;
using RoadReady.Repositories;
using System.Security.Claims;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        

        public PaymentsController(IPaymentRepository paymentRepository,IMapper mapper)
        {
          
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPaymentsByUserId(int userId)
        {
            try
            {
                var payments = await _paymentRepository.GetPaymentsByUserIdAsync(userId);

                if (payments == null || !payments.Any())
                {
                    return NotFound("No payments found for this user.");
                }

                return Ok(payments);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetAllPayments()
        {
            try
            {
                // Get user ID from JWT token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Check if the user is an Admin or Agent - they can access all payments
                if (User.IsInRole("Admin") || User.IsInRole("Agent"))
                {
                    // Admin/Agent can see all payments
                    var payments = await _paymentRepository.GetAllPaymentsAsync();

                    if (payments == null || !payments.Any())
                    {
                        return NotFound("No payments found.");
                    }

                    var paymentDTOs = _mapper.Map<IEnumerable<PaymentDTO>>(payments);
                    return Ok(paymentDTOs);
                }
                else if (User.IsInRole("Customer"))
                {
                    // Customer can only see their own payments
                    if (int.TryParse(userId, out int userIdInt))
                    {
                        var payments = await _paymentRepository.GetPaymentsByUserIdAsync(userIdInt);

                        if (payments == null || !payments.Any())
                        {
                            return NotFound("No payments found for this user.");
                        }

                        var paymentDTOs = _mapper.Map<IEnumerable<PaymentDTO>>(payments);
                        return Ok(paymentDTOs);
                    }
                    else
                    {
                        return Unauthorized("Invalid user ID");
                    }
                }

                // If no role is matched (just a safety net)
                return Unauthorized("You do not have permission to view payments.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving payments.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<PaymentDTO>> GetPaymentById(int id)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentByIdAsync(id);
                if (payment == null)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }

                var paymentDTO = _mapper.Map<PaymentDTO>(payment);
                return Ok(paymentDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving the payment.", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddPayment([FromBody] PaymentDTO paymentDTO)
        {
            if (paymentDTO == null)
            {
                return BadRequest("Payment data is required.");
            }

            try
            {
                var payment = _mapper.Map<Payment>(paymentDTO);
                await _paymentRepository.AddPaymentAsync(payment);
                return CreatedAtAction(nameof(GetPaymentById), new { id = payment.PaymentId }, paymentDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while adding the payment.", details = ex.Message });
            }
        }

       
    }
}



