namespace ERNI.PBA.Server.Domain.Commands.Requests
{
    public class ApproveRequestCommand : CommandBase<bool>
    {
        public int RequestId { get; set; }
    }
}
