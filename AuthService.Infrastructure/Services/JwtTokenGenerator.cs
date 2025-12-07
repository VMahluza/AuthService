using AuthService.Domain.DTOs;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Infrastructure.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettingsOptions _jwtSettings;
    private readonly IBase64TokenGenerator _refreshTokenGenerator;

    public JwtTokenGenerator(IOptions<JwtSettingsOptions> jwtSettings, IBase64TokenGenerator refreshTokenGenerator)
    {
        _jwtSettings = jwtSettings.Value;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task<AuthenticationResult> GenerateToken(Guid userId, string userName, string email)
    {

        // TODO: Need to return AuthenticationResult including refresh token


        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        var utc = DateTime.UtcNow;

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        string refreshToken = await _refreshTokenGenerator.GenerateToken();

        return new AuthenticationResult(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            Jti: Guid.NewGuid().ToString(),
            ExpiresAt: expiresAt
        );
    }
}


