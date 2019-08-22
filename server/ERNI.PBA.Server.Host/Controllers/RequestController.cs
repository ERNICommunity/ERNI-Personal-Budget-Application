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
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Model.PendingRequests;
using ERNI.PBA.Server.Host.Services;
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
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IRequestCategoryRepository _requestCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MailService _mailService;
        private readonly ILogger _logger;
        private const string validResponse = "OK";

        public RequestController(IRequestRepository requestRepository, IUserRepository userRepository, IBudgetRepository budgetRepository, IRequestCategoryRepository requestCategoryRepository, IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<RequestController> logger)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _budgetRepository = budgetRepository;
            _requestCategoryRepository = requestCategoryRepository;
            _mailService = new MailService(configuration);
            _logger = logger;
        }

        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUsersRequests(int year, CancellationToken cancellationToken)
        {
            var requests = await _requestRepository.GetRequests(year, HttpContext.User.GetId(), cancellationToken);

            var result = requests.Select(_ => new
            {
                Id = _.Id,
                Title = _.Title,
                Amount = _.Amount,
                Date = _.Date,
                Url = _.Url,
                State = _.State,
                CategoryTitle = _.Category.Title
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
                CategoryId = request.CategoryId,
                Url = request.Url
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

            string message;
            if (request.Url != null)
            {
                message = "Request: " + request.Title + " of amount: " + request.Amount + " with Url: " + request.Url + " has been " + request.State + ".";
            }
            else
            {
                message = "Request: " + request.Title + " of amount: " + request.Amount + " has been " + request.State + ".";
            }

            if (request.Category.Email != null)
            {
                var emails = request.Category.Email.Split(',').ToList();
                if (!emails.Contains(request.User.Username))
                {
                    emails.Add(request.User.Username);
                }

                _mailService.SendMail(message, string.Join(',', emails));
            }

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
        public async Task<IActionResult> AddRequest([FromBody]PostRequestModel payload, CancellationToken cancellationToken)
        {
            var userId = User.GetId();
            var currentYear = DateTime.Now.Year;

            var status = await CheckAmountForRequest(userId, currentYear, payload.Amount, payload.Category.Id, null, cancellationToken);

            if (status != validResponse)
            {
                return BadRequest(status);
            }

            var request = new Request
            {
                UserId = userId,
                Year = currentYear,
                Title = payload.Title,
                Amount = payload.Amount,
                Date = payload.Date.ToLocalTime(),
                State = RequestState.Pending,
                CategoryId = payload.Category.Id,
                Url = payload.Url
            };

            _requestRepository.AddRequest(request);

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Creates one request for each user added to mass request with enough budget left. Created requests are in Approved state
        /// </summary>
        [HttpPost("mass")]
        public async Task<IActionResult> AddRequestMass([FromBody] PostRequestMassModel payload, CancellationToken cancellationToken)
        {
            var currentYear = DateTime.Now.Year;
            var requests = new List<Request>();
            foreach (var user in payload.Users)
            {
                var userId = user.Id;
                var status = await CheckAmountForRequest(userId, currentYear, payload.Amount, payload.Category.Id, null, cancellationToken);

                if (status != validResponse)
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
                    CategoryId = payload.Category.Id,
                    Url = payload.Url
                };

                requests.Add(request);
            }

            _requestRepository.AddRequests(requests);

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
                return BadRequest("Not a valid id");
            }

            var currentUser = await _userRepository.GetUser(HttpContext.User.GetId(), cancellationToken);

            if (currentUser.Id != request.User.Id)
            {
                return BadRequest("No Access for request!");
            }

            var status = await CheckAmountForRequest(currentUser.Id, DateTime.Now.Year, payload.Amount, payload.CategoryId, request.Id, cancellationToken);

            if (status != validResponse)
            {
                return BadRequest(status);
            }

            request.Title = payload.Title;
            request.Amount = payload.Amount;
            request.CategoryId = payload.CategoryId;
            request.Date = payload.Date.ToLocalTime();
            request.Url = payload.Url;

            await _unitOfWork.SaveChanges(cancellationToken);

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

            _requestRepository.DeleteRequest(request);

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
                User = new ERNI.PBA.Server.Host.Model.PendingRequests.UserModel
                {
                    Id = request.UserId,
                    FirstName = request.Budget.User.FirstName,
                    LastName = request.Budget.User.LastName
                },
                Category = new CategoryModel
                {
                    Id = request.CategoryId,
                    Title = request.Category.Title
                }
            };
        }

        [HttpGet("budget-left/{id}/{ammount}/{categoryId}/{year}")]
        public async Task<bool> BudgetLeft(int id, decimal amount, int categoryId, int year, CancellationToken cancellationToken)
        {
            return string.Equals(await CheckAmountForRequest(id, year, amount, categoryId, null, cancellationToken), validResponse);
        }


        private async Task<string> CheckAmountForRequest(int userId, int year, decimal amount, int categoryId, int? requestId, CancellationToken cancellationToken)
        {
            decimal currentAmount = await CalculateCurrentAmount(userId, year, requestId, cancellationToken);

            if (currentAmount < amount)
            {
                return "Amount of your request is bigger than your current amount (" + currentAmount + ")!";
            }

            var category = await _requestCategoryRepository.GetRequestCategory(categoryId, cancellationToken);

            if (category.SpendLimit != null)
            {
                decimal requestsSumForCategory = await CalculateAmountSumForCategory(userId, year, category.Id, requestId, cancellationToken);

                var currentAmountForCategory = category.SpendLimit - requestsSumForCategory;

                if (currentAmountForCategory < amount)
                {
                    return "Amount of your request is over " + category.Title
                    + " category limit: " + category.SpendLimit +
                    ". Your current amount for this category is (" + currentAmountForCategory + ")!";
                }
            }
            return validResponse;
        }

        private async Task<decimal> CalculateCurrentAmount(int userId, int year, int? requestId, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(userId, year, cancellationToken);
            var requests = (await _requestRepository.GetRequests(year, userId, cancellationToken))
                .Where(req => req.Id != requestId)
                .Where(req => req.State != RequestState.Rejected);

            decimal requestsSum = 0;

            foreach (var item in requests)
            {
                requestsSum += item.Amount;
            }

            var currentAmount = budget.Amount - requestsSum;

            return currentAmount;
        }

        private async Task<decimal> CalculateAmountSumForCategory(int userId, int year, int categoryId, int? requestId, CancellationToken cancellationToken)
        {
            var requestsOfCategory = (await _requestRepository.GetRequests(year, userId, cancellationToken))
                .Where(req => req.CategoryId == categoryId && req.Id != requestId)
                .Where(req => req.State != RequestState.Rejected);

            decimal requestsSumForCategory = 0;

            foreach (var item in requestsOfCategory)
            {
                requestsSumForCategory += item.Amount;
            }

            return requestsSumForCategory;
        }
    }
}