namespace ERNI.PBA.Server.Domain.Models.Responses
{
    public class AdUserOutputModel
    {
        public string LastName { get; init; } = null!;

        public string FirstName { get; init; } = null!;

        public string DisplayName { get; init; } = null!;

        public string Code { get; init; } = null!;
    }
}