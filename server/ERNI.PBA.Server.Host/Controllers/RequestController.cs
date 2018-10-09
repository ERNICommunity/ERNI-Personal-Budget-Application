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
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace server.Controllers
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

        public RequestController(IRequestRepository requestRepository, IUserRepository userRepository, IBudgetRepository budgetRepository, IRequestCategoryRepository requestCategoryRepository,IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _budgetRepository = budgetRepository;
            _requestCategoryRepository = requestCategoryRepository;
            _mailService = new MailService(configuration);
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
                return BadRequest("Not a valid id");
            }

            var currentUser = await _userRepository.GetUser(HttpContext.User.GetId(), cancellationToken);
            if (currentUser.Id != request.User.Id)
            {
                return BadRequest("No access for request!");
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
                    return BadRequest($"User cannot manipulate the request id={request.Id}");
                }

                request.State = RequestState.ApprovedBySuperior;
            }

            await _unitOfWork.SaveChanges(cancellationToken);

            if(request.Url != null)
            {
                _mailService.SendMail("Your request " + request.Title + "with Url: "+ request.Url +" has been " + request.State + ".", request.User.Username);
                return Ok();
            }

            _mailService.SendMail("Your request " + request.Title + " has been " + request.State + ".", request.User.Username);

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

            _mailService.SendMail("Your request " + request.Title + " has been " + request.State + ".", request.User.Username);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddRequest([FromBody]PostRequestModel payload, CancellationToken cancellationToken)
        {
            var userId = User.GetId();
            var currentYear = DateTime.Now.Year;

            decimal currentAmount = await CalculateCurrentAmount(userId, currentYear, cancellationToken);
            var status = await CheckAmountForRequest(userId, currentYear, payload.Amount, payload.Category.Id, cancellationToken);
            
            if (status != "OK")
            {
                return BadRequest(status);
            }
            
            var request = new Request
            {
                UserId = userId,
                Year = currentYear,
                Title = payload.Title,
                Amount = payload.Amount,
                Date = payload.Date,
                State = RequestState.Pending,
                CategoryId = payload.Category.Id,
                Url = payload.Url
            };

            _requestRepository.AddRequest(request);

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

            var status = await CheckAmountForRequest(currentUser.Id, DateTime.Now.Year, payload.Amount - request.Amount, payload.CategoryId, cancellationToken);

            if (status != "OK")
            {
                return BadRequest(status);
            }

            request.Title = payload.Title;
            request.Amount = payload.Amount;
            request.CategoryId = payload.CategoryId;
            request.Date = payload.Date;
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

        [HttpGet("pending/notifications")]
        public async Task<IActionResult> SendNotificationsForPendingRequests(CancellationToken cancellationToken)
        {
            var pendingRequests = await _requestRepository.GetRequests(
                _ => _.Year == DateTime.Now.Year && _.State == RequestState.Pending, cancellationToken);

            if (pendingRequests.Any())
            {
                var superiorsMails = pendingRequests.Select(_ => new
                {
                    _.User.Superior.Username,
                }).Distinct();

                foreach (var mail in superiorsMails)
                {
                    _mailService.SendMail("You have new requests to handle", mail.Username);
                }
            }

            return Ok();
        }

        [HttpGet("ApprovedBySuperior/notifications")]
        public async Task<IActionResult> SendNotificationsForApprovedBySuperiorRequests(CancellationToken cancellationToken)
        {
            var approvedBySuperiorRequests = await _requestRepository.GetRequests(
                _ => _.Year == DateTime.Now.Year && _.State == RequestState.ApprovedBySuperior, cancellationToken);

            if (approvedBySuperiorRequests.Any())
            {
                var admins = await _userRepository.GetAdminUsers(cancellationToken);
                var adminsMails = admins.Select(u => u.Username).ToArray();

                foreach (var mail in adminsMails)
                {
                    _mailService.SendMail("You have new requests to handle", mail);
                }
            }

            return Ok();
        }

        private async Task<RequestModel[]> GetRequests(int year, IEnumerable<RequestState> requestStates, CancellationToken cancellationToken)
        {
            Expression<Func<Request, bool>> predicate;

            var currentUser = await _userRepository.GetUser(HttpContext.User.GetId(), cancellationToken);
            if (currentUser.IsAdmin)
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

        private async Task<string> CheckAmountForRequest(int userId, int year, decimal amount, int categoryId, CancellationToken cancellationToken)
        {
            decimal currentAmount = await CalculateCurrentAmount(userId, year, cancellationToken);

            if (currentAmount < amount)
            {
                return "Amount of your request is bigger than your current amount (" + currentAmount + ")!";
            }

            var category = await _requestCategoryRepository.GetRequestCategory(categoryId, cancellationToken);

            if (category.SpendLimit != null)
            {
                decimal requestsSumForCategory = await CalculateRequestsSumForCategory(userId, year, category.Id, cancellationToken);

                var currentAmountForCategory = category.SpendLimit - requestsSumForCategory;

                if (currentAmountForCategory < amount)
                {
                    return "Amount of your request is over " + category.Title
                    + " category limit: " + category.SpendLimit +
                    ". Your current amount for this category is (" + currentAmountForCategory + ")!";
                }
            }
            return "OK";
        }

        private async Task<decimal> CalculateCurrentAmount(int userId, int year, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(userId, year, cancellationToken);
            var requests = await _requestRepository.GetRequests(year, userId, cancellationToken);
            decimal requestsSum = 0;

            foreach (var item in requests)
            {
                requestsSum += item.Amount;
            }

            var currentAmount = budget.Amount - requestsSum;

            return currentAmount;
        }

        private async Task<decimal> CalculateRequestsSumForCategory(int userId, int year, int categoryId, CancellationToken cancellationToken)
        {
            var requestsOfCategory = (await _requestRepository.GetRequests(year, userId, cancellationToken)).Where(req => req.CategoryId == categoryId);

            decimal requestsSumForCategory = 0;

            foreach (var item in requestsOfCategory)
            {
                requestsSumForCategory += item.Amount;
            }

            return requestsSumForCategory;
        }
    }
}