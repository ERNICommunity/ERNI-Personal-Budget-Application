using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace ERNI.PBA.Server.Host.Services
{
    public class MailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly string _port;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _enableSsl;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpServer = _configuration["MailSettings:SmtpServer"];
            _port = _configuration["MailSettings:Port"];
            _userName = _configuration["MailSettings:UserName"];
            _password = _configuration["MailSettings:Password"];
            _enableSsl = _configuration["MailSettings:EnableSsl"];
        }

        public void SendMail(string body, string emails)
        {
            using (var client = new SmtpClient(_smtpServer, int.Parse(_port)))
            {
                client.EnableSsl = bool.Parse(_enableSsl);
                client.Credentials = new NetworkCredential(_userName, _password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_userName),
                    Body = body,
                    Subject = "PBA Notification",
                    To = { emails }
                };

                client.Send(mailMessage);
            }
        }
    }
}
