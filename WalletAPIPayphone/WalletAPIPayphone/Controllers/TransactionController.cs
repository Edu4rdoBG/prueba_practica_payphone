using Microsoft.AspNetCore.Mvc;
using WalletAPIPayphone.Domain.Entities;
using WalletAPIPayphone.Application.Services;

namespace WalletAPIPayphone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("wallet/{walletId}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions(int walletId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByWalletIdAsync(walletId);
                return Ok(transactions);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Billetera no encontrada.");
            }
        }

        [HttpPost("wallet/{walletId}")]
        public async Task<ActionResult<Transaction>> CreateTransaction(int walletId, [FromBody] TransactionRequest request)
        {
            try
            {
                var transaction = await _transactionService.CreateTransactionAsync(walletId, request.Amount, request.Type);
                return CreatedAtAction(nameof(GetTransactions), new { walletId = transaction.WalletId }, transaction);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Billetera no encontrada.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class TransactionRequest
    {
        public decimal Amount { get; set; }
        public string Type { get; set; } // "Debit" or "Credit"
    }
}
