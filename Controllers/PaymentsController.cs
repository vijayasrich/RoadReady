using Microsoft.AspNetCore.Mvc;
using RoadReady.Models;
using RoadReady.Repositories;

using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentsController(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

   
    [HttpGet]
    [Authorize(Roles = "Admin,Agent,Customer")]
    public async Task<ActionResult<IEnumerable<Payment>>> GetAllPayments()
    {
        try
        {
            var payments = await _paymentRepository.GetAllPaymentsAsync();
            if (payments == null || !payments.Any())
            {
                return NotFound("No payments found.");
            }
            return Ok(payments);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving payments.", details = ex.Message });
        }
    }

   
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Agent,Customer")]
    public async Task<ActionResult<Payment>> GetPaymentById(int id)
    {
        try
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound($"Payment with ID {id} not found.");
            }
            return Ok(payment);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving the payment.", details = ex.Message });
        }
    }

   
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> AddPayment([FromBody] Payment payment)
    {
        if (payment == null)
        {
            return BadRequest("Payment data is required.");
        }

        try
        {
            await _paymentRepository.AddPaymentAsync(payment);
            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.PaymentId }, payment);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while adding the payment.", details = ex.Message });
        }
    }

   
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> UpdatePayment(int id, [FromBody] Payment payment)
    {
        if (payment == null)
        {
            return BadRequest("Payment data is required.");
        }

        if (id != payment.PaymentId)
        {
            return BadRequest("Payment ID mismatch.");
        }

        try
        {
            var existingPayment = await _paymentRepository.GetPaymentByIdAsync(id);
            if (existingPayment == null)
            {
                return NotFound($"Payment with ID {id} not found.");
            }

            await _paymentRepository.UpdatePaymentAsync(payment);
            return Ok(new { message = $"ID {id} has been updated." });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while updating the payment.", details = ex.Message });
        }
    }

    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> DeletePayment(int id)
    {
        try
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound($"Payment with ID {id} not found.");
            }

            await _paymentRepository.DeletePaymentAsync(id);
            return Ok(new { message = $"ID {id} has been deleted." });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while deleting the payment.", details = ex.Message });
        }
    }
}


