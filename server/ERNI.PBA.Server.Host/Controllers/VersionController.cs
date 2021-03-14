using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    public class VersionController : Controller
    {
        [HttpGet]
        public IActionResult Get() => Ok(GetType().Assembly.GetName().Version.ToString());
    }
}