using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Payloads;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly Lazy<IGetBudgetQuery> _getBudgetQuery;
        private readonly Lazy<IGetCurrentUserBudgetByYearQuery> _getCurrentUserBudgetByYearQuery;
        private readonly Lazy<IGetActiveUsersBudgetsByYearQuery> _getActiveUsersBudgetsByYearQuery;
        private readonly Lazy<IGetBudgetsByYearQuery> _getBudgetsByYearQuery;
        private readonly Lazy<IGetUsersAvailableForBudgetQuery> _getUsersAvailableForBudgetQuery;
        private readonly Lazy<ICreateBudgetsForAllActiveUsersCommand> _createBudgetsForAllActiveUsersCommand;
        private readonly Lazy<ICreateBudgetCommand> _createBudgetCommand;
        private readonly Lazy<IUpdateBudgetCommand> _updateBudgetCommand;
        private readonly Lazy<ITransferBudgetCommand> _transferBudgetCommand;

        public BudgetController(
            Lazy<IGetBudgetQuery> getBudgetQuery,
            Lazy<IGetCurrentUserBudgetByYearQuery> getCurrentUserBudgetByYearQuery,
            Lazy<IGetActiveUsersBudgetsByYearQuery> getActiveUsersBudgetsByYearQuery,
            Lazy<IGetBudgetsByYearQuery> getBudgetsByYearQuery,
            Lazy<IGetUsersAvailableForBudgetQuery> getUsersAvailableForBudgetQuery,
            Lazy<ICreateBudgetsForAllActiveUsersCommand> createBudgetsForAllActiveUsersCommand,
            Lazy<ICreateBudgetCommand> createBudgetCommand,
            Lazy<IUpdateBudgetCommand> updateBudgetCommand,
            Lazy<ITransferBudgetCommand> transferBudgetCommand)
        {
            _getBudgetQuery = getBudgetQuery;
            _getCurrentUserBudgetByYearQuery = getCurrentUserBudgetByYearQuery;
            _getActiveUsersBudgetsByYearQuery = getActiveUsersBudgetsByYearQuery;
            _getBudgetsByYearQuery = getBudgetsByYearQuery;
            _getUsersAvailableForBudgetQuery = getUsersAvailableForBudgetQuery;
            _createBudgetsForAllActiveUsersCommand = createBudgetsForAllActiveUsersCommand;
            _createBudgetCommand = createBudgetCommand;
            _updateBudgetCommand = updateBudgetCommand;
            _transferBudgetCommand = transferBudgetCommand;
        }

        [HttpGet("{budgetId}")]
        [Authorize]
        public async Task<IActionResult> GetBudget(int budgetId, CancellationToken cancellationToken)
        {
            var outputModel = await _getBudgetQuery.Value.ExecuteAsync(budgetId, HttpContext.User, cancellationToken);

            return Ok(outputModel);
        }

        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUserBudgetByYear(int year, CancellationToken cancellationToken)
        {
            var outputModels = await _getCurrentUserBudgetByYearQuery.Value.ExecuteAsync(year, HttpContext.User, cancellationToken);

            return Ok(outputModels);
        }

        [HttpGet("users/active/year/{year}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<IActionResult> GetActiveUsersBudgetsByYear(int year, CancellationToken cancellationToken)
        {
            var outputModels = await _getActiveUsersBudgetsByYearQuery.Value.ExecuteAsync(year, HttpContext.User, cancellationToken);

            return Ok(outputModels);
        }

        [HttpGet("year/{year}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<IActionResult> GetBudgetsByYear(int year, CancellationToken cancellationToken)
        {
            var outputModels = await _getBudgetsByYearQuery.Value.ExecuteAsync(year, HttpContext.User, cancellationToken);

            return Ok(outputModels);
        }

        [HttpGet("usersAvailableForBudgetType/{budgetTypeId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetUsersAvailableForBudget(BudgetTypeEnum budgetTypeId, CancellationToken cancellationToken)
        {
            var outputModels = await _getUsersAvailableForBudgetQuery.Value.ExecuteAsync(budgetTypeId, HttpContext.User, cancellationToken);

            return Ok(outputModels);
        }

        [HttpPost("users/all")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateBudgetsForAllActiveUsers([FromBody]CreateBudgetsForAllActiveUsersRequest payload, CancellationToken cancellationToken)
        {
            await _createBudgetsForAllActiveUsersCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPost("users/{userId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateBudget(int userId, [FromBody] CreateBudgetRequest payload, CancellationToken cancellationToken)
        {
            await _createBudgetCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UpdateBudget([FromBody] UpdateBudgetRequest payload, CancellationToken cancellationToken)
        {
            await _updateBudgetCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPut("{budgetId}/transfer/{userId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> TransferBudget(TransferBudgetModel payload, CancellationToken cancellationToken)
        {
            await _transferBudgetCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetBudgetTypes()
        {
            return await Task.FromResult(Ok(BudgetType.Types));
        }
    }
}