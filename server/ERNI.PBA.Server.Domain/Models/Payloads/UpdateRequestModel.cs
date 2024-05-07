namespace ERNI.PBA.Server.Domain.Models.Payloads
{
    public class UpdateRequestModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
        public string Url { get; set; } = null!;
#pragma warning restore CA1056 // URI-like properties should not be strings
    }
}