using DataContext.Dtos;
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
        [HttpPost("trade/buy")]
        public async Task<IActionResult> BuyCrypto([FromBody] CryptoBuySellDto cryptoBuySellDto)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var transaction = await _tradeService.BuyCryptoAsync(userId, cryptoBuySellDto);
                return Ok(transaction);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("trade/sell")]
        public async Task<IActionResult> SellCrypto([FromBody] CryptoBuySellDto cryptoBuySellDto)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var transaction = await _tradeService.SellCryptoAsync(userId, cryptoBuySellDto);
                return Ok(transaction);
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
