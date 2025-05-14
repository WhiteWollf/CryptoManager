using DataContext.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;

namespace CryptoManager.Controllers
{
    [ApiController]
    [Route("api/alerts")]
    [AllowAnonymous]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetActiveAlerts(int userId)
        {
            try
            {
                var res = await _alertService.GetActiveAlertsAsync(userId);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateAlert([FromBody] AlertCreateDto alertCreateDto)
        {
            try
            {
                var res = await _alertService.CreateAlertAsync(alertCreateDto);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{alertId}")]
        public async Task<IActionResult> DeleteAlert(int alertId)
        {
            try
            {
                await _alertService.DeleteAlertAsync(alertId);
                return Ok("Alert deleted successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetAlertsHistoryAsync(int userId)
        {
            try
            {
                var res = await _alertService.GetAlertsHistoryAsync(userId);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
