using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Repository;
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

            return Ok(budget);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUsersBudgets(int userId, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudgetsByUser(userId, cancellationToken);

            return Ok(budget);
        }

        [HttpGet("year/{year}")]
        public async Task<IActionResult> GetBudgetsOfYear(int year, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudgetsByYear(year, cancellationToken);

            return Ok(budget);
        }
    }
}