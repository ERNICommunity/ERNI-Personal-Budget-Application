namespace ERNI.PBA.Server.Host.Commands
{
    public class ApproveRequestCommand : CommandBase<bool>
    {
        public int RequestId { get; set; }
    }
}
