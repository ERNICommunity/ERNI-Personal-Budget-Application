namespace ERNI.PBA.Server.Host.Model.PendingRequests
{
    public class RequestModel
    {
        public int Id { get; set; }

        public System.DateTime Date { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public UserModel User { get; set; }

        public CategoryModel Category { get; set; }

        public int Year { get; set; }
    }
}