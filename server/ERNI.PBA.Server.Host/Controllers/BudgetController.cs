using System;
using System.Collections.Generic;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ITeamRequestRepository _teamRequestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BudgetController(
            IBudgetRepository budgetRepository,
            IUserRepository userRepository,
            ITeamRequestRepository teamRequestRepository,
            IUnitOfWork unitOfWork)
        {
            _budgetRepository = budgetRepository;
            _userRepository = userRepository;
            _teamRequestRepository = teamRequestRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("user/{userId}/year/{year}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<IActionResult> GetUserBudgetByYear(int userId, int year, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgets(userId, year, cancellationToken);

            var result = budgets.Select(budget => new
            {
                Id = budget.Id,
                Year = budget.Year,
                Amount = budget.Amount,
                User = new User
                {
                    Id = budget.User.Id,
                    FirstName = budget.User.FirstName,
                    LastName = budget.User.LastName,
                }
            });

            return Ok(result);
        }

        [HttpGet("user/team/year/{year}")]
        public async Task<IActionResult> GetTeamBudgetByYear(int year, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetId();

            var teamBudges = await _budgetRepository.GetTeamBudgets(userId, year, cancellationToken);
            if (!teamBudges.Any())
                return Ok();

            var teamRequests = await _teamRequestRepository.GetAllByUserAsync(userId);
            var amount = teamBudges.Sum(_ => _.Amount);
            var amountLeft = teamRequests.SelectMany(_ => _.Requests).Sum(_ => _.Amount);

            return Ok(new
            {
                Year = year,
                Amount = amount,
                AmountLeft = amount - amountLeft,
                Title = BudgetType.Types.Single(x => x.Id == BudgetTypeEnum.TeamBudget).Name,
                Type = BudgetTypeEnum.TeamBudget,
                TeamRequests = teamRequests.Select(x => new
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Title = x.Title,
                    Amount = x.Requests.Sum(_ => _.Amount),
                    Date = x.Date,
                    State = x.State,
                    Requests = x.Requests.Select(_ => new
                    {
                        Id = _.Id,
                        Title = _.Title,
                        Amount = _.Amount,
                        Date = _.Date,
                        State = _.State
                    })
                })
            });
        }

        [HttpGet("{budgetId}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<IActionResult> GetUserBudgetByYear(int budgetId, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(budgetId, cancellationToken);

            var result = new
            {
                Id = budget.Id,
                Year = budget.Year,
                Amount = budget.Amount,
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
            var budgets = await _budgetRepository.GetSingleBudgets(HttpContext.User.GetId(), year, cancellationToken);

            var result = budgets.Select(budget => new
            {
                Id = budget.Id,
                Year = budget.Year,
                Amount = budget.Amount,
                AmountLeft = budget.Amount - budget.Requests.Sum(_ => _.Amount),
                Title = budget.Title,
                Type = budget.BudgetType,
                Requests = budget.Requests.Select(_ => new
                {
                    Id = _.Id,
                    Title = _.Title,
                    Amount = _.Amount,
                    Date = _.Date,
                    State = _.State,
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
                        _.BudgetType == BudgetTypeEnum.PersonalBudget).Select(_ => _.UserId).ToHashSet();
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
                        _.BudgetType == BudgetTypeEnum.PersonalBudget).Select(_ => _.UserId).ToHashSet();
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

        [HttpGet("types")]
        public async Task<IActionResult> GetBudgetTypes()
        {
            return await Task.FromResult(Ok(BudgetType.Types));
        }
    }
}