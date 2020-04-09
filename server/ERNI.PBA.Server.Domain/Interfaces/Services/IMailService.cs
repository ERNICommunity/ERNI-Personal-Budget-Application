namespace ERNI.PBA.Server.Domain.Interfaces.Services
{
    public interface IMailService
    {
        void SendMail(string body, string emails);
    }
}
