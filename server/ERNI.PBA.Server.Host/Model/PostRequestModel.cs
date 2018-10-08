using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Model
{
    public class PostRequestModel
    {
        public System.DateTime Date { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public RequestCategory Category { get; set; }

        public string Url { get; set; }
    }
}