namespace ERNI.PBA.Server.Domain.Commands.Requests
{
    public class RejectRequestCommand : CommandBase<bool>
    {
        public int RequestId { get; set; }
    }
}
