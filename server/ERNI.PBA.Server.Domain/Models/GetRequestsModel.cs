using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models
{
    public class GetRequestsModel
    {
        public int Year { get; init; }

        public RequestState[] RequestStates { get; init; } = null!;
    }
}