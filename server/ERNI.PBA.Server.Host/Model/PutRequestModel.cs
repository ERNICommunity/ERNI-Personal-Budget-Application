namespace ERNI.PBA.Server.Host.Model
{
    public class PutRequestModel
    {
        public System.DateTime Date { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public int CategoryId { get; set; }
    }
}