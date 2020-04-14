using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Commands.InvoiceImages;
using ERNI.PBA.Server.Domain.Interfaces.Queries.InvoiceImages;
using ERNI.PBA.Server.Domain.Models.Payloads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class InvoiceImageController : Controller
    {
        private readonly Lazy<IGetInvoiceImagesQuery> _getInvoiceImagesQuery;
        private readonly Lazy<IGetInvoiceImageFileQuery> _getInvoiceImageFileQuery;
        private readonly Lazy<IAddInvoiceImageCommand> _addInvoiceImageCommand;

        public InvoiceImageController(
            Lazy<IGetInvoiceImagesQuery> getInvoiceImagesQuery,
            Lazy<IGetInvoiceImageFileQuery> getInvoiceImageFileQuery,
            Lazy<IAddInvoiceImageCommand> addInvoiceImageCommand)
        {
            _getInvoiceImagesQuery = getInvoiceImagesQuery;
            _getInvoiceImageFileQuery = getInvoiceImageFileQuery;
            _addInvoiceImageCommand = addInvoiceImageCommand;
        }

        [HttpGet("images/{requestId}")]
        [Authorize]
        public async Task<IActionResult> GetInvoiceImages(int requestId, CancellationToken cancellationToken)
        {
            var outputModels = await _getInvoiceImagesQuery.Value.ExecuteAsync(requestId, HttpContext.User, cancellationToken);

            return Ok(outputModels);
        }

        [HttpGet("image/{imageId}")]
        [Authorize]
        public async Task<IActionResult> GetInvoiceImageFile(int imageId, CancellationToken cancellationToken)
        {
            var imageFileModel = await _getInvoiceImageFileQuery.Value.ExecuteAsync(imageId, HttpContext.User, cancellationToken);

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
            await _addInvoiceImageCommand.Value.ExecuteAsync(invoiceImageModel, HttpContext.User, cancellationToken);

            return Ok();
        }
    }
}
