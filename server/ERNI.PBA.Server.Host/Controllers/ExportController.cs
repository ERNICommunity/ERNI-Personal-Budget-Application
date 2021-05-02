using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Export;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("token")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult GetDownloadToken([FromServices] IDownloadTokenManager downloadTokenManager) => Ok(
            downloadTokenManager.GenerateToken(DateTime.Now.AddSeconds(10), DownloadTokenCategory.ExcelExport));

        [HttpGet("requests/{token}/{year}/{month}")]
        public async Task<IActionResult> Get(Guid token, int year, int month, CancellationToken cancellationToken)
        {
            if (!_downloadTokenManager.ValidateToken(token, DownloadTokenCategory.ExcelExport))
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
