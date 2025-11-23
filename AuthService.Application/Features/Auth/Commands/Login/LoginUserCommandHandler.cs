using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Features.Auth.Commands.Login;

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
        throw new NotImplementedException();
    }
}
