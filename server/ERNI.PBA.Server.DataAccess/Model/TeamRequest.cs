using System;
using System.Collections.Generic;

namespace ERNI.PBA.Server.DataAccess.Model
{
    public class TeamRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public DateTime Date { get; set; }

        public RequestState State { get; set; }

        public virtual User User { get; set; }

        public virtual IList<Request> Requests { get; set; }
    }
}
