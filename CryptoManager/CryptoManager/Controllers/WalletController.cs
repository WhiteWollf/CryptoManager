using DataContext.Dtos;
using Microsoft.AspNetCore.Mvc;
using Services.Services;

namespace CryptoManager.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWallet(int userId)
        {
            try
            {
                var wallet = await _walletService.GetWalletAsync(userId);
                if (wallet == null) return NotFound();
                return Ok(wallet);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateWalletBalance(int userId, [FromBody] decimal newBalance)
        {
            try
            {
                await _walletService.UpdateWalletBalanceAsync(userId, newBalance);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteWallet(int userId)
        {
            try
            {
                await _walletService.DeleteWalletAsync(userId);
                return Ok("Wallet deleted successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
