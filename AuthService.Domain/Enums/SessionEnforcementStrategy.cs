using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Enums;
/// <summary>
/// Defines how to handle sessions when max concurrent sessions is exceeded
/// </summary>
public enum SessionEnforcementStrategy
{
    /// <summary>
    /// Revoke the oldest session when limit is exceeded
    /// </summary>
    RevokeOldest = 0,

    /// <summary>
    /// Deny new login when limit is reached
    /// </summary>
    DenyNew = 1,

    /// <summary>
    /// Allow unlimited sessions (no enforcement)
    /// </summary>
    Unlimited = 2
}
