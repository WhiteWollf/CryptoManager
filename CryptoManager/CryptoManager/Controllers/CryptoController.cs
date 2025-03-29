using DataContext.Dtos;
using Microsoft.AspNetCore.Mvc;
using Services.Services;

namespace CryptoManager.Controllers
{
    [ApiController]
    [Route("api/cryptos")]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoService _cryptoService;

        public CryptoController(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        [HttpGet]
        public async Task<ActionResult<IList<CryptoDto>>> GetAllCryptos()
        {
            try
            {
                return Ok(await _cryptoService.GetAllCryptosAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{cryptoId}")]
        public async Task<ActionResult<CryptoDto>> GetCryptoById(int cryptoId)
        {
            try
            {
                var crypto = await _cryptoService.GetCryptoByIdAsync(cryptoId);
                if (crypto == null) return NotFound();
                return Ok(crypto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<CryptoDto>> AddCrypto([FromBody] CryptoDto newCrypto)
        {
            try
            {
                var createdCrypto = await _cryptoService.AddCryptoAsync(newCrypto);
                return CreatedAtAction(nameof(GetCryptoById), new { cryptoId = createdCrypto.Name }, createdCrypto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{cryptoId}")]
        public async Task<IActionResult> DeleteCrypto(int cryptoId)
        {
            try
            {
                await _cryptoService.DeleteCryptoAsync(cryptoId);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
