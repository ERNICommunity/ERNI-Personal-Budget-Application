using ERNI.PBA.Server.Domain.Interfaces.Services;

namespace ERNI.PBA.Server.Host.Services
{
    public class MailServiceMock : IMailService
    {
        public void SendMail(string body, string emails)
        {
        }
    }
}
