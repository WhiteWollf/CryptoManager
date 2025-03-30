using Microsoft.AspNetCore.Mvc;
using Services.Services;
using System.Security.Claims;

namespace CryptoManager.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserTransactions(int userId)
        {
            try
            {
                var res = await _transactionService.GetTransactionsAsync(userId);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("details/{transactionId}")]
        public async Task<IActionResult> GetTransactionDetails(int transactionId)
        {
            try
            {
                var res = await _transactionService.GetTransactionDetailsAsync(transactionId);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
