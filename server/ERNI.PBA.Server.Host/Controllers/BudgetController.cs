using System;
using System.Collections;
using System.Collections.Generic;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

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

        [HttpGet("user/{userId}/year/{year}")]
        public async Task<IActionResult> GetUserBudgetByYear(int userId, int year, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgets(userId, year, cancellationToken);

            var result = budgets.Select(budget => new
            {
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

        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUserBudgetByYear(int year, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgets(HttpContext.User.GetId(), year, cancellationToken);

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
        public async Task<IActionResult> GetActiveUsersBudgetsByYear(int year, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgetsByYear(year, cancellationToken);
            var activeUsers = await _userRepository.GetAllUsers(_ => _.State == UserState.Active, cancellationToken);

            var amounts = (await _budgetRepository.GetTotalAmountsByYear(year, cancellationToken))
                .ToDictionary(_ => _.BudgetId, _ => _.Amount);

            var result = budgets.Select(b =>
                    new
                    {
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


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUsersBudgets(int userId, CancellationToken cancellationToken)  //not used
        {
            var budgets = await _budgetRepository.GetBudgetsByUser(userId, cancellationToken);

            var result = budgets.Select(_ => new
            {
                Year = _.Year,
                Amount = _.Amount,
            });

            return Ok(result);
        }


        [HttpGet("user/current")]
        public async Task<IActionResult> GetCurrentUsersBudgets(CancellationToken cancellationToken)  //not used
        {
            var budgets = await _budgetRepository.GetBudgetsByUser(HttpContext.User.GetId(), cancellationToken);

            var result = budgets.Select(_ => new
            {
                Year = _.Year,
                Amount = _.Amount,
            });

            return Ok(result);
        }

        [HttpGet("usersAvailableForBudgetType/{budgetTypeId}")]
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
        public async Task<IActionResult> CreateBudgetsForAllActiveUsers(
            // [FromBody] (string Title, decimal Amount, BudgetTypeEnum BudgetType) payload,
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
        public async Task<IActionResult> AddOrUpdateBudget([FromBody] UpdateBudgetModel payload, CancellationToken cancellationToken)
        {
            //var budget = await _budgetRepository.GetBudgets(payload.User.Id, payload.Year, cancellationToken);

            //if (budget == null)
            //{
            //    budget = new Budget()
            //    {
            //        UserId = payload.User.Id,
            //        Year = payload.Year,
            //        Amount = payload.Amount
            //    };

            //    _budgetRepository.AddBudget(budget);

            //    await _unitOfWork.SaveChanges(cancellationToken);

            //    return Ok();
            //}
            //else
            //{
            //    budget.Amount = payload.Amount;

            //    await _unitOfWork.SaveChanges(cancellationToken);

            //    return Ok();
            //}

            return Ok();
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetBudgetTypes()
        {
            return await Task.FromResult(Ok(BudgetType.Types));
        }
    }
}