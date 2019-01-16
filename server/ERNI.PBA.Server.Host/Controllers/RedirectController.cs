using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class RedirectController : Controller
    {
        [HttpGet("conditions-of-use")]
        public IActionResult ConditionsOfUse(int userId, int year, CancellationToken cancellationToken)
        {
            return Redirect("https://erniegh.sharepoint.com/sites/people/benefit/ESK/Pages/Personal-budget.aspx");
        }
    }
}