using ReferralSystem.Api.Models;

namespace ReferralSystem.Api.Services;

public class MockReferralService : IReferralService
{
    private static readonly Random Random = new();
    private static readonly List<ReferralHistoryItem> ReferralHistory = new();

    public Task<ReferralLinkResponse> GenerateReferralLinkAsync(ReferralLinkRequest request)
    {
        var referralCode = GenerateReferralCode();
        var response = new ReferralLinkResponse
        {
            ReferralCode = referralCode,
            ReferralLink = $"https://example.com/refer/{referralCode}",
            ShortLink = $"https://ex.co/r/{referralCode}",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Destination = request.Destination
        };

        // Add a mock history item
        ReferralHistory.Add(new ReferralHistoryItem
        {
            Id = Guid.NewGuid().ToString(),
            Status = ReferralStatus.Pending,
            CreatedAt = DateTime.UtcNow
        });

        return Task.FromResult(response);
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

    public Task<ReferralValidationResponse> ValidateReferralLinkAsync(ReferralValidationRequest request)
    {
        var isValid = request.ReferralCode.Length == 8 && request.ReferralLink.Contains(request.ReferralCode);
        
        var response = new ReferralValidationResponse
        {
            IsValid = isValid,
            Destination = isValid ? "/welcome" : null,
            ExpiresAt = isValid ? DateTime.UtcNow.AddDays(7) : null,
            ReferrerInfo = isValid ? new ReferrerInfo
            {
                Id = Guid.NewGuid().ToString(),
                ReferralCode = request.ReferralCode
            } : null
        };

        return Task.FromResult(response);
    }

    private static string GenerateReferralCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
} 