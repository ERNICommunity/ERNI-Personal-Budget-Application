using System;

namespace ERNI.PBA.Server.Domain.Models.Payloads
{
    public class PostRequestModel
    {
        public DateTime Date { get; set; }

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public int BudgetId { get; set; }
    }
}