using System;

namespace ERNI.PBA.Server.DataAccess.Model
{
    public class Request
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public int CategoryId { get; set; }

        public int BudgetId { get; set; }

        public string Url { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public RequestState State { get; set; }

        public virtual Budget Budget { get; set; }

        public virtual RequestCategory Category { get; set; }

        public virtual User User { get; set; }

        public override string ToString()
        {
            return $"{Title} ({Amount}) by {Budget?.User?.FirstName} {Budget?.User?.LastName}";
        }
    }
}