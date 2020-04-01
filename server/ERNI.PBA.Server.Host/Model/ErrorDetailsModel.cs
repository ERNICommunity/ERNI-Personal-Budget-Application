using Newtonsoft.Json;

namespace ERNI.PBA.Server.Host.Model
{
    public class ErrorDetailsModel
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
