using ERNI.PBA.Server.Domain.Models;

namespace ERNI.PBA.Server.Domain.Output
{
    public class UserModel
    {
        public int Id { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsSuperior { get; set; }

        public bool IsViewer { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public SuperiorModel Superior { get; set; }

        public UserState State { get; set; }
    }
}