using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Enums;

public enum UserStatus
{
    Active,
    Inactive,
    Suspended,
    Locked,
    PendingVerification
}

