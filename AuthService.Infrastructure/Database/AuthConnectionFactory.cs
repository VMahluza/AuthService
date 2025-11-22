using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AuthService.Infrastructure.Database;

public class AuthConnectionFactory : IAuthConnectionFactory
{
    private readonly string _connectionString;

    public AuthConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}
