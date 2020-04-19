using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models
{
    public class GetRequestsModel
    {
        public int Year { get; set; }

        public RequestState[] RequestStates { get; set; }
    }
}
