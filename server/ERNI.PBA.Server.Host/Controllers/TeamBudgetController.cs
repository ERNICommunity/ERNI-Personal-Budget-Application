using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class TeamBudgetController : Controller
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUserRepository _userRepository;

        public TeamBudgetController(
            IBudgetRepository budgetRepository,
            IUserRepository userRepository)
        {
            _budgetRepository = budgetRepository;
            _userRepository = userRepository;
        }

        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUserBudgetByYear(int year, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetId();
            var user = await _userRepository.GetUser(userId, cancellationToken);
            if (!user.IsSuperior)
                return Forbid();

            var budgets = await _budgetRepository.GetCumulativeBudgets(userId, year, cancellationToken);
            if (!budgets.Any())
                return Ok();

            var masterBudget = budgets.SingleOrDefault(x => x.UserId == userId);
            if (masterBudget == null)
                return BadRequest("Cumulative budget does not exists");

            var amount = budgets.Sum(_ => _.Amount);
            var amountLeft = amount - budgets.SelectMany(_ => _.Transactions ?? Enumerable.Empty<Transaction>()).Sum(_ => _.Amount);

            var model = new
            {
                Id = masterBudget.Id,
                Year = masterBudget.Year,
                Amount = amount,
                AmountLeft = amountLeft,
                Title = masterBudget.Title,
                Type = masterBudget.BudgetType,
                Requests = masterBudget.Requests.Select(_ => new
                {
                    Id = _.Id,
                    Title = _.Title,
                    Amount = _.Amount,
                    Date = _.Date,
                    State = _.State
                })
            };

            return Ok(new[] { model });
        }
    }
}