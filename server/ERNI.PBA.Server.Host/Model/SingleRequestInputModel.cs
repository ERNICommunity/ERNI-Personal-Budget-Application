using System;

namespace ERNI.PBA.Server.Host.Model
{
    public class SingleRequestInputModel
    {
        public int BudgetId { get; set; }

        public DateTime Date { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }
    }
}