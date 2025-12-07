using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Interfaces.Services;
public interface IServerAddress
{
    Task<string> GetCurrentIPv4ServerAddress();
}