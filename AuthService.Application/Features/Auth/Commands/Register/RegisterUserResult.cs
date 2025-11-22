using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Features.Auth.Commands.Register;

public record RegisterUserResult(
    Guid UserId,
    string Username,
    string Email
    );