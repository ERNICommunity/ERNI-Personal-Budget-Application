using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Commands.Users;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Queries.Users;
using ERNI.PBA.Server.Host.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public UserController(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork,
            ILogger<UserController> logger,
            IMediator mediator)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
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
            User[] users;

            var user = await _userRepository.GetUser(HttpContext.User.GetId(), cancellationToken);

            if (user.IsAdmin)
            {
                users = await _userRepository.GetAllUsers(cancellationToken);
            }
            else
            {
                users = await _userRepository.GetSubordinateUsers(user.Id, cancellationToken);
            }

            var result = users.Select(_ => new UserModel
            {
                Id = _.Id,
                IsAdmin = _.IsAdmin,
                IsSuperior = _.IsSuperior,
                IsViewer = _.IsViewer,
                FirstName = _.FirstName,
                LastName = _.LastName,
                State = _.State,
                Superior = _.Superior != null ? new SuperiorModel
                {
                    Id = _.Superior.Id,
                    FirstName = _.Superior.FirstName,
                    LastName = _.Superior.LastName,
                }
                :
                null
            }).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName);

            return Ok(result);
        }

        [HttpGet("active")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetActiveUsers(CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllUsers(_ => _.State == UserState.Active, cancellationToken);

            var result = users.Select(_ => new UserModel
            {
                Id = _.Id,
                FirstName = _.FirstName,
                LastName = _.LastName,
            }).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName);

            return Ok(result);
        }
    }
}
