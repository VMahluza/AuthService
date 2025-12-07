using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Features.Auth.Commands.Login;

public record LoginUserCommand(
    string UserName,
    string Password
    ) : IRequest<LoginUserResult>;
