using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets;
using ERNI.PBA.Server.Domain.Models.Payloads;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Constants.ClientPolicy)]
public class BudgetController(
    Lazy<IGetBudgetQuery> getBudgetQuery,
    Lazy<IGetCurrentUserBudgetByYearQuery> getCurrentUserBudgetByYearQuery,
    Lazy<IGetActiveUsersBudgetsByYearQuery> getActiveUsersBudgetsByYearQuery,
    Lazy<IGetBudgetsByYearQuery> getBudgetsByYearQuery,
    Lazy<IGetUsersAvailableForBudgetQuery> getUsersAvailableForBudgetQuery,
    Lazy<ICreateBudgetsForAllActiveUsersCommand> createBudgetsForAllActiveUsersCommand,
    Lazy<ICreateBudgetCommand> createBudgetCommand,
    Lazy<IUpdateBudgetCommand> updateBudgetCommand,
    Lazy<ITransferBudgetCommand> transferBudgetCommand) : Controller
{
    [HttpGet("{budgetId}")]
    [Authorize(Policy = Constants.ClientPolicy)]
    public async Task<IActionResult> GetBudget(int budgetId, CancellationToken cancellationToken)
    {
        var outputModel = await getBudgetQuery.Value.ExecuteAsync(budgetId, HttpContext.User, cancellationToken);

        return Ok(outputModel);
    }

    [HttpGet("user/current/year/{year}")]
    [Authorize(Policy = Constants.ClientPolicy)]
    public async Task<IActionResult> GetCurrentUserBudgetByYear(int year, CancellationToken cancellationToken)
    {
        var outputModels = await getCurrentUserBudgetByYearQuery.Value.ExecuteAsync(year, HttpContext.User, cancellationToken);

        return Ok(outputModels);
    }

    [HttpGet("users/active/year/{year}")]
    [Authorize(Roles = Roles.Admin + ", " + Roles.Finance)]
    public async Task<IActionResult> GetActiveUsersBudgetsByYear(int year, CancellationToken cancellationToken)
    {
        var outputModels = await getActiveUsersBudgetsByYearQuery.Value.ExecuteAsync(year, HttpContext.User, cancellationToken);

        return Ok(outputModels);
    }

    [HttpGet("year/{year}")]
    [Authorize(Roles = Roles.Admin + ", " + Roles.Finance)]
    public async Task<IActionResult> GetBudgetsByYear(int year, CancellationToken cancellationToken)
    {
        var outputModels = await getBudgetsByYearQuery.Value.ExecuteAsync(year, HttpContext.User, cancellationToken);

        return Ok(outputModels);
    }

    [HttpGet("usersAvailableForBudgetType/{budgetTypeId}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetUsersAvailableForBudget(BudgetTypeEnum budgetTypeId, CancellationToken cancellationToken)
    {
        var outputModels = await getUsersAvailableForBudgetQuery.Value.ExecuteAsync(budgetTypeId, HttpContext.User, cancellationToken);

        return Ok(outputModels);
    }

    [HttpPost("users/all")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateBudgetsForAllActiveUsers([FromBody] CreateBudgetsForAllActiveUsersRequest payload, CancellationToken cancellationToken)
    {
        await createBudgetsForAllActiveUsersCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetRequest payload, CancellationToken cancellationToken)
    {
        await createBudgetCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

        return Ok();
    }

    [HttpPut]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateBudget([FromBody] UpdateBudgetRequest payload, CancellationToken cancellationToken)
    {
        await updateBudgetCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

        return Ok();
    }

    [HttpPut("{budgetId}/transfer/{userId}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> TransferBudget(TransferBudgetModel payload, CancellationToken cancellationToken)
    {
        await transferBudgetCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

        return Ok();
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetBudgetTypes() => await Task.FromResult(Ok(Domain.Models.BudgetType.Types));
}