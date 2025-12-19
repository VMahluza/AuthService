using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Features.Auth.Commands.Login;

public record LoginUserResult(
Guid UserId,
string UserName,
string Email,
string AccessToken,
string RefreshToken
);
