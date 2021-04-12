using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Responses
{
    public class UserModel
    {
        public int Id { get; init; }

        public string FirstName { get; init; } = null!;

        public string LastName { get; init; } = null!;

        public SuperiorModel? Superior { get; init; }

        public UserState State { get; init; }
    }
}