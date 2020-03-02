using System;

namespace ERNI.PBA.Server.Host.Model
{
    public class TeamRequestInputModel
    {
        public int RequestId { get; set; }

        public int Year { get; set; }

        public DateTime Date { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }
    }
}
