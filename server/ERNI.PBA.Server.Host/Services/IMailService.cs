namespace ERNI.PBA.Server.Host.Services
{
    public interface IMailService
    {
        void SendMail(string body, string emails);
    }
}
