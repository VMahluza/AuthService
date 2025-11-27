using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Entities.User;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Features.Auth.Commands.VerifyEmail;
public class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, VerifyEmailResult>
{

    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuthEmailSender _authEmailSender;
    public VerifyEmailHandler(IEmailVerificationTokenRepository emailVerificationTokenRepository, IUserRepository userRepository, IAuthEmailSender authEmailSender)
    {
        _emailVerificationTokenRepository = emailVerificationTokenRepository;
        _userRepository = userRepository;
        _authEmailSender = authEmailSender;
    }

    public async Task<VerifyEmailResult> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        EmailVerificationToken? emailToken = await CheckAndUpdateEmailVarificationToken(request);

        if (emailToken is null)
        {
            throw new NullReferenceException("Token expired or not found");
        }

        await UpdateUserStatus(emailToken);
        await UpdateUsedToken(emailToken);

        return new VerifyEmailResult(true, "Email verified successfully.");
    }

    private async Task UpdateUsedToken(EmailVerificationToken emailToken)
    {
        var updatedEmailToken = new EmailVerificationToken(
                    emailToken.Id,
                    emailToken.UserId,
                    emailToken.Token,
                    emailToken.ExpiresAt,
                    DateTime.UtcNow
                );

        await _emailVerificationTokenRepository.UpdateAsync(updatedEmailToken);
    }

    private async Task UpdateUserStatus(EmailVerificationToken emailToken)
    {
        var user = await _userRepository.GetByIdAsync(emailToken.UserId);

        if (user is null)
        {
            throw new NullReferenceException("User not found.");
        }

        var updatedUser = new User(

            user.Id,
            user.UserName,
            user.Email,
            user.PasswordHash,
            UserStatus.Active,
            user.FailedLoginAttempts
            );

        await _userRepository.UpdateAsync(updatedUser);
    }

    private async Task<EmailVerificationToken?> CheckAndUpdateEmailVarificationToken(VerifyEmailCommand request)
    {
        var emailToken = await _emailVerificationTokenRepository.GetByTokenAsync(request.token);
        if (emailToken is null)
        {
            throw new NullReferenceException("Invalid token.");
        }

        emailToken.Consume();
        return emailToken;
    }
}
