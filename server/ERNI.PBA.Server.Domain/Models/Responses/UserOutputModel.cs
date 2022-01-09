namespace ERNI.PBA.Server.Domain.Models.Responses
{
    public class UserOutputModel
    {
        public int Id { get; init; }

        public string FirstName { get; init; } = null!;

        public string LastName { get; init; } = null!;
    }
}