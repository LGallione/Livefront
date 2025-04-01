using System.Collections.Concurrent;
using System.Net;

namespace ReferralSystem.Api.Middleware;

public class ReferralAbusePreventionMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, HashSet<string>> _ipReferralTracking = new();

    // Configuration constants
    private const int MaxReferralsPerDay = 10;
    //    private const int MinTimeBetweenReferrals = 300; // 5 minutes in seconds
    //    private const int MaxReferralsFromSameIp = 3;

    public ReferralAbusePreventionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = GetIpAddress(context);
        var path = context.Request.Path.ToString().ToLower();

        if (path.Contains("/referrals"))
        {
            if (!await ValidateRequest(context, ipAddress, path))
            {
                return;
            }
        }

        await _next(context);
    }

    private async Task<bool> ValidateRequest(HttpContext context, string ipAddress, string path)
    {
        // Additional checks for referral creation
        if (path.Contains("/links") && context.Request.Method == "POST")
        {
            if (!ValidateReferralCreation(ipAddress))
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                await context.Response.WriteAsJsonAsync(new { error = "Daily referral limit exceeded" });
                return false;
            }
        }

        return true;
    }

    private bool ValidateReferralCreation(string ipAddress)
    {
        // Check daily referral limit
        var referrals = _ipReferralTracking.GetOrAdd(ipAddress, _ => new HashSet<string>());
        
        // Clean up old referrals (older than 24 hours)
        var oneDayAgo = DateTime.UtcNow.AddDays(-1);
        referrals.RemoveWhere(r => DateTime.Parse(r.Split('|')[1]) < oneDayAgo);

        // Add new referral with timestamp
        var newReferral = $"{Guid.NewGuid()}|{DateTime.UtcNow:O}";
        referrals.Add(newReferral);

        return referrals.Count <= MaxReferralsPerDay;
    }

    private static string GetIpAddress(HttpContext context)
    {
        // Try to get the real IP if behind a proxy
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        return ip ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
} 