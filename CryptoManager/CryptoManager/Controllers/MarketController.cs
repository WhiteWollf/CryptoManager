using DataContext.Dtos;
using DataContext.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using System.Security.Claims;

namespace CryptoManager.Controllers
{
    [ApiController]
    [Route("api/market")]
    [AllowAnonymous]
    public class MarketController : Controller
    {
        private readonly IMarketService _marketService;
        public MarketController(IMarketService marketService)
        {
            _marketService = marketService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetListings()
        {
            try
            {
                var res = await _marketService.GetMarketListingsAsync();
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("create-listing")]
        public async Task<IActionResult> CreateListing([FromBody] MarketListingCreateDto marketListingCreateDto)
        {
            try
            {
                var listing = await _marketService.CreateMarketListingAsync(marketListingCreateDto);
                return Ok(listing);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{listingId}/accept")]
        public async Task<IActionResult> AcceptListing(int listingId, [FromBody] AcceptMarketListingDto acceptDto)
        {
            try
            {
                await _marketService.AcceptMarketListingAsync(listingId, acceptDto);
                return Ok("Accepted");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{listingId}")]
        public async Task<IActionResult> DeleteMarketListing(int listingId)
        {
            try
            {
                await _marketService.DeleteMarketListingAsync(listingId);
                return Ok("Market listing is now inactive");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
