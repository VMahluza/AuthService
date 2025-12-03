using AuthService.Domain.Entities.User;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using MediatR;
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

    public LoginUserCommandHandler(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginUserResult> Handle(
        LoginUserCommand request, 
        CancellationToken cancellationToken)
    {

        // Credential Authentication:
        var user = await _userRepository.GetByUsernameAsync(request.UserName);

        // Account Status Checks:
        if (user != null)
        {
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


        // Password Verification:
        if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            // Failed Authentication Handling:
            // record the failed login attempt.
            // if a threshold of consecutive failures is reached, mark the account as locked to mitigate brute - force attacks
            throw new UnauthorizedAccessException("Invalid username or password.");
        }



        // Successful Authentication & JWT Issuance:

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.UserName, user.Email.Value);
        return new LoginUserResult(
            user.Id, 
            user.UserName, 
            user.Email.Value, 
            token
            );
    }
}
