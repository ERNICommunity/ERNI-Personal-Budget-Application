using System;

namespace ERNI.PBA.Server.Host.Commands.Requests
{
    public class AddTeamRequestCommand : CommandBase<bool>
    {
        public int BudgetId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public int CurrentYear { get; set; }

        public DateTime Date { get; set; }
    }
}
