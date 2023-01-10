using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Domain.Models.Payloads
{
    public class RequestMassModel
    {
        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public User[] Users { get; set; } = null!;
    }
}