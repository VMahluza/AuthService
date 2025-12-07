using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}