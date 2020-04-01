using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Model.PendingRequests
{
    public class RequestModel
    {
        public int Id { get; set; }

        public BudgetModel Budget { get; set; }

        public System.DateTime Date { get; set; }

        public System.DateTime CreateDate { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public RequestState State { get; set; }

        public UserOutputModel User { get; set; }

        public int Year { get; set; }
    }
}