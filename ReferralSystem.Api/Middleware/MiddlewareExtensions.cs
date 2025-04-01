namespace ReferralSystem.Api.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseReferralAbusePrevention(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ReferralAbusePreventionMiddleware>();
    }
} 