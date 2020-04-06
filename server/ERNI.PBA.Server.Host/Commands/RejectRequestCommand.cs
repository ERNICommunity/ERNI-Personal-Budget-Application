namespace ERNI.PBA.Server.Host.Commands
{
    public class RejectRequestCommand : CommandBase<bool>
    {
        public int RequestId { get; set; }
    }
}
