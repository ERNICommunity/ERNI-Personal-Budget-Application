using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Model
{
    public class RequestMassModel
    {
        public System.DateTime Date { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public RequestState State { get; set; }

        public User[] Users { get; set; }
    }
}
