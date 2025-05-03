using CareerConnect.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OfferController : ControllerBase
{
    private readonly OfferService _offerService;

    public OfferController(OfferService offerService)
    {
        _offerService = offerService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendOffer([FromQuery] int applicationId, [FromQuery] int employerId)
    {
        try
        {
            var offer = await _offerService.SendOfferAsync(applicationId, employerId);
            return Ok(offer);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("user/{seekerId}")]
    public async Task<IActionResult> GetOffersByUser(int seekerId)
    {
        var offers = await _offerService.GetOffersByUserIdAsync(seekerId);
        return Ok(offers);
    }
}
