using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
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

    public LoginUserCommandHandler(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginUserResult> Handle(
        LoginUserCommand request, 
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.UserName);
        if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var token = "";
        // Save the user in a session and generate a JWT TOKEN
        //var token = _tokenService.GenerateToken(user);
        return new LoginUserResult(
            user.Id, 
            user.UserName, 
            user.Email.Value, 
            token
            );
    }
}
