using DataContext.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;

namespace CryptoManager.Controllers
{
    [ApiController]
    [Route("api/transactions/fees")]
    [AllowAnonymous]
    public class FeeController : ControllerBase
    {
        private readonly IFeeService _feeService;

        public FeeController(IFeeService feeService)
        {
            _feeService = feeService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetTransactionFees()
        {
            try
            {
                var res = await _feeService.GetTransactionFeesAsync();
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserTransactionFees(int userId)
        {
            try
            {
                var res = await _feeService.GetUserTransactionFeesAsync(userId);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("")]
        public async Task<IActionResult> ChangeFee([FromBody] double newFee)
        {
            try
            {
                await _feeService.ChangeFeeAsync(newFee);
                return Ok($"Transaction fee has succefully updated to {newFee}%");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
