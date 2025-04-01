using Microsoft.AspNetCore.Mvc;
using ReferralSystem.Api.Models;
using ReferralSystem.Api.Services;

namespace ReferralSystem.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReferralsController : ControllerBase
{
    private readonly IReferralService _referralService;

    public ReferralsController(IReferralService referralService)
    {
        _referralService = referralService;
    }

    [HttpPost("links")]
    [ProducesResponseType(typeof(ReferralLinkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ReferralLinkResponse>> GenerateReferralLink(ReferralLinkRequest request)
    {
        var response = await _referralService.GenerateReferralLinkAsync(request);
        return Ok(response);
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(ReferralStats), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReferralStats>> GetReferralStats()
    {
        var stats = await _referralService.GetReferralStatsAsync();
        return Ok(stats);
    }

    [HttpPost("validate")]
    [ProducesResponseType(typeof(ReferralValidationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<ActionResult<ReferralValidationResponse>> ValidateReferralLink(ReferralValidationRequest request)
    {
        var response = await _referralService.ValidateReferralLinkAsync(request);
        return Ok(response);
    }
} 