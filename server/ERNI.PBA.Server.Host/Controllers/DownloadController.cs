using System;
using ERNI.PBA.Server.Domain.Interfaces.Export;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DownloadController : ControllerBase
    {
        private readonly IDownloadTokenManager _downloadTokenManager;

        public DownloadController(IDownloadTokenManager downloadTokenManager) => _downloadTokenManager = downloadTokenManager;

        [HttpGet("token")]
        [Authorize]
        public IActionResult GetToken() => Ok(_downloadTokenManager.GenerateToken(DateTime.Now.AddSeconds(10)));
    }
}