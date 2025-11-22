using AuthService.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Interfaces.Repositories;
public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByUsernameAsync(string username);
    Task<bool> ExistsAsync(string email, string username);
}