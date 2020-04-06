using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Commands.Requests;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Model.PendingRequests;
using ERNI.PBA.Server.Host.Queries.Requests;
using ERNI.PBA.Server.Host.Services;
using ERNI.PBA.Server.Host.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserModel = ERNI.PBA.Server.Host.Model.UserModel;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class RequestController : Controller
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public RequestController(
            IRequestRepository requestRepository,
            IUserRepository userRepository,
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _budgetRepository = budgetRepository;
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequest(int id, CancellationToken cancellationToken)
        {
            var requestQuery = new GetRequestQuery
            {
                Principal = HttpContext.User,
                RequestId = id
            };

            var request = await _mediator.Send(requestQuery, cancellationToken);

            return Ok(request);
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ApproveRequest(int id, CancellationToken cancellationToken)
        {
            var approveRequestCommand = new ApproveRequestCommand { RequestId = id };
            await _mediator.Send(approveRequestCommand, cancellationToken);

            return Ok();
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RejectRequest(int id, CancellationToken cancellationToken)
        {
            var rejectRequestCommand = new RejectRequestCommand { RequestId = id };
            await _mediator.Send(rejectRequestCommand, cancellationToken);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddRequest([FromBody]PostRequestModel payload, CancellationToken cancellationToken)
        {
            var addRequestCommand = new AddRequestCommand
            {
                BudgetId = payload.BudgetId,
                UserId = User.GetId(),
                Title = payload.Title,
                Amount = payload.Amount,
                CurrentYear = DateTime.Now.Year,
                Date = payload.Date
            };

            await _mediator.Send(addRequestCommand, cancellationToken);

            return Ok();
        }

        [HttpPost("team")]
        public async Task<IActionResult> AddTeamRequest([FromBody] PostRequestModel payload, CancellationToken cancellationToken)
        {
            var addTeamRequestCommand = new AddTeamRequestCommand
            {
                BudgetId = payload.BudgetId,
                UserId = User.GetId(),
                Title = payload.Title,
                Amount = payload.Amount,
                CurrentYear = DateTime.Now.Year,
                Date = payload.Date
            };

            await _mediator.Send(addTeamRequestCommand, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Creates one request for each user added to mass request with enough budget left. Created requests are in Approved state.
        /// </summary>
        [HttpPost("mass")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AddRequestMass([FromBody] RequestMassModel payload, CancellationToken cancellationToken)
        {
            var addMassRequestCommand = new AddMassRequestCommand
            {
                UserId = HttpContext.User.GetId(),
                Title = payload.Title,
                Amount = payload.Amount,
                State = payload.State,
                CurrentYear = DateTime.Now.Year,
                Date = payload.Date,
                Users = payload.Users
            };

            await _mediator.Send(addMassRequestCommand, cancellationToken);

            return Ok();
        }

        [HttpGet("{year}/pending")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<RequestModel[]> GetPendingRequests(int year, CancellationToken cancellationToken)
        {
            var getRequestsQuery = new GetRequestsQuery
            {
                UserId = HttpContext.User.GetId(),
                Year = year,
                RequestStates = new[] { RequestState.Pending }
            };

            return await _mediator.Send(getRequestsQuery, cancellationToken);
        }

        [HttpGet("{year}/approved")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<RequestModel[]> GetApprovedRequests(int year, CancellationToken cancellationToken)
        {
            var getRequestsQuery = new GetRequestsQuery
            {
                UserId = HttpContext.User.GetId(),
                Year = year,
                RequestStates = new[] { RequestState.Approved }
            };

            return await _mediator.Send(getRequestsQuery, cancellationToken);
        }

        [HttpGet("{year}/approvedBySuperior")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<RequestModel[]> GetApprovedBySuperiorRequests(int year, CancellationToken cancellationToken)
        {
            var getRequestsQuery = new GetRequestsQuery
            {
                UserId = HttpContext.User.GetId(),
                Year = year,
                RequestStates = new[] { RequestState.ApprovedBySuperior }
            };

            return await _mediator.Send(getRequestsQuery, cancellationToken);
        }

        [HttpGet("{year}/rejected")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Viewer)]
        public async Task<RequestModel[]> GetRejectedRequests(int year, CancellationToken cancellationToken)
        {
            var getRequestsQuery = new GetRequestsQuery
            {
                UserId = HttpContext.User.GetId(),
                Year = year,
                RequestStates = new[] { RequestState.Rejected }
            };

            return await _mediator.Send(getRequestsQuery, cancellationToken);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRequest([FromBody] UpdateRequestModel payload, CancellationToken cancellationToken)
        {
            var updateRequestCommand = new UpdateRequestCommand
            {
                UserId = HttpContext.User.GetId(),
                RequestId = payload.Id,
                Title = payload.Title,
                Amount = payload.Amount,
                Date = payload.Date
            };

            await _mediator.Send(updateRequestCommand, cancellationToken);

            return Ok();
        }

        [HttpPut("team")]
        public async Task<IActionResult> UpdateTeamRequest([FromBody] UpdateRequestModel payload, CancellationToken cancellationToken)
        {
            var updateTeamRequestCommand = new UpdateTeamRequestCommand
            {
                UserId = HttpContext.User.GetId(),
                RequestId = payload.Id,
                Title = payload.Title,
                Amount = payload.Amount,
                Date = payload.Date
            };

            await _mediator.Send(updateTeamRequestCommand, cancellationToken);

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

        [HttpGet("budget-left/{amount}/{year}")]
        [Authorize(Roles = Roles.Admin)]
#pragma warning disable SA1202 // Elements should be ordered by access
        public async Task<UserModel[]> BudgetLeft(decimal amount, int year, CancellationToken cancellationToken)
#pragma warning restore SA1202 // Elements should be ordered by access
        {
            var budgetAmount = (await _budgetRepository.GetTotalAmountsByYear(year, cancellationToken))
                .ToDictionary(_ => _.BudgetId, _ => _.Amount);

            var budgets = await _budgetRepository.GetBudgets(year, BudgetTypeEnum.PersonalBudget, cancellationToken);

            var users = (await _userRepository.GetAllUsers(cancellationToken))
                .ToDictionary(_ => _.Id);

            var usersWithBudgetLeft = new List<UserModel>();

            foreach (var budget in budgets)
            {
                if (budgetAmount[budget.Id] + amount <= budget.Amount)
                {
                    var user = users[budget.UserId];

                    usersWithBudgetLeft.Add(new UserModel
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
                        }
                        :
                        null
                    });
                }
            }

            return usersWithBudgetLeft.ToArray();
        }
    }
}