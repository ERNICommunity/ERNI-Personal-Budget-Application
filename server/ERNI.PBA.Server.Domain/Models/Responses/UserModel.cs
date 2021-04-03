using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Responses
{
    public class UserModel
    {
        public int Id { get; init; }

        public bool IsAdmin { get; init; }

        public bool IsSuperior { get; init; }

        public bool IsViewer { get; init; }

        public string FirstName { get; init; } = null!;

        public string LastName { get; init; } = null!;

        public SuperiorModel? Superior { get; init; }

        public UserState State { get; init; }
    }
}