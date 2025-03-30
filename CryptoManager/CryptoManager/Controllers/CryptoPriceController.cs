using DataContext.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;

namespace CryptoManager.Controllers
{
    [ApiController]
    [Route("api/crypto")]
    public class CryptoPriceController : ControllerBase
    {
        private readonly ICryptoPriceService _cryptoPriceService;

        public CryptoPriceController(ICryptoPriceService cryptoPriceService)
        {
            _cryptoPriceService = cryptoPriceService;
        }

        [HttpPut("price")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCryptoPrice([FromBody] CryptoPriceChangeDto cryptoPriceChangeDto)
        {
            try
            {
                var res = await _cryptoPriceService.UpdateCryptoPriceAsync(cryptoPriceChangeDto);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("price/history/{cryptoId}")]
        [AllowAnonymous]
        public async Task<IActionResult>GetCryptoPriceChanges(int cryptoId)
        {
            try
            {
                var res = await _cryptoPriceService.GetCryptoChangesAsync(cryptoId);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
