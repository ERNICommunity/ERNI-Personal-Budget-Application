using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
{
    [Route("api/[controller]")]
    // [Authorize]
    public class BudgetController : Controller
    {
        private readonly IBudgetRepository _budgetRepository;

        public BudgetController(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        [HttpGet("user/{userId}/year/{year}")]
        public async Task<IActionResult> GetUsersBudget(int userId, int year, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(userId, year, cancellationToken);

            var result = new
            {
                Year = budget.Year,
                Amount = budget.Amount,
            };

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUsersBudgets(int userId, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgetsByUser(userId, cancellationToken);

            var result = budgets.Select(_ => new
            {
                Year = _.Year,
                Amount = _.Amount,
            });

            return Ok(result);
        }

        [HttpGet("year/{year}")]
        public async Task<IActionResult> GetBudgetsOfYear(int year, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgetsByYear(year, cancellationToken);

            var result = budgets.Select(_ => new
            {
                Year = _.Year,
                User = new
                {
                    Id = _.UserId,
                    FirstName = _.User.FirstName,
                    LastName = _.User.LastName
                },
                Amount = _.Amount,
            });

            return Ok(result);
        }

        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUsersBudget(int year, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(HttpContext.User.GetId(), year, cancellationToken);

            var result = new
            {
                Year = budget.Year,
                Amount = budget.Amount,
            };

            return Ok(result);
        }

        [HttpGet("user/current")]
        public async Task<IActionResult> GetCurrentUsersBudgets(CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgetsByUser(HttpContext.User.GetId(), cancellationToken);

            var result = budgets.Select(_ => new
            {
                Year = _.Year,
                Amount = _.Amount,
            });

            return Ok(result);
        }
    }
}