using System;
using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Outputs.PendingRequests
{
    public class RequestModel
    {
        public int Id { get; set; }

        public BudgetModel Budget { get; set; }

        public DateTime Date { get; set; }

        public DateTime CreateDate { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public RequestState State { get; set; }

        public UserOutputModel User { get; set; }

        public int Year { get; set; }
    }
}