using System.Net;
using Microsoft.AspNetCore.Http;
using ReferralSystem.Api.Middleware;
using Xunit;

namespace ReferralSystem.Tests;

public class ReferralAbusePreventionTests
{
    [Fact]
    public async Task ReferralCreation_ShouldEnforceDailyLimit()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/referrals/links";
        context.Request.Method = "POST";
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");

        var middleware = new ReferralAbusePreventionMiddleware(next: (innerContext) =>
        {
            innerContext.Response.StatusCode = (int)HttpStatusCode.OK;
            return Task.CompletedTask;
        });

        // Act & Assert
        // Make 10 successful requests (at the limit)
        for (int i = 0; i < 10; i++)
        {
            await middleware.InvokeAsync(context);
            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
            context.Response.StatusCode = 0; // Reset status code
        }

        // This request should be blocked (over the limit)
        await middleware.InvokeAsync(context);
        Assert.Equal((int)HttpStatusCode.TooManyRequests, context.Response.StatusCode);
    }

    [Fact]
    public async Task NonReferralEndpoints_ShouldNotBeLimited()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/other/endpoint";
        context.Request.Method = "GET";
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.3");

        var middleware = new ReferralAbusePreventionMiddleware(next: (innerContext) =>
        {
            innerContext.Response.StatusCode = (int)HttpStatusCode.OK;
            return Task.CompletedTask;
        });

        // Act & Assert
        // Make multiple requests to non-referral endpoint
        for (int i = 0; i < 150; i++)
        {
            await middleware.InvokeAsync(context);
            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
        }
    }
} 