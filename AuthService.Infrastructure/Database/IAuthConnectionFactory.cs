using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AuthService.Infrastructure.Database;
public interface IAuthConnectionFactory
{ 
    IDbConnection CreateConnection();
}

