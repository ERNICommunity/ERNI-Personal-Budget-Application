using System;
using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Responses
{
    public class RequestOutputModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public DateTime CreateDate { get; set; }

        public RequestState State { get; set; }
    }
}
