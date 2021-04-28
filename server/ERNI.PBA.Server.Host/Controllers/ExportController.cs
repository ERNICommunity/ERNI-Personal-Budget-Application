using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Export;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly IExcelExportService _excelExport;
        private readonly IDownloadTokenManager _downloadTokenManager;

        public ExportController(IExcelExportService excelExport, IDownloadTokenManager downloadTokenManager)
        {
            _excelExport = excelExport;
            _downloadTokenManager = downloadTokenManager;
        }

        [HttpGet("requests/{token}/{year}/{month}")]
        public async Task<IActionResult> Get(Guid token, int year, int month, CancellationToken cancellationToken)
        {
            if (!_downloadTokenManager.ValidateToken(token))
            {
                throw new InvalidOperationException("Invalid download token");
            }

            await using var stream = new MemoryStream();
            await _excelExport.Export(stream, year, month, cancellationToken);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "users.xlsx");
        }
    }
}
