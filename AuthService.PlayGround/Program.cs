using System.Net;
using System.Net.Mail;

var smtpClient = new SmtpClient("smtp.example.com")
{
    Port = 587,
    Credentials = new NetworkCredential("username", "password"),
    EnableSsl = true,
};
smtpClient.Send("from@example.com", "to@example.com", "subject", "body");