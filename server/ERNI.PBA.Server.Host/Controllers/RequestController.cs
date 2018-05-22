using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Examples;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Model.PendingRequests;
using ERNI.PBA.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;

namespace server.Controllers
{
    [Route("api/[controller]")]
    // [Authorize]
    public class RequestController : Controller
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RequestController(IRequestRepository requestRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
        }

        [HttpGet("user/current/year/{year}")]
        public async Task<IActionResult> GetCurrentUsersBudget(int year, CancellationToken cancellationToken)
        {
            var requests = await _requestRepository.GetRequests(year, HttpContext.User.GetId(), cancellationToken);

            var result = requests.Select(_ => new
            {
                Title = _.Title,
                Amount = _.Amount,
                Date = _.Date
            });

            return Ok(result);
        }

        [HttpGet("user/{userId}/year/{year}")]
        public async Task<IActionResult> GetUsersBudget(int userId, int year, CancellationToken cancellationToken)
        {
            var requests = await _requestRepository.GetRequests(year, userId, cancellationToken);

            var result = requests.Select(_ => new
            {
                Title = _.Title,
                Amount = _.Amount,
                Date = _.Date
            });

            return Ok(result);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveRequest(int id, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(id, cancellationToken);

            request.State = RequestState.Approved;

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectRequest(int id, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(id, cancellationToken);

            request.State = RequestState.Rejected;

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> PutRequest([FromBody]PutRequestModel payload, CancellationToken cancellationToken)
        {
            var request = new Request
            {
                UserId = 1,
                Year = 2018,
                Title = payload.Title,
                Amount = payload.Amount,
                Date = payload.Date,
                State = RequestState.Pending
            };

            _requestRepository.AddRequest(request);

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchRequest(int id, [FromBody]PatchRequestModel payload, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetRequest(id, cancellationToken);

            request.Title = payload.Title;
            request.Amount = payload.Amount;
            request.Date = payload.Date;
            request.State = RequestState.Pending;

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpGet("pending")]
        [SwaggerResponseExample(200, typeof(PendingRequestExample))]
        public async Task<IActionResult> GetPendingRequests(CancellationToken cancellationToken)
        {
            var requests = await _requestRepository.GetPendingRequests(cancellationToken);

            var result = requests.Select(_ =>
                new RequestModel
                {
                    Id = _.Id,
                    Title = _.Title,
                    Amount = _.Amount,
                    Year = _.Year,
                    Date = _.Date,
                    User = new ERNI.PBA.Server.Host.Model.PendingRequests.UserModel
                    {
                        Id = _.UserId,
                        FirtName = _.Budget.User.FirstName,
                        LastName = _.Budget.User.LastName
                    }
                }).ToArray();

            return Ok(result);
        }
    }
}