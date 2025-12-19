using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthService.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserSessionRepository _sessionRepository;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;
    private readonly JwtSettingsOptions _jwtSettings;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IUserSessionRepository sessionRepository,
        ILogger<RefreshTokenCommandHandler> logger,
        IOptions<JwtSettingsOptions> jwtSettings)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _sessionRepository = sessionRepository;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<RefreshTokenResult> Handle(
        RefreshTokenCommand request, 
        CancellationToken cancellationToken)
    {
        // Validate refresh token
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (refreshToken == null || !refreshToken.IsValid())
        {
            _logger.LogWarning("Invalid or expired refresh token attempt");
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        // Mark old refresh token as used (token rotation)
        refreshToken.Consume();
        await _refreshTokenRepository.UpdateAsync(refreshToken);

        // Get user and verify account status
        var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
        if (user == null)
        {
            _logger.LogWarning("Refresh token used for non-existent user: {UserId}", refreshToken.UserId);
            throw new UnauthorizedAccessException("User not found");
        }

        // Get user roles for JWT claims
        var userRoles = await _userRoleRepository.GetRolesByUserIdAsync(user.Id);
        var roleNames = userRoles.Select(r => r.Name).ToList();

        // Generate new token pair
        var newToken = await _jwtTokenGenerator.GenerateToken(
            user.Id, 
            user.UserName, 
            user.Email.Value, 
            roleNames);

        // Store new refresh token with 30-day expiration
        var newRefreshToken = Domain.Entities.Supporting.RefreshToken.Create(
            user.Id,
            newToken.RefreshToken,
            DateTime.UtcNow.AddDays(30));
        
        await _refreshTokenRepository.AddAsync(newRefreshToken);

        // Create new session with new access token
        var newSession = UserSession.Create(user.Id, newToken, newToken.ExpiresAt);
        await _sessionRepository.AddAsync(newSession);

        _logger.LogInformation("Token refreshed successfully for user {UserId}", user.Id);

        return new RefreshTokenResult(
            newToken.AccessToken,
            newToken.RefreshToken,
            newToken.ExpiresAt);
    }
}
