using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Examples;
using ERNI.PBA.Server.Host.Exceptions;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Model.PendingRequests;
using ERNI.PBA.Server.Host.Services;
using ERNI.PBA.Server.Host.Utils;
using ERNI.PBA.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Examples;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class RequestController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly ITeamRequestRepository _teamRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MailService _mailService;
        private readonly ILogger _logger;

        public RequestController(
            IRequestService requestService,
            IRequestRepository requestRepository,
            IUserRepository userRepository,
            IBudgetRepository budgetRepository,
            ITeamRequestRepository teamRequestRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<RequestController> logger)
        {
            _requestService = requestService;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _budgetRepository = budgetRepository;
            _teamRequestRepository = teamRequestRepository;
            _unitOfWork = unitOfWork;
            _mailService = new MailService(configuration);
            _logger = logger;
        }

        [HttpGet("budget/{budgetId}")]
        public async Task<IActionResult> GetRequests(int budgetId, CancellationToken cancellationToken)
        {
            // TODO: check for access: HttpContext.User.GetId(), 

            var requests = await _requestRepository.GetRequests(budgetId, cancellationToken);

            var result = requests.Select(_ => new
            {
                Id = _.Id,
                Title = _.Title,
                Amount = _.Amount,
                Date = _.Date,
                State = _.State,
            }).OrderByDescending(_ => _.Date);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequest(int id, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(id, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Not a valid id");
                return BadRequest("Not a valid id");
            }

            var currentUser = await _userRepository.GetUser(HttpContext.User.GetId(), cancellationToken);

            var isAdmin = currentUser.IsAdmin;
            var isSuperior = currentUser.Id == request.User.SuperiorId;
            var isViewer = currentUser.IsViewer;

            if (currentUser.Id != request.User.Id && !isAdmin && !isSuperior && !isViewer)
            {
                _logger.LogWarning("No access for request!");
                return StatusCode(401);
            }

            var result = new Request
            {
                Id = request.Id,
                Title = request.Title,
                Amount = request.Amount,
                Date = request.Date,
            };

            return Ok(result);
        }

        [HttpGet("team/{id}")]
        [Authorize(Roles = "Admin, Superior, Viewer")]
        public async Task<IActionResult> GetTeamRequest(int id, CancellationToken cancellationToken)
        {
            var request = await _teamRequestRepository.GetAsync(id);
            if (request == null)
            {
                _logger.LogWarning("Not a valid id");
                return BadRequest("Not a valid id");
            }

            var result = new Request
            {
                Id = request.Id,
                Title = request.Title,
                Amount = request.Requests.Sum(x => x.Amount),
                Date = request.Date,
            };

            return Ok(result);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveRequest(int id, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(id, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Not a valid id");
                return BadRequest("Not a valid id");
            }

            var currentUser = await _userRepository.GetUser(HttpContext.User.GetId(), cancellationToken);
            if (currentUser.IsAdmin)
            {
                request.State = RequestState.Approved;
            }
            else
            {
                var subordinates = await _userRepository.GetSubordinateUsers(currentUser.Id, cancellationToken);
                var subordinatesIds = subordinates.Select(u => u.Id).ToArray();
                if (!subordinatesIds.Contains(request.UserId))
                {
                    _logger.LogWarning($"User cannot manipulate the request id={request.Id}");
                    return BadRequest($"User cannot manipulate the request id={request.Id}");
                }

                request.State = RequestState.ApprovedBySuperior;
            }

            await _unitOfWork.SaveChanges(cancellationToken);

            string message = "Request: " + request.Title + " of amount: " + request.Amount + " has been " +
                             request.State + ".";

            _mailService.SendMail(message, request.User.Username);

            return Ok();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectRequest(int id, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(id, cancellationToken);
            if (request == null)
            {
                return BadRequest("Not a valid id");
            }

            var currentUser = await _userRepository.GetUser(HttpContext.User.GetId(), cancellationToken);
            if (!currentUser.IsAdmin)
            {
                // current user must be superior of request's user
                var subordinates = await _userRepository.GetSubordinateUsers(currentUser.Id, cancellationToken);
                var subordinatesIds = subordinates.Select(u => u.Id).ToArray();
                if (!subordinatesIds.Contains(request.UserId))
                {
                    return BadRequest($"User cannot manipulate the request id={request.Id}");
                }

                // if request was approved by admin, it cannot be rejected by superior
                if (request.State == RequestState.Approved)
                {
                    return BadRequest($"Superior cannot reject the request approved by admin. Request id={request.Id}");
                }
            }

            request.State = RequestState.Rejected;

            await _unitOfWork.SaveChanges(cancellationToken);

            _mailService.SendMail("Your request: " + request.Title + " has been " + request.State + ".", request.User.Username);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddRequest([FromBody]SingleRequestInputModel payload, CancellationToken cancellationToken)
        {
            var userId = User.GetId();
            var currentYear = DateTime.Now.Year;

            var budget = await _budgetRepository.GetBudget(payload.BudgetId, cancellationToken);

            if (budget == null)
            {
                return BadRequest($"Budget {payload.BudgetId} was not found.");
            }

            var requestedAmount = await _budgetRepository.GetTotalRequestedAmount(payload.BudgetId, cancellationToken);

            if (payload.Amount > budget.Amount - requestedAmount)
            {
                return BadRequest($"Requested amount {payload.Amount} exceeds the amount left ({requestedAmount} of {budget.Amount}).");
            }

            var request = new Request
            {
                BudgetId = budget.Id,
                UserId = userId,
                Year = currentYear,
                Title = payload.Title,
                Amount = payload.Amount,
                Date = payload.Date.ToLocalTime(),
                State = RequestState.Pending,
            };

            await _requestRepository.AddRequest(request);

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpPost("team")]
        [Authorize(Roles = nameof(Role.Superior))]
        public async Task<IActionResult> AddTeamRequest([FromBody]TeamRequestInputModel payload, CancellationToken cancellationToken)
        {
            var userId = User.GetId();

            try
            {
                var teamRequest = await _requestService.CreateTeamRequests(userId, payload, cancellationToken);
                if (teamRequest == null)
                    return BadRequest();

                await _teamRequestRepository.AddAsync(teamRequest);
                await _unitOfWork.SaveChanges(cancellationToken);
            }
            catch (NoAvailableFundsException)
            {
                return BadRequest($"Requested amount {payload.Amount} exceeds the limit.");
            }

            return Ok();
        }

        private async Task<decimal> GetRemainingAmount(Budget budget, CancellationToken cancellationToken)
        {
            return budget.Amount - await _budgetRepository.GetTotalRequestedAmount(budget.Id, cancellationToken);
        }

        /// <summary>
        /// Creates one request for each user added to mass request with enough budget left. Created requests are in Approved state
        /// </summary>
        [HttpPost("mass")]
        public async Task<IActionResult> AddRequestMass([FromBody] RequestMassModel payload, CancellationToken cancellationToken)
        {
            var currentUser = await _userRepository.GetUser(HttpContext.User.GetId(), cancellationToken);
            if (!currentUser.IsAdmin)
            {
                return StatusCode(403);
            }

            var currentYear = DateTime.Now.Year;
            var requests = new List<Request>();
            foreach (var user in payload.Users)
            {
                var userId = user.Id;

                var budgets = await _budgetRepository.GetBudgetsByType(user.Id, BudgetTypeEnum.PersonalBudget, currentYear,
                    cancellationToken);

                if (budgets.Length > 1)
                {
                    throw new InvalidOperationException($"User {user.Id} has multiple budgets of type {BudgetTypeEnum.PersonalBudget} for year {currentYear}");
                }

                var budget = budgets.Single();

                if (payload.Amount > await GetRemainingAmount(budget, cancellationToken))
                {
                    continue;
                }

                var request = new Request
                {
                    UserId = userId,
                    Year = currentYear,
                    Title = payload.Title,
                    Amount = payload.Amount,
                    Date = payload.Date.ToLocalTime().Date,
                    State = RequestState.Approved,
                };

                requests.Add(request);
            }

            await _requestRepository.AddRequests(requests);

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpGet("{year}/pending")]
        [SwaggerResponseExample(200, typeof(RequestExample))]
        public async Task<RequestModel[]> GetPendingRequests(int year, CancellationToken cancellationToken)
        {
            return await GetRequests(year, new[] { RequestState.Pending }, cancellationToken);
        }

        [HttpGet("{year}/approved")]
        [SwaggerResponseExample(200, typeof(RequestExample))]
        public async Task<RequestModel[]> GetApprovedRequests(int year, CancellationToken cancellationToken)
        {
            return await GetRequests(year, new[] { RequestState.Approved }, cancellationToken);
        }

        [HttpGet("{year}/approvedBySuperior")]
        [SwaggerResponseExample(200, typeof(RequestExample))]
        public async Task<RequestModel[]> GetApprovedBySuperiorRequests(int year, CancellationToken cancellationToken)
        {
            return await GetRequests(year, new[] { RequestState.ApprovedBySuperior }, cancellationToken);
        }

        [HttpGet("{year}/rejected")]
        [SwaggerResponseExample(200, typeof(RequestExample))]
        public async Task<RequestModel[]> GetRejectedRequests(int year, CancellationToken cancellationToken)
        {
            return await GetRequests(year, new[] { RequestState.Rejected }, cancellationToken);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRequest([FromBody] UpdateRequestModel payload, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(payload.Id, cancellationToken);

            if (request == null)
            {
                return BadRequest($"Request with id {payload.Id} not found.");
            }

            var currentUser = await _userRepository.GetUser(HttpContext.User.GetId(), cancellationToken);

            if (currentUser.Id != request.User.Id)
            {
                return BadRequest("No Access for request!");
            }

            var requestedAmount = await _budgetRepository.GetTotalRequestedAmount(request.BudgetId, cancellationToken);

            var budget = await _budgetRepository.GetBudget(request.BudgetId, cancellationToken);

            if (payload.Amount > budget.Amount - requestedAmount)
            {
                return BadRequest($"Requested amount {payload.Amount} exceeds the amount left ({requestedAmount} of {budget.Amount}).");
            }

            request.Title = payload.Title;
            request.Amount = payload.Amount;
            request.Date = payload.Date.ToLocalTime();

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpPut("team")]
        public async Task<IActionResult> UpdateTeamRequest([FromBody] UpdateRequestModel payload, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetId();
            var request = await _teamRequestRepository.GetAsync(payload.Id);

            if (request == null)
                return BadRequest($"Request with id {payload.Id} not found.");

            if (userId != request.UserId)
                return BadRequest("No Access for request!");

            var requestInputModel = new TeamRequestInputModel
            {
                RequestId = payload.Id,
                Title = payload.Title,
                Amount = payload.Amount,
                Date = payload.Date,
                Year = request.Year
            };
            try
            {
                var teamRequest = await _requestService.CreateTeamRequests(userId, requestInputModel, cancellationToken);
                if (teamRequest == null)
                    return BadRequest();

                _teamRequestRepository.Delete(request);
                await _teamRequestRepository.AddAsync(teamRequest);
                await _unitOfWork.SaveChanges(cancellationToken);
            }
            catch (NoAvailableFundsException)
            {
                return BadRequest($"Requested amount {payload.Amount} exceeds the limit.");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(id, cancellationToken);

            if (request == null)
            {
                return BadRequest("Not a valid id");
            }

            await _requestRepository.DeleteRequest(request);

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpDelete("team/{id}")]
        public async Task<IActionResult> DeleteTeamRequest(int id, CancellationToken cancellationToken)
        {
            var request = await _teamRequestRepository.GetAsync(id);
            if (request == null)
                return BadRequest("Not a valid id");

            if (HttpContext.User.GetId() != request.UserId)
                return Forbid();

            _teamRequestRepository.Delete(request);
            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        private async Task<RequestModel[]> GetRequests(int year, IEnumerable<RequestState> requestStates, CancellationToken cancellationToken)
        {
            Expression<Func<Request, bool>> predicate;

            var currentUser = await _userRepository.GetUser(HttpContext.User.GetId(), cancellationToken);
            if (currentUser.IsAdmin || currentUser.IsViewer)
            {
                predicate = request => request.Year == year && requestStates.Contains(request.State);
            }
            else
            {
                var subordinates = await _userRepository.GetSubordinateUsers(HttpContext.User.GetId(), cancellationToken);
                var subordinatesIds = subordinates.Select(u => u.Id).ToArray();
                predicate = request => request.Year == year && requestStates.Contains(request.State) && subordinatesIds.Contains(request.UserId);
            }

            var requests = await _requestRepository.GetRequests(predicate, cancellationToken);

            var result = requests.Select(GetModel).ToArray();

            return result;
        }

        private static RequestModel GetModel(Request request)
        {
            return new RequestModel
            {
                Id = request.Id,
                Title = request.Title,
                Amount = request.Amount,
                Year = request.Year,
                Date = request.Date,
                State = request.State,
                User = new UserModel
                {
                    Id = request.UserId,
                    FirstName = request.Budget.User.FirstName,
                    LastName = request.Budget.User.LastName
                },
                Budget = new BudgetModel
                {
                    Id = request.BudgetId,
                    Title = request.Budget.Title,
                    Type = request.Budget.BudgetType
                }
            };
        }

        [HttpGet("budget-left/{amount}/{categoryId}/{year}")]
        public async Task<UserOutputModel[]> BudgetLeft(decimal amount, int categoryId, int year, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllUsers(cancellationToken);
            var usersWithBudgetLeft = new List<UserOutputModel>();
            foreach (var user in users)
            {
                //if(string.Equals(await CheckAmountForRequest(user.Id, year, amount, categoryId, null, cancellationToken), validResponse))
                {
                    usersWithBudgetLeft.Add(new UserOutputModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        IsAdmin = user.IsAdmin,
                        IsSuperior = user.IsSuperior,
                        IsViewer = user.IsViewer,
                        LastName = user.LastName,
                        State = user.State,
                        Superior = user.Superior != null ? new SuperiorModel
                        {
                            FirstName = user.Superior.FirstName,
                            Id = user.Superior.Id,
                            LastName = user.Superior.LastName
                        } : null
                    });
                }
            }
            return usersWithBudgetLeft.ToArray();
        }


        //private async Task<string> aCheckAmountForRequest(int userId, int year, decimal amount, int categoryId, int? requestId, CancellationToken cancellationToken)
        //{
        //    var category = await _requestCategoryRepository.GetRequestCategory(categoryId, cancellationToken);

        //    if (category.SpendLimit != null)
        //    {
        //        decimal requestsSumForCategory = await CalculateAmountSumForCategory(userId, year, category.Id, requestId, cancellationToken);

        //        var currentAmountForCategory = category.SpendLimit - requestsSumForCategory;

        //        if (currentAmountForCategory < amount)
        //        {
        //            return "Amount of your request is over " + category.Title
        //            + " category limit: " + category.SpendLimit +
        //            ". Your current amount for this category is (" + currentAmountForCategory + ")!";
        //        }
        //    }
        //    return validResponse;
        //}

        //private async Task<decimal> aCalculateAmountSumForCategory(int userId, int year, int categoryId, int? requestId, CancellationToken cancellationToken)
        //{
        //    var requestsOfCategory = (await _requestRepository.GetRequests(userId, cancellationToken))
        //        .Where(req => req.CategoryId == categoryId && req.Id != requestId)
        //        .Where(req => req.State != RequestState.Rejected);

        //    decimal requestsSumForCategory = 0;

        //    foreach (var item in requestsOfCategory)
        //    {
        //        requestsSumForCategory += item.Amount;
        //    }

        //    return requestsSumForCategory;
        //}
    }
}