using AuthService.Domain.Entities.User;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Options;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using System.Text;

namespace AuthService.Application.Features.Auth.Commands.Login;

/// <summary>
/// why don’t you use abp? comes pre built with user management, auth and multitenancy
/// FastEndpoints
/// </summary>

public class LoginUserCommandHandler : 
    IRequestHandler<LoginUserCommand, LoginUserResult>
{

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly SecuritySettingsOptions _securitySettingsOptions;

    public LoginUserCommandHandler(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IOptions<SecuritySettingsOptions> securitySettingsOptions)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _securitySettingsOptions = securitySettingsOptions.Value;
    }

    public async Task<LoginUserResult> Handle(
        LoginUserCommand request, 
        CancellationToken cancellationToken)
    {
        User user = await GetUserForLoginAsync(request);
        await DoAccountStatusChecks(user);
        await VarifyPassword(request, user);

        // Successful Authentication & JWT Issuance:

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.UserName, user.Email.Value);
        return new LoginUserResult(
            user.Id,
            user.UserName,
            user.Email.Value,
            token
            );

        async Task<User> GetUserForLoginAsync(LoginUserCommand request)
        {
            User user = await _userRepository.GetByUsernameAsync(request.UserName);
            if (user == null)
            {
                // Failed Authentication Handling:
                throw new UnauthorizedAccessException("Invalid username or password.");
            }
            // Credential Authentication:
            return user;
        }
    }

    private async Task VarifyPassword(LoginUserCommand request, User? user)
    {
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
     
            user.UpdateLoginAttempts(false);
           
            if (user.FailedLoginAttempts + 1 >= _securitySettingsOptions.MaxFailedAccessAttempts)
            {
                user.LockAccount();
                await _userRepository.UpdateAsync(user);
                // Optional : Notify user of account lockout via email/SMS
                throw new UnauthorizedAccessException($"Max failed login attempts reached. Account locked. wait for {_securitySettingsOptions.DefaultLockoutTimeSpanInMinutes} Minutes and Try again");

            }
            await _userRepository.UpdateAsync(user);
            throw new UnauthorizedAccessException("Invalid username or password.");
        }
    }

    private static async Task DoAccountStatusChecks(User? user)
    {

        if (user == null) {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }
       
        switch (user.Status)
        {
            case UserStatus.Locked:
                throw new UnauthorizedAccessException("The Account is locked. please contact support");
            case UserStatus.Inactive:
                throw new UnauthorizedAccessException("The Account is inactive. please activate your account");
            case UserStatus.Suspended:
                throw new UnauthorizedAccessException("The Account is suspended. please contact support");
            case UserStatus.PendingVerification:
                throw new UnauthorizedAccessException("The Account is pending verification. please verify your account");
            default:
                break;
        }
       
    }
}
