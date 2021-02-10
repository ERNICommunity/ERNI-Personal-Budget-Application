using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Domain.Models.Responses.InvoiceImages
{
    public class ImageFileModel
    {
        public InvoiceImage InvoiceImage { get; init; }

        public string ContentType { get; init; }
    }
}
