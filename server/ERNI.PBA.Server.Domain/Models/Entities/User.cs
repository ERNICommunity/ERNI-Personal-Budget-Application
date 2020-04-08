using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string UniqueIdentifier { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsSuperior { get; set; }

        public bool IsViewer { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public int? SuperiorId { get; set; }

        public UserState State { get; set; }

        public User Superior { get; set; }
    }
}
