using ReferralSystem.Api.Models;

namespace ReferralSystem.Api.Services;

public interface IReferralService
{
    Task<ReferralLinkResponse> GenerateReferralLinkAsync(ReferralLinkRequest request);
    Task<ReferralStats> GetReferralStatsAsync();
    Task<ReferralValidationResponse> ValidateReferralLinkAsync(ReferralValidationRequest request);
} 