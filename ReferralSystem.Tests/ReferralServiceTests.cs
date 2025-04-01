using ReferralSystem.Api.Models;
using ReferralSystem.Api.Services;
using Xunit;

namespace ReferralSystem.Tests;

public class ReferralServiceTests
{
    private readonly IReferralService _service;

    public ReferralServiceTests()
    {
        _service = new MockReferralService();
    }

    [Fact]
    public async Task GenerateReferralLink_ShouldReturnValidResponse()
    {
        // Arrange
        var request = new ReferralLinkRequest
        {
            Channel = ReferralChannel.Email,
            Destination = "/welcome"
        };

        // Act
        var response = await _service.GenerateReferralLinkAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response.ReferralLink);
        Assert.NotEmpty(response.ShortLink);
        Assert.NotEmpty(response.ReferralCode);
        Assert.Equal(request.Destination, response.Destination);
        Assert.True(response.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task GetReferralStats_ShouldReturnStats()
    {
        // Arrange
        var request = new ReferralLinkRequest { Channel = ReferralChannel.Email };
        await _service.GenerateReferralLinkAsync(request);

        // Act
        var stats = await _service.GetReferralStatsAsync();

        // Assert
        Assert.NotNull(stats);
        Assert.True(stats.TotalReferrals > 0);
        Assert.NotEmpty(stats.ReferralHistory);
    }

    [Theory]
    [InlineData("12345678", "https://example.com/refer/12345678", true)]
    [InlineData("123", "https://example.com/refer/123", false)]
    [InlineData("12345678", "https://example.com/refer/different", false)]
    public async Task ValidateReferralLink_ShouldValidateCorrectly(string code, string link, bool expectedValid)
    {
        // Arrange
        var request = new ReferralValidationRequest
        {
            ReferralCode = code,
            ReferralLink = link
        };

        // Act
        var response = await _service.ValidateReferralLinkAsync(request);

        // Assert
        Assert.Equal(expectedValid, response.IsValid);
        if (expectedValid)
        {
            Assert.NotNull(response.Destination);
            Assert.NotNull(response.ReferrerInfo);
            Assert.NotNull(response.ExpiresAt);
        }
        else
        {
            Assert.Null(response.Destination);
            Assert.Null(response.ReferrerInfo);
            Assert.Null(response.ExpiresAt);
        }
    }
} 