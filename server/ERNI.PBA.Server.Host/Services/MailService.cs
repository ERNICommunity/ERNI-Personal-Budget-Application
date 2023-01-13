using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using ERNI.PBA.Server.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace ERNI.PBA.Server.Host.Services
{
    public class MailService : IMailService
    {
        private readonly string _smtpServer;
        private readonly string _port;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _enableSsl;

        public MailService(IConfiguration configuration)
        {
            string GetConfig(string key)
            {
                return configuration[key] ??
                       throw new InvalidOperationException($"Configuration '{key}' not found");
            }

            _smtpServer = GetConfig("MailSettings:SmtpServer");
            _port = GetConfig("MailSettings:Port");
            _userName = GetConfig("MailSettings:UserName");
            _password = GetConfig("MailSettings:Password");
            _enableSsl = GetConfig("MailSettings:EnableSsl");
        }

        public void SendMail(string body, string emails)
        {
            using var client = new SmtpClient(_smtpServer, int.Parse(_port, CultureInfo.InvariantCulture))
            {
                EnableSsl = bool.Parse(_enableSsl),
                Credentials = new NetworkCredential(_userName, _password)
            };

            using var mailMessage = new MailMessage
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
