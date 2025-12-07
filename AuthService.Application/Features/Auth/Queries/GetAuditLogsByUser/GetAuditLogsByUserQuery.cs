using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Features.Auth.Queries.GetAuditLogsByUser;

public record GetAuditLogsByUserQuery(
    Guid UserId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<GetAuditLogsByUserResult>;