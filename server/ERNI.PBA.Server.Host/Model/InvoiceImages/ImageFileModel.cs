using ERNI.PBA.Server.Domain.Model;

namespace ERNI.PBA.Server.Host.Model.InvoiceImages
{
    public class ImageFileModel
    {
        public InvoiceImage InvoiceImage { get; set; }

        public string ContentType { get; set; }
    }
}
