using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using AuthService.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResult>
{
    private readonly ILogger<ResetPasswordCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;

    public ResetPasswordCommandHandler(
        ILogger<ResetPasswordCommandHandler> logger,
        IUserRepository userRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IPasswordHasher passwordHasher,
        IUserSessionRepository userSessionRepository,
        IAuditLogRepository auditLogRepository,
        IServerAddress serverAddress)
    {
        _logger = logger;
        _userRepository = userRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _passwordHasher = passwordHasher;
        _userSessionRepository = userSessionRepository;
        _auditLogRepository = auditLogRepository;
        _serverAddress = serverAddress;
    }

    public async Task<ResetPasswordResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // Validate token
        var resetToken = await _passwordResetTokenRepository.GetByTokenAsync(request.Token);

        if (resetToken == null)
        {
            _logger.LogWarning("Password reset attempted with invalid token");
            throw new InvalidOperationException("Invalid or expired reset token. Please request a new password reset.");
        }

        // Check if token is expired
        if (resetToken.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Password reset attempted with expired token for user {UserId}", resetToken.UserId);
            throw new InvalidOperationException("Invalid or expired reset token. Please request a new password reset.");
        }

        // Check if token has already been used
        if (resetToken.UsedAt.HasValue)
        {
            _logger.LogWarning("Password reset attempted with already used token for user {UserId}", resetToken.UserId);
            throw new InvalidOperationException("This reset token has already been used. Please request a new password reset.");
        }

        // Get user
        var user = await _userRepository.GetByIdAsync(resetToken.UserId);
        if (user == null)
        {
            _logger.LogError("User not found for valid reset token. UserId: {UserId}", resetToken.UserId);
            throw new InvalidOperationException("User account not found.");
        }

        // Hash new password
        var newPasswordHash = _passwordHasher.Hash(request.NewPassword);

        // Update user password
        user.ResetPassword(newPasswordHash);
        await _userRepository.UpdateAsync(user);

        // Mark token as used
        resetToken.Consume();
        await _passwordResetTokenRepository.UpdateAsync(resetToken);

        // Invalidate all other reset tokens for this user
        await _passwordResetTokenRepository.InvalidateAllForUserAsync(user.Id);

        // Invalidate all active sessions (security measure - force re-login)
        await _userSessionRepository.InvalidateAllForUserAsync(user.Id);

        // Log the password reset completion
        await LogPasswordResetCompletion(user.Id);

        _logger.LogInformation("Password successfully reset for user {UserId}", user.Id);

        return new ResetPasswordResult(
            true,
            "Password reset successful. You may now log in with your new password."
        );
    }

    private async Task LogPasswordResetCompletion(Guid userId)
    {
        try
        {
            var ipAddress = await _serverAddress.GetCurrentIPv4ServerAddress();
            var auditLog = new AuditLog(
                Guid.NewGuid(),
                userId,
                AuditLogActions.PasswordResetCompleted,
                "Password was successfully reset",
                ipAddress
            );

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log password reset completion audit");
        }
    }
}
