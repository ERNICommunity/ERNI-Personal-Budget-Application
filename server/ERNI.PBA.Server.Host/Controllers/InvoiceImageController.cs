using System;
using System.Collections.Generic;
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

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet("{requestId}")]
        public async Task<IActionResult> GetInvoiceImagesName(int requestId, CancellationToken cancellationToken)
        {
            //check if id exist 
            var imagesName = await _invoiceImageRepository.GetInvoiceImagesName(requestId, cancellationToken);
            return Ok(imagesName);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddInvoiceImage([FromForm] InvoiceImageModel invoiceImageModel,
            CancellationToken cancellationToken)
        {
            byte[] buffer;

            //Exclude from controller
            using (var memStream = new MemoryStream())
            {
                await invoiceImageModel.FileKey.CopyToAsync(memStream, cancellationToken);
                buffer = memStream.ToArray();
            }

            //Check if id is valid, and is in DB (415)
            var requestId = Convert.ToInt32(invoiceImageModel.RequestId);

            //Check if format is supported (415)
            var fullName = invoiceImageModel.FileKey.FileName;
            var extension = Path.GetExtension(fullName);
            var fileName = Path.GetFileNameWithoutExtension(fullName);

            var image = new InvoiceImage
            {
                Data = buffer, 
                Name = fileName,
                Extension = extension,
                RequestId = requestId
            };

            await _invoiceImageRepository.AddInvoiceImage(image, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }
    }
}
