using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Features.Auth.Commands.Register;

public record RegisterUserCommand(
    string Username,
    string Email,
    string Password
    ) : IRequest<RegisterUserResult>;