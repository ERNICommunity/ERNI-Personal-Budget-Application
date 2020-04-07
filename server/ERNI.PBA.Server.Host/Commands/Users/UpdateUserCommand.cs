using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Commands.Users
{
    public class UpdateUserCommand : CommandBase<bool>
    {
        public int Id { get; set; }

        public User Superior { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsSuperior { get; set; }

        public bool IsViewer { get; set; }

        public UserState State { get; set; }
    }
}
