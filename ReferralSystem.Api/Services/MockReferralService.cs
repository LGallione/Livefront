using ReferralSystem.Api.Models;

namespace ReferralSystem.Api.Services;

public class MockReferralService : IReferralService
{
    private static readonly List<ReferralHistoryItem> ReferralHistory = new();
    private readonly IThirdPartyReferralService _thirdPartyService;

    public MockReferralService(IThirdPartyReferralService thirdPartyService)
    {
        _thirdPartyService = thirdPartyService;
    }

    public async Task<ReferralLinkResponse> GenerateReferralLinkAsync(ReferralLinkRequest request)
    {
        var referralCode = await _thirdPartyService.GenerateReferralCodeAsync();
        var expiresAt = await _thirdPartyService.GetReferralCodeExpirationAsync(referralCode);
        
        var response = new ReferralLinkResponse
        {
            ReferralCode = referralCode,
            ReferralLink = $"https://example.com/refer/{referralCode}",
            ShortLink = $"https://ex.co/r/{referralCode}",
            ExpiresAt = expiresAt,
            Destination = request.Destination
        };

        // Add a mock history item
        ReferralHistory.Add(new ReferralHistoryItem
        {
            Id = Guid.NewGuid().ToString(),
            Status = ReferralStatus.Pending,
            CreatedAt = DateTime.UtcNow
        });

        return response;
    }

    public Task<ReferralStats> GetReferralStatsAsync()
    {
        var stats = new ReferralStats
        {
            TotalReferrals = ReferralHistory.Count,
            SuccessfulReferrals = ReferralHistory.Count(r => r.Status == ReferralStatus.Completed),
            PendingReferrals = ReferralHistory.Count(r => r.Status == ReferralStatus.Pending),
            ReferralHistory = ReferralHistory.OrderByDescending(r => r.CreatedAt).ToList()
        };

        return Task.FromResult(stats);
    }

    public async Task<ReferralValidationResponse> ValidateReferralLinkAsync(ReferralValidationRequest request)
    {
        // First validate the code format
        var isValidCode = await _thirdPartyService.ValidateReferralCodeAsync(request.ReferralCode);
        
        // Then check if the code matches the one in the link
        var expectedLink = $"https://example.com/refer/{request.ReferralCode}";
        var isValidLink = isValidCode && request.ReferralLink == expectedLink;
        
        DateTime? expiresAt = null;
        if (isValidLink)
        {
            expiresAt = await _thirdPartyService.GetReferralCodeExpirationAsync(request.ReferralCode);
        }
        
        var response = new ReferralValidationResponse
        {
            IsValid = isValidLink,
            Destination = isValidLink ? "/welcome" : null,
            ExpiresAt = expiresAt,
            ReferrerInfo = isValidLink ? new ReferrerInfo
            {
                Id = Guid.NewGuid().ToString(),
                ReferralCode = request.ReferralCode
            } : null
        };

        return response;
    }
} 