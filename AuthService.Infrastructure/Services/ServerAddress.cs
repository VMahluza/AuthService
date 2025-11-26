using AuthService.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace AuthService.Infrastructure.Services;

public class ServerAddress : IServerAddress
{
    ILogger<ServerAddress> _logger;

    public ServerAddress(ILogger<ServerAddress> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetCurrentIPv4ServerAddress()
    {
        string ipAddress = string.Empty;

        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                    break;
                }
            }
            _logger.LogInformation("Server IPv4 Address: {IpAddress}", ipAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting server IPv4 address.");
            return string.Empty;
        }

        return ipAddress;
    }
}

