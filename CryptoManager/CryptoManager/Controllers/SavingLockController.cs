using DataContext.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;

namespace CryptoManager.Controllers
{
    [ApiController]
    [Route("api/savings")]
    [AllowAnonymous]
    public class SavingLockController : ControllerBase
    {
        private readonly ISavingLockService _savingLockService;

        public SavingLockController(ISavingLockService savingLockService)
        {
            _savingLockService = savingLockService;
        }

        [HttpPost("lock")]
        public async Task<IActionResult> CreateSavingLock([FromBody] SavingLockCreateDto dto)
        {
            try
            {
                var res = await _savingLockService.CreateSavingLockAsync(dto);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserSavingLockDto>> GetUserSavings(int userId)
        {
            var result = await _savingLockService.GetUserSavingLocksAsync(userId);
            return Ok(result);
        }

        [HttpPut("interest-rate")]
        public async Task<IActionResult> UpdateInterestRate([FromBody] CryptoInterestRateUpdateDto dto)
        {
            try
            {
                await _savingLockService.UpdateCryptoInterestRateAsync(dto);
                return Ok("Interest rate updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("rates")]
        public async Task<ActionResult<List<CryptoInterestRateDto>>> GetAllInterestRates()
        {
            var rates = await _savingLockService.GetAllInterestRatesAsync();
            return Ok(rates);
        }

        [HttpDelete("unlock/{lockId}")]
        public async Task<IActionResult> UnlockEarly(int lockId)
        {
            var result = await _savingLockService.UnlockSavingLockAsync(lockId);
            return Ok(result);
        }
    }
}
