using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ReferralSystem.Api.Models;

public class ReferralLinkRequest
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ReferralChannel? Channel { get; set; }
    public string? Destination { get; set; }
}

public enum ReferralChannel
{
    Sms,
    Email,
    Social
}

public class ReferralLinkResponse
{
    [Required]
    public string ReferralLink { get; set; } = string.Empty;
    
    [Required]
    public string ShortLink { get; set; } = string.Empty;
    
    [Required]
    public DateTime ExpiresAt { get; set; }
    
    [Required]
    public string ReferralCode { get; set; } = string.Empty;
    
    public string? Destination { get; set; }
}

public class ReferralStats
{
    public int TotalReferrals { get; set; }
    public int SuccessfulReferrals { get; set; }
    public int PendingReferrals { get; set; }
    public List<ReferralHistoryItem> ReferralHistory { get; set; } = new();
}

public class ReferralHistoryItem
{
    public string Id { get; set; } = string.Empty;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ReferralStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ReferredUser? ReferredUser { get; set; }
}

public enum ReferralStatus
{
    Pending,
    Completed,
    Expired
}

public class ReferredUser
{
    public string Id { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
}

public class ReferralValidationRequest
{
    [Required]
    public string ReferralCode { get; set; } = string.Empty;
    
    [Required]
    public string ReferralLink { get; set; } = string.Empty;
}

public class ReferralValidationResponse
{
    [Required]
    public bool IsValid { get; set; }
    
    public string? Destination { get; set; }
    public ReferrerInfo? ReferrerInfo { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class ReferrerInfo
{
    public string Id { get; set; } = string.Empty;
    public string ReferralCode { get; set; } = string.Empty;
} 