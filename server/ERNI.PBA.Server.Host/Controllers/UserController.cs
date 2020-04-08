using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Commands.Users;
using ERNI.PBA.Server.Domain.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(CancellationToken cancellationToken)
        {
            var registerUserCommand = new RegisterUserCommand { Principal = HttpContext.User };
            var userModel = await _mediator.Send(registerUserCommand, cancellationToken);

            return Ok(userModel);
        }

        [HttpPost("create")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserCommand payload, CancellationToken cancellationToken)
        {
            await _mediator.Send(payload, cancellationToken);

            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UpdateUser([FromBody]UpdateUserCommand payload, CancellationToken cancellationToken)
        {
            await _mediator.Send(payload, cancellationToken);

            return Ok();
        }

        // GET api/values
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            var getCurrentUserQuery = new GetCurrentUserQuery { Principal = HttpContext.User };
            var userModel = await _mediator.Send(getCurrentUserQuery, cancellationToken);

            return Ok(userModel);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Get(int id)
        {
            var getUserQuery = new GetUserQuery { UserId = id };
            var userModel = await _mediator.Send(getUserQuery);

            return Ok(userModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubordinateUsers(CancellationToken cancellationToken)
        {
            var getSubordinateUsersQuery = new GetSubordinateUsersQuery { Principal = HttpContext.User };
            var userModels = await _mediator.Send(getSubordinateUsersQuery, cancellationToken);

            return Ok(userModels);
        }

        [HttpGet("active")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetActiveUsers(CancellationToken cancellationToken)
        {
            var userModels = await _mediator.Send(new GetActiveUsersQuery(), cancellationToken);

            return Ok(userModels);
        }
    }
}
