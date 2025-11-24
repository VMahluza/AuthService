using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Services;
using AuthService.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace AuthService.Tests.Services;

[TestFixture]
public class JwtTokenGeneratorTests
{
    private Mock<IOptions<JwtSettingsOptions>> _mockJwtSettings;
    private JwtSettingsOptions _jwtSettings;
    private JwtTokenGenerator _jwtTokenGenerator;

    [SetUp]
    public void Setup()
    {
        _jwtSettings = new JwtSettingsOptions
        {
            Secret = "super-secret-key-that-is-long-enough-for-hmac",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 60
        };

        _mockJwtSettings = new Mock<IOptions<JwtSettingsOptions>>();
        _mockJwtSettings.Setup(x => x.Value).Returns(_jwtSettings);

        _jwtTokenGenerator = new JwtTokenGenerator(_mockJwtSettings.Object);
    }

    [Test]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userName = "testuser";
        var email = "test@example.com";

        // Act
        var token = _jwtTokenGenerator.GenerateToken(userId, userName, email);

        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty);

        // Validate token structure
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSettings.Secret));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = key
        };

        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
        Assert.That(principal, Is.Not.Null);
        Assert.That(((JwtSecurityToken)validatedToken).Issuer, Is.EqualTo(_jwtSettings.Issuer));
    }

    [Test]
    public void GenerateToken_ShouldContainCorrectClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userName = "testuser";
        var email = "test@example.com";

        // Act
        var token = _jwtTokenGenerator.GenerateToken(userId, userName, email);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        Assert.That(jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value, Is.EqualTo(userId.ToString()));
        Assert.That(jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value, Is.EqualTo(userName));
        Assert.That(jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value, Is.EqualTo(email));
        Assert.That(jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value, Is.Not.Null);
    }

    [Test]
    public void GenerateToken_ShouldHaveCorrectExpiry()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userName = "testuser";
        var email = "test@example.com";
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = _jwtTokenGenerator.GenerateToken(userId, userName, email);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var expectedExpiry = beforeGeneration.AddMinutes(_jwtSettings.ExpiryMinutes);
        Assert.That(jwtToken.ValidTo >= expectedExpiry.AddSeconds(-1) && jwtToken.ValidTo <= expectedExpiry.AddSeconds(1), Is.True);
    }
}
