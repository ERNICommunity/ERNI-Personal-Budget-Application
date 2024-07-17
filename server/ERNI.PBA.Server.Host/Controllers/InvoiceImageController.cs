using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Commands.InvoiceImages;
using ERNI.PBA.Server.Business.Queries.InvoiceImages;
using ERNI.PBA.Server.Domain.Interfaces.Export;
using ERNI.PBA.Server.Domain.Interfaces.Queries.InvoiceImages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    public class InvoiceImageController(IDownloadTokenManager downloadTokenManager) : Controller
    {
        [HttpGet("images/{requestId}")]
        [Authorize]
        public async Task<IActionResult> GetInvoiceImages(int requestId, [FromServices] IGetInvoiceImagesQuery query, CancellationToken cancellationToken)
        {
            var outputModels = await query.ExecuteAsync(requestId, HttpContext.User, cancellationToken);

            return Ok(outputModels);
        }

        [HttpGet("image/{imageId}/token")]
        [Authorize]
        public async Task<IActionResult> GetDownloadToken(int imageId, [FromServices] GetInvoiceDownloadTokenQuery query,
            CancellationToken cancellationToken) =>
            Ok(await query.ExecuteAsync(imageId, HttpContext.User, cancellationToken));


        [HttpGet("image/{token}/{imageId}")]
        public async Task<IActionResult> GetInvoiceImageFile(Guid token, int imageId, [FromServices] GetInvoiceImageFileQuery query, CancellationToken cancellationToken)
        {
            if (!downloadTokenManager.ValidateToken(token, $"{DownloadTokenCategory.Invoice}-{imageId}"))
            {
                throw new InvalidOperationException("Invalid download token");
            }

            var imageFileModel = await query.ExecuteAsync(imageId, HttpContext.User, cancellationToken);

            return File(imageFileModel.Data, imageFileModel.MimeType, imageFileModel.Filename);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddInvoiceImage([FromBody] AddInvoiceImageCommand.InvoiceImageModel invoiceImageModel, [FromServices] AddInvoiceImageCommand command, CancellationToken cancellationToken)
        {
            var id = await command.ExecuteAsync(invoiceImageModel, HttpContext.User, cancellationToken);

            return Ok(id);
        }
    }
}
