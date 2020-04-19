using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Payloads
{
    public class CreateUserModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public decimal Amount { get; set; }

        public int Year { get; set; }

        public int? Superior { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsSuperior { get; set; }

        public bool IsViewer { get; set; }

        public UserState State { get; set; }
    }
}
