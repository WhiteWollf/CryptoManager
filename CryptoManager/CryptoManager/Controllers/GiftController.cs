using DataContext.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;

namespace CryptoManager.Controllers
{
    [ApiController]
    [Route("api/trade")]
    [AllowAnonymous]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftService;

        public GiftController(IGiftService giftService)
        {
            _giftService = giftService;
        }

        [HttpPost("gift")]
        public async Task<IActionResult> GiftListing([FromBody] GiftDto dto)
        {
            try
            {
                var res = await _giftService.GiftListingAsync(dto);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("accept/{giftId}/{accepted}")]
        public async Task<IActionResult> GiftCrypto(int giftId, bool accepted)
        {
            try
            {
                var res = await _giftService.AcceptGiftAsync(giftId, accepted);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("gifts/{userId}")]
        public async Task<IActionResult> GetGiftHistory(int userId)
        {
            try
            {
                var result = await _giftService.GetGiftHistoryAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
