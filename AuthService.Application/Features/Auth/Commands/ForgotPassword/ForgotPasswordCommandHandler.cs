using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResult>
{
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IBase64TokenGenerator _tokenGenerator;
    private readonly IAuthEmailSender _emailSender;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;

    public ForgotPasswordCommandHandler(
        ILogger<ForgotPasswordCommandHandler> logger,
        IUserRepository userRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IBase64TokenGenerator tokenGenerator,
        IAuthEmailSender emailSender,
        IAuditLogRepository auditLogRepository,
        IServerAddress serverAddress)
    {
        _logger = logger;
        _userRepository = userRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _tokenGenerator = tokenGenerator;
        _emailSender = emailSender;
        _auditLogRepository = auditLogRepository;
        _serverAddress = serverAddress;
    }

    public async Task<ForgotPasswordResult> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        // Always return a generic success message for security (don't reveal if email exists)
        const string genericMessage = "If that email is registered, a password reset link has been sent.";

        try
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                // Log the attempt but don't reveal the email doesn't exist
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
                await LogPasswordResetRequest(null, request.Email, false);
                return new ForgotPasswordResult(true, genericMessage);
            }

            // Invalidate any existing unused password reset tokens for this user
            await _passwordResetTokenRepository.InvalidateAllForUserAsync(user.Id);

            // Generate new reset token
            string token = await _tokenGenerator.GenerateToken();
            var resetToken = new PasswordResetToken(
                Guid.NewGuid(),
                user.Id,
                token,
                DateTime.UtcNow.AddHours(1), // 1 hour expiration
                null
            );

            await _passwordResetTokenRepository.AddAsync(resetToken);

            // Send reset email
            await _emailSender.SendPasswordResetEmailAsync(user, token);

            // Log successful request
            await LogPasswordResetRequest(user.Id, request.Email, true);

            _logger.LogInformation("Password reset token generated for user {UserId}", user.Id);

            return new ForgotPasswordResult(true, genericMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing forgot password request for email: {Email}", request.Email);
            // Still return generic message to not reveal errors
            return new ForgotPasswordResult(true, genericMessage);
        }
    }

    private async Task LogPasswordResetRequest(Guid? userId, string email, bool emailExists)
    {
        try
        {
            var ipAddress = await _serverAddress.GetCurrentIPv4ServerAddress();
            var description = emailExists
                ? $"Password reset requested for email {email}"
                : $"Password reset requested for non-existent email {email}";

            var auditLog = new AuditLog(
                Guid.NewGuid(),
                userId ?? Guid.Empty, // Use Empty if user doesn't exist
                AuditLogActions.PasswordResetRequested,
                description,
                ipAddress
            );

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log password reset request audit");
        }
    }
}
