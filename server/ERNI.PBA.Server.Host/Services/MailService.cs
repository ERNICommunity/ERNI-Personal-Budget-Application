﻿using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace ERNI.PBA.Server.Host.Services
{
    public  class MailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly string _port;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _enableSsl;

        public MailService(IConfiguration Configuration)
        {
            _configuration = Configuration;
            _smtpServer = _configuration["MailSettings:SmtpServer"];
            _port = _configuration["MailSettings:Port"];
            _userName = _configuration["MailSettings:UserName"];
            _password = _configuration["MailSettings:Password"];
            _enableSsl = _configuration["MailSettings:EnableSsl"];
        }

        public void SendMail(string body, string To)
        {
            try
            {
                using (SmtpClient client = new SmtpClient(_smtpServer, Int32.Parse(_port)))
                {
                    client.EnableSsl = bool.Parse(_enableSsl);
                    client.Credentials = new NetworkCredential(_userName, _password);

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("pba@erni.sk");
                    mailMessage.To.Add(To);
                    mailMessage.Body = body;
                    mailMessage.Subject = "PBA Notification";

                    client.Send(mailMessage);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("MailServiceException",ex); ;
            }
        }
    }
}
