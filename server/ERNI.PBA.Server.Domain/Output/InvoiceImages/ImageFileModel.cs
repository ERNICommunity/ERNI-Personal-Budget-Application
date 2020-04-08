using ERNI.PBA.Server.Domain.Models;

namespace ERNI.PBA.Server.Domain.Output.InvoiceImages
{
    public class ImageFileModel
    {
        public InvoiceImage InvoiceImage { get; set; }

        public string ContentType { get; set; }
    }
}
