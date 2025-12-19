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

    public async Task<AuthenticationResult> GenerateToken(Guid userId, string userName, string email, IEnumerable<string> roles)
    {
        var jti = Guid.NewGuid().ToString();
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, jti)
        };

        // Add role claims for authorization
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

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
            Jti: jti,
            ExpiresAt: expiresAt
        );
    }
}


