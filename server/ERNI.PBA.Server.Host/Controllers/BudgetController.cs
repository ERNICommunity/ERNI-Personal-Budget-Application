using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Host.Utils;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BudgetController(IBudgetRepository budgetRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _budgetRepository = budgetRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{budgetId}")]
        [Authorize]
        public async Task<IActionResult> GetBudget(int budgetId, CancellationToken cancellationToken)
        {
            var user = HttpContext.User;
            var budget = await _budgetRepository.GetBudget(budgetId, cancellationToken);
            if (budget == null || (!user.IsInRole(Roles.Admin) && user.GetId() != budget.UserId))
            {
                return Unauthorized();
            }

            var result = new
            {
                Id = budget.Id,
                Year = budget.Year,
                Amount = budget.Amount,
                Type = budget.BudgetType,
                User = new User
                {
                    Id = budget.User.Id,
                    FirstName = budget.User.FirstName,
                    LastName = budget.User.LastName,
                }
            };

            return Ok(result);
        }


        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUserBudgetByYear(int year, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetId();
            var budgets = await _budgetRepository.GetSingleBudgets(userId, year, cancellationToken);

            var result = budgets.Select(budget => new
            {
                Id = budget.Id,
                Year = budget.Year,
                Amount = budget.Amount,
                AmountLeft = budget.Amount - budget.Requests
                    .Where(_ => _.State != RequestState.Rejected)
                    .Sum(_ => _.Amount),
                Title = budget.Title,
                Type = budget.BudgetType,
                Requests = budget.Requests.Select(_ => new
                {
                    Id = _.Id,
                    Title = _.Title,
                    Amount = _.Amount,
                    Date = _.Date,
                    CreateDate = _.CreateDate,
                    State = _.State,
                    Transactions = _.Transactions.Select(x => new
                    {
                        Id = x.Id,
                        Amount = x.Amount
                    })
                })
            });

            return Ok(result);
        }

        [HttpGet("users/active/year/{year}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<IActionResult> GetActiveUsersBudgetsByYear(int year, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgetsByYear(year, cancellationToken);

            var amounts = (await _budgetRepository.GetTotalAmountsByYear(year, cancellationToken))
                .ToDictionary(_ => _.BudgetId, _ => _.Amount);

            var result = budgets.Select(b =>
                    new
                    {
                        Id = b.Id,
                        User = new
                        {
                            Id = b.User.Id,
                            FirstName = b.User.FirstName,
                            LastName = b.User.LastName,
                        },
                        Title = b.Title,
                        Amount = b.Amount,
                        TotalAmount = amounts[b.Id],
                        AmountLeft = b.Amount - amounts[b.Id],
                        Type = b.BudgetType
                    })
                .OrderBy(_ => _.User.LastName).ThenBy(_ => _.User.FirstName);

            return Ok(result);
        }

        [HttpGet("year/{year}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<IActionResult> GetBudgetsByYear(int year, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgetsByYear(year, cancellationToken);

            var result = budgets.Select(_ => new
            {
                Id = _.Id,
                Year = _.Year,
                Amount = _.Amount,
                Title = _.Title,
                User = new User
                {
                    FirstName = _.User.FirstName,
                    LastName = _.User.LastName
                },
                Type = _.BudgetType
            }).OrderBy(_ => _.User.LastName).ThenBy(_ => _.User.FirstName);
            return Ok(result);
        }


        [HttpGet("usersAvailableForBudgetType/{budgetTypeId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetUsersAvailableForBudget(BudgetTypeEnum budgetTypeId,
            CancellationToken cancellationToken)
        {
            IEnumerable<User> users =
                await _userRepository.GetAllUsers(_ => _.State == UserState.Active, cancellationToken);

            var budgetType = BudgetType.Types.Single(_ => _.Id == budgetTypeId);

            if (budgetType.SinglePerUser)
            {
                var budgets =
                    (await _budgetRepository.GetBudgetsByYear(DateTime.Now.Year, cancellationToken)).Where(_ =>
                        _.BudgetType == budgetType.Id).Select(_ => _.UserId).ToHashSet();
                users = users.Where(_ => !budgets.Contains(_.Id));
            }

            return Ok(users.Select(_ => new
            {
                Id = _.Id,
                FirstName = _.FirstName,
                LastName = _.LastName
            }).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName));
        }

        public class CreateBudgetsForAllActiveUsersRequest
        {
            public string Title { get; set; }
            public decimal Amount { get; set; }
            public BudgetTypeEnum BudgetType { get; set; }
        }

        [HttpPost("users/all")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateBudgetsForAllActiveUsers(
            [FromBody]CreateBudgetsForAllActiveUsersRequest payload,
            CancellationToken cancellationToken)
        {
            var currentYear = DateTime.Now.Year;

            IEnumerable<User> users =
                await _userRepository.GetAllUsers(_ => _.State == UserState.Active, cancellationToken);

            var budgetType = BudgetType.Types.Single(_ => _.Id == payload.BudgetType);

            if (budgetType.SinglePerUser)
            {
                var budgets =
                    (await _budgetRepository.GetBudgetsByYear(DateTime.Now.Year, cancellationToken)).Where(_ =>
                        _.BudgetType == budgetType.Id).Select(_ => _.UserId).ToHashSet();
                users = users.Where(_ => !budgets.Contains(_.Id));
            }

            foreach (var user in users)
            {
                var budget = new Budget()
                {
                    UserId = user.Id,
                    Year = currentYear,
                    Amount = payload.Amount,
                    BudgetType = payload.BudgetType,
                    Title = payload.Title
                };

                _budgetRepository.AddBudget(budget);
            }

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        public class CreateBudgetRequest
        {
            public string Title { get; set; }

            public decimal Amount { get; set; }

            public BudgetTypeEnum BudgetType { get; set; }
        }

        [HttpPost("users/{userId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateBudget(int userId,
            [FromBody] CreateBudgetRequest payload,
            CancellationToken cancellationToken)
        {
            var currentYear = DateTime.Now.Year;
            var user = await _userRepository.GetUser(userId, cancellationToken);

            if (user == null || user.State != UserState.Active)
            {
                return BadRequest($"No active user with id {userId} found");
            }

            var budgets = await _budgetRepository.GetBudgets(userId, currentYear, cancellationToken);

            var budgetType = BudgetType.Types.Single(_ => _.Id == payload.BudgetType);

            if (budgetType.SinglePerUser && budgets.Any(b => b.BudgetType == payload.BudgetType))
            {
                return BadRequest(
                    $"User {userId} already has a budget of type {budgetType.Name} assigned for this year");
            }

            var budget = new Budget
            {
                UserId = user.Id,
                Year = currentYear,
                Amount = payload.Amount,
                BudgetType = payload.BudgetType,
                Title = payload.Title
            };

            _budgetRepository.AddBudget(budget);

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UpdateBudget([FromBody] UpdateBudgetModel payload, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(payload.Id, cancellationToken);

            if (budget == null)
            {
                return BadRequest($"Budget with id {payload.Id} not found");
            }

            budget.Amount = payload.Amount;

            await _unitOfWork.SaveChanges(cancellationToken);
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