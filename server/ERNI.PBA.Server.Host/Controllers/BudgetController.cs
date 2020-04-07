using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Commands.Budgets;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Queries.Budgets;
using ERNI.PBA.Server.Host.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public BudgetController(
            IBudgetRepository budgetRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IMediator mediator)
        {
            _budgetRepository = budgetRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        [HttpGet("{budgetId}")]
        [Authorize]
        public async Task<IActionResult> GetBudget(int budgetId, CancellationToken cancellationToken)
        {
            var getBudgetQuery = new GetBudgetQuery
            {
                Principal = HttpContext.User,
                BudgetId = budgetId
            };

            var outputModel = await _mediator.Send(getBudgetQuery, cancellationToken);

            return Ok(outputModel);
        }

        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUserBudgetByYear(int year, CancellationToken cancellationToken)
        {
            var getCurrentUserBudgetByYearQuery = new GetCurrentUserBudgetByYearQuery
            {
                UserId = HttpContext.User.GetId(),
                Year = year
            };

            var outputModels = (await _mediator.Send(getCurrentUserBudgetByYearQuery, cancellationToken)).ToList();

            return Ok(outputModels);
        }

        [HttpGet("users/active/year/{year}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<IActionResult> GetActiveUsersBudgetsByYear(int year, CancellationToken cancellationToken)
        {
            var getActiveUsersBudgetsByYearQuery = new GetActiveUsersBudgetsByYearQuery { Year = year };
            var outputModels = await _mediator.Send(getActiveUsersBudgetsByYearQuery, cancellationToken);

            return Ok(outputModels);
        }

        [HttpGet("year/{year}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<IActionResult> GetBudgetsByYear(int year, CancellationToken cancellationToken)
        {
            var getBudgetsByYearQuery = new GetBudgetsByYearQuery { Year = year };
            var outputModels = await _mediator.Send(getBudgetsByYearQuery, cancellationToken);

            return Ok(outputModels);
        }

        [HttpGet("usersAvailableForBudgetType/{budgetTypeId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetUsersAvailableForBudget(BudgetTypeEnum budgetTypeId, CancellationToken cancellationToken)
        {
            var getUsersAvailableForBudgetQuery = new GetUsersAvailableForBudgetQuery { BudgetType = budgetTypeId };
            var outputModels = await _mediator.Send(getUsersAvailableForBudgetQuery, cancellationToken);

            return Ok(outputModels);
        }

        [HttpPost("users/all")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateBudgetsForAllActiveUsers([FromBody]CreateBudgetsForAllActiveUsersRequest payload, CancellationToken cancellationToken)
        {
            var createBudgetsForAllActiveUsersCommand = new CreateBudgetsForAllActiveUsersCommand
            {
                Title = payload.Title,
                CurrentYear = DateTime.Now.Year,
                Amount = payload.Amount,
                BudgetType = payload.BudgetType
            };

            await _mediator.Send(createBudgetsForAllActiveUsersCommand, cancellationToken);

            return Ok();
        }

        [HttpPost("users/{userId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateBudget(int userId, [FromBody] CreateBudgetRequest payload, CancellationToken cancellationToken)
        {
            var createBudgetCommand = new CreateBudgetCommand
            {
                UserId = userId,
                Title = payload.Title,
                CurrentYear = DateTime.Now.Year,
                Amount = payload.Amount,
                BudgetType = payload.BudgetType
            };

            await _mediator.Send(createBudgetCommand, cancellationToken);

            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UpdateBudget([FromBody] UpdateBudgetCommand payload, CancellationToken cancellationToken)
        {
            await _mediator.Send(payload, cancellationToken);

            return Ok();
        }

        [HttpPut("{budgetId}/transfer/{userId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> TransferBudget(int budgetId, int userId, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(budgetId, cancellationToken);
            if (budget == null)
            {
                return BadRequest($"Budget with id {budgetId} not found");
            }

            if (!BudgetType.Types.Single(type => type.Id == budget.BudgetType).IsTransferable)
            {
                return BadRequest($"Budget with id {budgetId} can not be transferred");
            }

            var user = await _userRepository.GetUser(userId, cancellationToken);
            if (user == null)
            {
                return BadRequest($"User with id {userId} not found");
            }

            budget.UserId = userId;
            await _unitOfWork.SaveChanges(cancellationToken);
            return Ok();
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetBudgetTypes()
        {
            return await Task.FromResult(Ok(BudgetType.Types));
        }
    }
}