using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Responses.PendingRequests
{
    public class BudgetModel
    {
        public int Id { get; set; }

        public BudgetTypeEnum Type { get; set; }

        public string Title { get; set; }
    }
}