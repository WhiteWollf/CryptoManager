using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using System.Security.Claims;

namespace CryptoManager.Controllers
{
    [ApiController]
    [Route("api/profit")]
    public class ProfitLossController : Controller
    {
        private readonly IProfitLossService _profitLossService;
        public ProfitLossController(IProfitLossService profitLossService)
        {
            _profitLossService = profitLossService;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetTotalProfitLoss(int userId)
        {
            try
            {
                var res = await _profitLossService.TotalProfitLossAsync(userId);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("details/{userId}")]
        public async Task<IActionResult> GetDetailedProfitLoss(int userId)
        {
            try
            {
                var res = await _profitLossService.DetailedProfitLossAsync(userId);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
