using ReferralSystem.Api.Models;
using ReferralSystem.Api.Services;
using Xunit;

namespace ReferralSystem.Tests;

public class ReferralServiceTests
{
    private readonly IReferralService _service;
    private readonly IThirdPartyReferralService _thirdPartyService;

    public ReferralServiceTests()
    {
        _thirdPartyService = new MockThirdPartyReferralService();
        _service = new MockReferralService(_thirdPartyService);
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
        Assert.NotNull(response);
        Assert.Equal(expectedValid, response.IsValid);
        Assert.Equal(expectedValid ? "/welcome" : null, response.Destination);
        Assert.Equal(expectedValid, response.ExpiresAt.HasValue);
        Assert.Equal(expectedValid, response.ReferrerInfo != null);
    }
} 