using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Payments

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            var payments = await _context.Payments.ToListAsync();

            return Ok(payments);
        }

        // GET: api/Payments/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        // POST: api/Payments
        [HttpPost]
        public async Task<ActionResult<Payment>> CreatePayment(
            Payment payment)
        {
            var enrolmentExists = await _context.Enrolments
                .AnyAsync(e => e.EnrolmentId == payment.EnrolmentId);

            if (!enrolmentExists)
            {
                return BadRequest(
                    "The specified EnrolmentId does not exist.");
            }

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPayment),
                new { id = payment.PaymentId },
                payment);
        }

        // PUT: api/Payments/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(
            int id, Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return BadRequest();
            }

            var existingPayment = await _context.Payments.FindAsync(id);

            if (existingPayment == null)
            {
                return NotFound();
            }

            var enrolmentExists = await _context.Enrolments
                .AnyAsync(e => e.EnrolmentId == payment.EnrolmentId);

            if (!enrolmentExists)
            {
                return BadRequest(
                    "The specified EnrolmentId does not exist.");
            }

            existingPayment.EnrolmentId = payment.EnrolmentId;
            existingPayment.Amount = payment.Amount;
            existingPayment.Status = payment.Status;
            existingPayment.PaymentDate = payment.PaymentDate;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Payments/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            _context.Payments.Remove(payment);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}