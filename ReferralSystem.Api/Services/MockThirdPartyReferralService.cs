using ReferralSystem.Api.Models;

namespace ReferralSystem.Api.Services;

public class MockThirdPartyReferralService : IThirdPartyReferralService
{
    private static readonly Random Random = new();

    public Task<string> GenerateReferralCodeAsync()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var code = new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
        
        return Task.FromResult(code);
    }

    public Task<bool> ValidateReferralCodeAsync(string referralCode)
    {
        // For testing purposes, we'll consider codes that are exactly 8 characters and contain only letters and numbers as valid
        var isValid = referralCode.Length == 8 && 
                     referralCode.All(c => char.IsLetterOrDigit(c));
        return Task.FromResult(isValid);
    }

    public Task<DateTime> GetReferralCodeExpirationAsync(string referralCode)
    {
        return Task.FromResult(DateTime.UtcNow.AddDays(7));
    }
} 