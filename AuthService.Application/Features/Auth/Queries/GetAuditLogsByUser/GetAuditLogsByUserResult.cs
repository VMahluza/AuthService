using AuthService.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Features.Auth.Queries.GetAuditLogsByUser;
public record GetAuditLogsByUserResult(
    IEnumerable<AuditLogDto> AuditLogs,
    int PageNumber,
    int PageSize,
    int TotalCount
);