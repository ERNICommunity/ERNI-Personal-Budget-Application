using System;
using ERNI.PBA.Server.Domain.Model;

namespace ERNI.PBA.Server.Host.Model
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
