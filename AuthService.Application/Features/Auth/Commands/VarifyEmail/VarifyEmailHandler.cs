using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Features.Auth.Commands.VarifyEmail;
public class VarifyEmailHandler : IRequestHandler<VarifyEmailCommand, VarifyEmailResult>
{
    public Task<VarifyEmailResult> Handle(VarifyEmailCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new VarifyEmailResult(true, "Email verified successfully."));
    }
}
