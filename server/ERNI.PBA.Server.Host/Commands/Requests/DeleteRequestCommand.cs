using System.Security.Claims;

namespace ERNI.PBA.Server.Host.Commands.Requests
{
    public class DeleteRequestCommand : CommandBase<bool>
    {
        public ClaimsPrincipal Principal { get; set; }

        public int RequestId { get; set; }
    }
}
