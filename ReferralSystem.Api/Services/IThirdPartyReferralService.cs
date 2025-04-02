using ReferralSystem.Api.Models;

namespace ReferralSystem.Api.Services;

public interface IThirdPartyReferralService
{
    Task<string> GenerateReferralCodeAsync();
    Task<bool> ValidateReferralCodeAsync(string referralCode);
    Task<DateTime> GetReferralCodeExpirationAsync(string referralCode);
} 