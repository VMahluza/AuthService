using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Infrastructure.Services;

public class AuthEmailSender : IAuthEmailSender
{
    private readonly ILogger<AuthEmailSender> _logger;

    public AuthEmailSender(ILogger<AuthEmailSender> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailVerificationSuccessAsync(User user)
    {
        _logger.LogInformation(@"
            **************************************************
            EMAIL SIMULATION
            To: {Email}
            Subject: Account Verified Successfully!
            
            Dear {Username},
            Your email has been successfully verified. You can now log in to your account.
            **************************************************
            ", user.Email.Value, user.UserName);
    }

    public async Task SendVarificationEmailAsync(User user, string token)
    {

        var verificationLink = $"http://localhost:5102/api/auth/verify-email?token={token}";

        _logger.LogInformation(@"
            **************************************************
            EMAIL SIMULATION
            To: {Email}
            Subject: Verify your account
            
            Hello {Username},
            Please verify your account by clicking this link:
            {Link}
            
            (Token: {Token})
            **************************************************
            ", user.Email.Value, user.UserName, verificationLink, token);
    }
}