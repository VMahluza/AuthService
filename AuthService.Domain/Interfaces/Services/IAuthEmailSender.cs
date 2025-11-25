using AuthService.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Interfaces.Services;
public interface IAuthEmailSender
{
    Task SendVarificationEmailAsync(User user, string token);
}