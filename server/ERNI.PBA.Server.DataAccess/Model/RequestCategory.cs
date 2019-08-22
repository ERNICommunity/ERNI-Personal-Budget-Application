namespace ERNI.PBA.Server.DataAccess.Model
{
    public class RequestCategory
    {
        public int Id { get; set; }

        public string Title {get;set;}

        public int? SpendLimit { get; set; }

        public bool IsActive { get; set; }

        public bool IsUrlNeeded { get; set; }

        public string Email { get; set; }
    }
}