using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.DTOs;
public record AuditLogDto(
    Guid Id,
    Guid UserId,
    string Action,
    string? Description,
    string IpAddress,
    DateTime CreatedAt
);