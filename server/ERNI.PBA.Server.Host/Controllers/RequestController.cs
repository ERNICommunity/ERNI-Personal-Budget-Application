using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Examples;
using ERNI.PBA.Server.Host.Exceptions;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Model.PendingRequests;
using ERNI.PBA.Server.Host.Services;
using ERNI.PBA.Server.Host.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

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
            var budget = await _budgetRepository.GetBudget(budgetId, cancellationToken);

            if (!HttpContext.User.IsInRole(Roles.Admin) && budget.UserId != HttpContext.User.GetId())
            {
                return Unauthorized();
            }

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
            var isViewer = currentUser.IsViewer;

            if (currentUser.Id != request.User.Id && !isAdmin && !isViewer)
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
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer + "," + Roles.Superior)]
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
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ApproveRequest(int id, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(id, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Not a valid id");
                return BadRequest("Not a valid id");
            }

            var currentUser = await _userRepository.GetUser(HttpContext.User.GetId(), cancellationToken);
            request.State = RequestState.Approved;

            await _unitOfWork.SaveChanges(cancellationToken);

            var message = "Request: " + request.Title + " of amount: " + request.Amount + " has been " +
                             request.State + ".";

            _mailService.SendMail(message, request.User.Username);

            return Ok();
        }

        [HttpPost("team/{id}/approve")]
        public async Task<IActionResult> ApproveTeamRequest(int id, CancellationToken cancellationToken)
        {
            var teamRequest = await _teamRequestRepository.GetAsync(id);
            if (teamRequest == null)
            {
                _logger.LogWarning("Not a valid id");
                return BadRequest("Not a valid id");
            }

            var isAdmin = HttpContext.User.IsInRole(Role.Admin.ToString());
            if (!isAdmin)
            {
                _logger.LogWarning($"User cannot manipulate the request id={teamRequest.Id}");
                return BadRequest($"User cannot manipulate the request id={teamRequest.Id}");
            }

            teamRequest.State = RequestState.Approved;
            foreach (var request in teamRequest.Requests)
            {
                request.State = RequestState.Approved;
            }

            string message = "Request: " + teamRequest.Title + " of amount: " + teamRequest.Requests.Sum(_ => _.Amount) + " has been " +
                             teamRequest.State + ".";

            _mailService.SendMail(message, teamRequest.User.Username);

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RejectRequest(int id, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(id, cancellationToken);
            if (request == null)
            {
                return BadRequest("Not a valid id");
            }

            request.State = RequestState.Rejected;

            await _unitOfWork.SaveChanges(cancellationToken);

            _mailService.SendMail("Your request: " + request.Title + " has been " + request.State + ".", request.User.Username);

            return Ok();
        }

        [HttpPost("team/{id}/reject")]
        public async Task<IActionResult> RejectTeamRequest(int id, CancellationToken cancellationToken)
        {
            var teamRequest = await _teamRequestRepository.GetAsync(id);
            if (teamRequest == null)
                return BadRequest("Not a valid id");

            var isAdmin = HttpContext.User.IsInRole(Role.Admin.ToString());
            if (!isAdmin)
            {
                return BadRequest($"User cannot manipulate the request id={teamRequest.Id}");
            }

            teamRequest.State = RequestState.Rejected;
            foreach (var request in teamRequest.Requests)
            {
                request.State = RequestState.Rejected;
            }

            _mailService.SendMail("Your request: " + teamRequest.Title + " has been " + teamRequest.State + ".", teamRequest.User.Username);

            await _unitOfWork.SaveChanges(cancellationToken);

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
        [Authorize(Roles = Roles.Admin)]
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
                    BudgetId = budget.Id
                };

                requests.Add(request);
            }

            await _requestRepository.AddRequests(requests);

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpGet("{year}/pending")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        [SwaggerResponseExample(200, typeof(RequestExample))]
        public async Task<RequestModel[]> GetPendingRequests(int year, CancellationToken cancellationToken)
        {
            return await GetRequests(year, new[] { RequestState.Pending }, cancellationToken);
        }

        [HttpGet("{year}/approved")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        [SwaggerResponseExample(200, typeof(RequestExample))]
        public async Task<RequestModel[]> GetApprovedRequests(int year, CancellationToken cancellationToken)
        {
            return await GetRequests(year, new[] { RequestState.Approved }, cancellationToken);
        }

        [HttpGet("{year}/approvedBySuperior")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        [SwaggerResponseExample(200, typeof(RequestExample))]
        public async Task<RequestModel[]> GetApprovedBySuperiorRequests(int year, CancellationToken cancellationToken)
        {
            return await GetRequests(year, new[] { RequestState.ApprovedBySuperior }, cancellationToken);
        }

        [HttpGet("{year}/rejected")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
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

            var user = HttpContext.User;
            if (!user.IsInRole(Roles.Admin) && user.GetId() != request.UserId)
            {
                return BadRequest("Access denied");
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
                predicate = request => request.Year == year && requestStates.Contains(request.State) && !request.TeamRequestId.HasValue;
            }
            else
            {
                var subordinates = await _userRepository.GetSubordinateUsers(HttpContext.User.GetId(), cancellationToken);
                var subordinatesIds = subordinates.Select(u => u.Id).ToArray();
                predicate = request => request.Year == year && requestStates.Contains(request.State) && subordinatesIds.Contains(request.UserId) && !request.TeamRequestId.HasValue;
            }

            var requests = await _requestRepository.GetRequests(predicate, cancellationToken);
            var teamRequests = await _teamRequestRepository.GetAllAsync(x => x.Year == year && requestStates.Contains(x.State), cancellationToken);

            var result = requests.Select(RequestModelHelper.GetModel).ToList();
            result.AddRange(teamRequests.Select(RequestModelHelper.GetModel));

            return result.ToArray();
        }

        [HttpGet("budget-left/{amount}/{year}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<UserOutputModel[]> BudgetLeft(decimal amount, int year, CancellationToken cancellationToken)
        {
            var budgetAmount = (await _budgetRepository.GetTotalAmountsByYear(year, cancellationToken))
                .ToDictionary(_ => _.BudgetId, _ => _.Amount);

            var budgets = await _budgetRepository.GetBudgets(year, BudgetTypeEnum.PersonalBudget, cancellationToken);

            var users = (await _userRepository.GetAllUsers(cancellationToken))
                .ToDictionary(_ => _.Id);

            var usersWithBudgetLeft = new List<UserOutputModel>();

            foreach (var budget in budgets)
            {
                if (budgetAmount[budget.Id] + amount <= budget.Amount)
                {
                    var user = users[budget.UserId];

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
    }
}