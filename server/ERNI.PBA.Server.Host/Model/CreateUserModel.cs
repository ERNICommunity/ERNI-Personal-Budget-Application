namespace ERNI.PBA.Server.Host.Model
{
    public class CreateUserModel : UserModelBase
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public decimal Amount { get; set; }

        public int Year { get; set; }

        public int? Superior { get; set; }
    }
}
