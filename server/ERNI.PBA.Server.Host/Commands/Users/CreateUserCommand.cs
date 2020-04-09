using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Commands.Users
{
    public class CreateUserCommand : CommandBase<bool>
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
