using AuthService.Domain.DTOs;
using AuthService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Features.Auth.Queries.GetAuditLogsByUser;
public class GetAuditLogsByUserQueryHandler
    : IRequestHandler<GetAuditLogsByUserQuery, GetAuditLogsByUserResult>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditLogsByUserQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<GetAuditLogsByUserResult> Handle(
        GetAuditLogsByUserQuery request,
        CancellationToken cancellationToken)
    {
        var auditLogs = await _auditLogRepository.GetPagedAsync(
            request.UserId,
            request.PageNumber,
            request.PageSize);

        var auditLogDtos = auditLogs.Select(log => new AuditLogDto(
            log.Id,
            log.UserId,
            log.Action,
            log.Description,
            log.IpAddress,
            log.CreatedAt
        ));

        return new GetAuditLogsByUserResult(
            auditLogDtos,
            request.PageNumber,
            request.PageSize,
            auditLogDtos.Count()
        );
    }
}