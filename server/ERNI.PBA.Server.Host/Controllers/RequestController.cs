using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Requests;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Requests;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Payloads;
using ERNI.PBA.Server.Domain.Models.Responses;
using ERNI.PBA.Server.Domain.Models.Responses.PendingRequests;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class RequestController : Controller
    {
        private readonly Lazy<IGetRequestQuery> _getRequestQuery;
        private readonly Lazy<IGetRequestsQuery> _getRequestsQuery;
        private readonly Lazy<IGetBudgetLeftQuery> _getBudgetLeftQuery;
        private readonly Lazy<IAddTeamRequestCommand> _addTeamRequestCommand;
        private readonly Lazy<IApproveRequestCommand> _approveRequestCommand;
        private readonly Lazy<IRejectRequestCommand> _rejectRequestCommand;
        private readonly Lazy<IAddRequestCommand> _addRequestCommand;
        private readonly Lazy<IAddMassRequestCommand> _addMassRequestCommand;
        private readonly Lazy<IUpdateRequestCommand> _updateRequestCommand;
        private readonly Lazy<IUpdateTeamRequestCommand> _updateTeamRequestCommand;
        private readonly Lazy<IDeleteRequestCommand> _deleteRequestCommand;

        public RequestController(
            Lazy<IGetRequestQuery> getRequestQuery,
            Lazy<IGetRequestsQuery> getRequestsQuery,
            Lazy<IGetBudgetLeftQuery> getBudgetLeftQuery,
            Lazy<IAddTeamRequestCommand> addTeamRequestCommand,
            Lazy<IApproveRequestCommand> approveRequestCommand,
            Lazy<IRejectRequestCommand> rejectRequestCommand,
            Lazy<IAddRequestCommand> addRequestCommand,
            Lazy<IAddMassRequestCommand> addMassRequestCommand,
            Lazy<IUpdateRequestCommand> updateRequestCommand,
            Lazy<IUpdateTeamRequestCommand> updateTeamRequestCommand,
            Lazy<IDeleteRequestCommand> deleteRequestCommand)
        {
            _getRequestQuery = getRequestQuery;
            _getRequestsQuery = getRequestsQuery;
            _getBudgetLeftQuery = getBudgetLeftQuery;
            _addTeamRequestCommand = addTeamRequestCommand;
            _approveRequestCommand = approveRequestCommand;
            _rejectRequestCommand = rejectRequestCommand;
            _addRequestCommand = addRequestCommand;
            _addMassRequestCommand = addMassRequestCommand;
            _updateRequestCommand = updateRequestCommand;
            _updateTeamRequestCommand = updateTeamRequestCommand;
            _deleteRequestCommand = deleteRequestCommand;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequest(int id, CancellationToken cancellationToken)
        {
            var request = await _getRequestQuery.Value.ExecuteAsync(id, HttpContext.User, cancellationToken);

            return Ok(request);
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ApproveRequest(int id, CancellationToken cancellationToken)
        {
            await _approveRequestCommand.Value.ExecuteAsync(id, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RejectRequest(int id, CancellationToken cancellationToken)
        {
            await _rejectRequestCommand.Value.ExecuteAsync(id, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddRequest([FromBody]PostRequestModel payload, CancellationToken cancellationToken)
        {
            await _addRequestCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPost("team")]
        public async Task<IActionResult> AddTeamRequest([FromBody] PostRequestModel payload, CancellationToken cancellationToken)
        {
            await _addTeamRequestCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Creates one request for each user added to mass request with enough budget left. Created requests are in Approved state.
        /// </summary>
        [HttpPost("mass")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AddRequestMass([FromBody] RequestMassModel payload, CancellationToken cancellationToken)
        {
            await _addMassRequestCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpGet("{year}/pending")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Finance)]
        public async Task<RequestModel[]> GetPendingRequests(int year, CancellationToken cancellationToken)
        {
            var getRequestsModel = new GetRequestsModel
            {
                Year = year,
                RequestStates = new[] { RequestState.Pending }
            };

            return await _getRequestsQuery.Value.ExecuteAsync(getRequestsModel, HttpContext.User, cancellationToken);
        }

        [HttpGet("{year}/approved")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Finance)]
        public async Task<RequestModel[]> GetApprovedRequests(int year, CancellationToken cancellationToken)
        {
            var getRequestsModel = new GetRequestsModel
            {
                Year = year,
                RequestStates = new[] { RequestState.Approved }
            };

            return await _getRequestsQuery.Value.ExecuteAsync(getRequestsModel, HttpContext.User, cancellationToken);
        }

        [HttpGet("{year}/approvedBySuperior")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Finance)]
        public async Task<RequestModel[]> GetApprovedBySuperiorRequests(int year, CancellationToken cancellationToken)
        {
            var getRequestsModel = new GetRequestsModel
            {
                Year = year,
                RequestStates = new[] { RequestState.ApprovedBySuperior }
            };

            return await _getRequestsQuery.Value.ExecuteAsync(getRequestsModel, HttpContext.User, cancellationToken);
        }

        [HttpGet("{year}/rejected")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Finance)]
        public async Task<RequestModel[]> GetRejectedRequests(int year, CancellationToken cancellationToken)
        {
            var getRequestsModel = new GetRequestsModel
            {
                Year = year,
                RequestStates = new[] { RequestState.Rejected }
            };

            return await _getRequestsQuery.Value.ExecuteAsync(getRequestsModel, HttpContext.User, cancellationToken);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRequest([FromBody] UpdateRequestModel payload, CancellationToken cancellationToken)
        {
            await _updateRequestCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPut("team")]
        public async Task<IActionResult> UpdateTeamRequest([FromBody] UpdateRequestModel payload, CancellationToken cancellationToken)
        {
            await _updateTeamRequestCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id, CancellationToken cancellationToken)
        {
            await _deleteRequestCommand.Value.ExecuteAsync(id, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpGet("budget-left/{amount}/{year}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<UserModel[]> BudgetLeft(BudgetLeftModel payload, CancellationToken cancellationToken)
        {
            return await _getBudgetLeftQuery.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);
        }
    }
}