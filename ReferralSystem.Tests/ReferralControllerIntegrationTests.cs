using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ReferralSystem.Api;
using ReferralSystem.Api.Models;
using Xunit;

namespace ReferralSystem.Tests;

public class ReferralControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ReferralControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GenerateReferralLink_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new ReferralLinkRequest
        {
            Channel = ReferralChannel.Email,
            Destination = "/welcome"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/referrals/links", request);
        var result = await response.Content.ReadFromJsonAsync<ReferralLinkResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result.ReferralLink);
        Assert.NotEmpty(result.ShortLink);
        Assert.Equal(request.Destination, result.Destination);
    }

    [Fact]
    public async Task GetReferralStats_ReturnsSuccessResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/referrals/stats");
        var result = await response.Content.ReadFromJsonAsync<ReferralStats>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.NotNull(result.ReferralHistory);
    }

    [Fact]
    public async Task ValidateReferralLink_WithValidData_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new ReferralValidationRequest
        {
            ReferralCode = "12345678",
            ReferralLink = "https://example.com/refer/12345678"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/referrals/validate", request);
        var result = await response.Content.ReadFromJsonAsync<ReferralValidationResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.NotNull(result.Destination);
        Assert.NotNull(result.ReferrerInfo);
    }

    [Fact]
    public async Task ValidateReferralLink_WithInvalidData_ReturnsInvalidResponse()
    {
        // Arrange
        var request = new ReferralValidationRequest
        {
            ReferralCode = "123",
            ReferralLink = "https://example.com/refer/123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/referrals/validate", request);
        var result = await response.Content.ReadFromJsonAsync<ReferralValidationResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.Null(result.Destination);
        Assert.Null(result.ReferrerInfo);
    }
} 