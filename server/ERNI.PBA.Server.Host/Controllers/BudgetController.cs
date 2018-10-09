using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
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
            var budget = await _budgetRepository.GetBudget(userId, year, cancellationToken);

            if (budget != null)
            {
                var result = new
                {
                    Year = budget.Year,
                    Amount = budget.Amount,
                    User = new User
                    {
                        FirstName = budget.User.FirstName,
                        LastName = budget.User.LastName
                    }
                };

                return Ok(result);
            }
            else
            {
                var user = await _userRepository.GetUser(userId, cancellationToken);
                var result = new
                {
                    Year = year,
                    User = user
                };

                return Ok(result);
            }
        }

        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUserBudgetByYear(int year, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(HttpContext.User.GetId(), year, cancellationToken);

            if (budget == null)
            {
                return Ok(null);
            }

            var result = new
            {
                Year = budget.Year,
                Amount = budget.Amount,
            };

            return Ok(result);
        }

        [HttpGet("users/active/year/{year}")]
        public async Task<IActionResult> GetActiveUsersBudgetsByYear(int year, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgetsByYear(year, cancellationToken);
            var activeUsers = await _userRepository.GetAllUsers(_ => _.State == UserState.Active, cancellationToken);

            var result = from au in activeUsers
                         join b in budgets on au.Id equals b.UserId into joined
                         from j in joined.DefaultIfEmpty(new Budget())
                         select new
                         {
                             User = new
                             {
                                 Id = au.Id,
                                 FirstName = au.FirstName,
                                 LastName = au.LastName
                             },
                             Amount = j.Amount
                         };

            return Ok(result);
        }

        [HttpGet("year/{year}")]
        public async Task<IActionResult> GetBudgetsByYear(int year, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetBudgetsByYear(year, cancellationToken);

            var result = budgets.Select(_ => new
            {
                Year = _.Year,
                Amount = _.Amount,
                User = new User
                {
                    FirstName = _.User.FirstName,
                    LastName = _.User.LastName
                }
            });
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

        [HttpPost]
        public async Task<IActionResult> SetBudgetsForCurrentUsers([FromBody] Budget payload, CancellationToken cancellationToken)
        {
            var year = payload.Year;
            var activeUsers = await _userRepository.GetAllUsers(_ => _.State == UserState.Active, cancellationToken);
            var budgets = await _budgetRepository.GetBudgetsByYear(year, cancellationToken);
            
            foreach (var user in activeUsers)
            {
                var exists = budgets.Any(x => x.UserId == user.Id);

                if (!exists)
                {
                    var budget = new Budget()
                    {
                        UserId = user.Id,
                        Year = year,
                        Amount = payload.Amount
                    };

                    _budgetRepository.AddBudget(budget);
                }
            }

            await _unitOfWork.SaveChanges(cancellationToken);
            
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> AddOrUpdateBudget([FromBody] UpdateBudgetModel payload, CancellationToken cancellationToken)
         {
            var budget = await _budgetRepository.GetBudget(payload.User.Id, payload.Year, cancellationToken);

            if (budget == null)
            {
                budget = new Budget()
                {
                    UserId = payload.User.Id,
                    Year = payload.Year,
                    Amount = payload.Amount
                };

                _budgetRepository.AddBudget(budget);

                await _unitOfWork.SaveChanges(cancellationToken);

                return Ok();
            }
            else
            {
                budget.Amount = payload.Amount;

                await _unitOfWork.SaveChanges(cancellationToken);

                return Ok();
            }
        }
    }
}