using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Domain.Models.Responses.InvoiceImages
{
    public class ImageFileModel
    {
        public InvoiceImage InvoiceImage { get; set; }

        public string ContentType { get; set; }
    }
}
