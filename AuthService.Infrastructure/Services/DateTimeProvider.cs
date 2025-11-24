using AuthService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

