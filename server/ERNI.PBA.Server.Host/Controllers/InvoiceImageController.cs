using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Model.PendingRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class InvoiceImageController : Controller
    {
        private readonly IInvoiceImageRepository _invoiceImageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceImageController(IInvoiceImageRepository invoiceImageRepository,
            IUnitOfWork unitOfWork)
        {
            _invoiceImageRepository = invoiceImageRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("images/{requestId}")]
        [Authorize]
        public async Task<IActionResult> GetInvoiceImages(int requestId, CancellationToken cancellationToken)
        {
            var imagesName = await _invoiceImageRepository.GetInvoiceImagesIdNamePairs(requestId, cancellationToken);
            var result = imagesName.Select(image => new
            {
                Id = image.Id,
                Name = image.Name
            });
            return Ok(result);
        }

        [HttpGet("image/{imageId}")]
        [Authorize]
        public async Task<IActionResult> GetInvoiceImageFile(int imageId,
            CancellationToken cancellationToken)
        {
            var image = await _invoiceImageRepository.GetInvoiceImage(imageId, cancellationToken);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(image.Name , out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return new FileContentResult(image.Data, contentType)
            {
                FileDownloadName = image.Name 
            };
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> AddInvoiceImage([FromForm] InvoiceImageModel invoiceImageModel,
            CancellationToken cancellationToken)
        {
            byte[] buffer;
            var fullName = invoiceImageModel.File.FileName;

            if (invoiceImageModel.File.Length > 1048576)
            {
                return StatusCode(413);
            }

            using (var openReadStream = invoiceImageModel.File.OpenReadStream())
            {
                buffer = new byte[invoiceImageModel.File.Length];
                openReadStream.Read(buffer, 0, buffer.Length);
            }

            var image = new InvoiceImage
            {
                Data = buffer, 
                Name = fullName,
                RequestId = invoiceImageModel.RequestId
            };

            await _invoiceImageRepository.AddInvoiceImage(image, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }
    }
}
