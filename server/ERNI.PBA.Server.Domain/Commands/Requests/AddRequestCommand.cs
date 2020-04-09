using System;

namespace ERNI.PBA.Server.Domain.Commands.Requests
{
    public class AddRequestCommand : CommandBase<bool>
    {
        public int BudgetId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public int CurrentYear { get; set; }

        public DateTime Date { get; set; }
    }
}
