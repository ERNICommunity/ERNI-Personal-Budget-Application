using System;

namespace ERNI.PBA.Server.Host.Model
{
    public abstract class RequestInputModel
    {
        public DateTime Date { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }
    }
}
