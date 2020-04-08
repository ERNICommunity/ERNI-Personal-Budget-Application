using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Output.PendingRequests
{
    public class BudgetModel
    {
        public int Id { get; set; }

        public BudgetTypeEnum Type { get; set; }

        public string Title { get; set; }
    }
}