using DataContext.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using System.Security.Claims;

namespace CryptoManager.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class TradeController : Controller
    {
        private readonly ITradeService _tradeService;
        public TradeController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }
        [HttpPost("trade/buy/{cryptoId}")]
        public async Task<IActionResult> BuyCrypto(int cryptoId, [FromBody] int amount)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var crypto = await _tradeService.BuyCryptoAsync(userId, cryptoId, amount);
                return Ok(crypto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("trade/sell/{cryptoId}")]
        public async Task<IActionResult> SellCrypto(int cryptoId, [FromBody] int amount)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var crypto = await _tradeService.SellCryptoAsync(userId, cryptoId, amount);
                return Ok(crypto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("portfolio/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPortfolio(int userId)
        {
            try
            {
                var res = await _tradeService.GetPortfolio(userId);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
