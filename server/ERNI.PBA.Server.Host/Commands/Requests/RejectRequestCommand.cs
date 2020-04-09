namespace ERNI.PBA.Server.Host.Commands.Requests
{
    public class RejectRequestCommand : CommandBase<bool>
    {
        public int RequestId { get; set; }
    }
}
