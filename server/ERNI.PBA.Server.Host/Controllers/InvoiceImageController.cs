using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Host.Commands.InvoiceImages;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Queries.InvoiceImages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class InvoiceImageController : Controller
    {
        private readonly IMediator _mediator;

        public InvoiceImageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("images/{requestId}")]
        [Authorize]
        public async Task<IActionResult> GetInvoiceImages(int requestId, CancellationToken cancellationToken)
        {
            var getInvoiceImagesQuery = new GetInvoiceImagesQuery
            {
                Principal = HttpContext.User,
                RequestId = requestId
            };

            var outputModels = await _mediator.Send(getInvoiceImagesQuery, cancellationToken);

            return Ok(outputModels);
        }

        [HttpGet("image/{imageId}")]
        [Authorize]
        public async Task<IActionResult> GetInvoiceImageFile(int imageId, CancellationToken cancellationToken)
        {
            var getInvoiceImageFileQuery = new GetInvoiceImageFileQuery
            {
                Principal = HttpContext.User,
                ImageId = imageId
            };

            var imageFileModel = await _mediator.Send(getInvoiceImageFileQuery, cancellationToken);

            return new FileContentResult(imageFileModel.InvoiceImage.Data, imageFileModel.ContentType)
            {
                FileDownloadName = imageFileModel.InvoiceImage.Name
            };
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> AddInvoiceImage([FromForm] InvoiceImageModel invoiceImageModel, CancellationToken cancellationToken)
        {
            var addInvoiceImageCommand = new AddInvoiceImageCommand
            {
                Principal = HttpContext.User,
                RequestId = invoiceImageModel.RequestId,
                File = invoiceImageModel.File
            };

            await _mediator.Send(addInvoiceImageCommand, cancellationToken);

            return Ok();
        }
    }
}
