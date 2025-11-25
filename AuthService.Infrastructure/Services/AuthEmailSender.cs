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

    public Task SendVarificationEmailAsync(User user, string token)
    {
        // In a real implementation, this would use SmtpClient or an HTTP API.
        // Here, we just log it so you can copy-paste the token for testing.

        // Note: We assume the API is running on https://localhost:5001
        var verificationLink = $"https://localhost:5001/api/auth/verify-email?token={token}";

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

        return Task.CompletedTask;
    }
}