using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Entities;
using ERNI.PBA.Server.Host.Model.PendingRequests;

namespace ERNI.PBA.Server.Host.Model
{
    public class BudgetOutputModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public decimal Amount { get; set; }

        public decimal AmountLeft { get; set; }

        public BudgetTypeEnum Type { get; set; }

        public UserOutputModel User { get; set; }

        public IEnumerable<RequestOutputModel> Requests { get; set; }
    }
}
