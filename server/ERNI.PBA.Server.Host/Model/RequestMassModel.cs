using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Model
{
    public class RequestMassModel
    {
        public System.DateTime Date { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public RequestCategory Category { get; set; }

        public RequestState State { get; set; }

        public string Url { get; set; }

        public User[] Users { get; set; }
    }
}
