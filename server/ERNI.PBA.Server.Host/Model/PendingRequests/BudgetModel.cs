using ERNI.PBA.Server.Domain.Model;

namespace ERNI.PBA.Server.Host.Model.PendingRequests
{
    public class BudgetModel
    {
        public int Id { get; set; }

        public BudgetTypeEnum Type { get; set; }

        public string Title { get; set; }
    }
}