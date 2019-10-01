using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Extensions;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Utils;
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

        public UserController(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork,
            ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(CancellationToken cancellationToken)
        {
            var username = HttpContext.User.GetIdentifier(Claims.UserName);
            if (string.IsNullOrWhiteSpace(username))
                return Forbid();

            var user = await _userRepository.GetAsync(username);
            if (user == null)
                return Forbid();

            if (cancellationToken.IsCancellationRequested)
                return BadRequest();

            user.UniqueIdentifier = HttpContext.User.GetIdentifier(Claims.UniqueIndetifier);
            user.FirstName = HttpContext.User.GetIdentifier(Claims.FirstName);
            user.LastName = HttpContext.User.GetIdentifier(Claims.LastName);

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserModel payload)
        {
            var userExists = await _userRepository.ExistsAsync(payload.Email);
            if (userExists)
                return StatusCode(409);

            var user = new User
            {
                FirstName = payload.FirstName,
                LastName = payload.LastName,
                Username = payload.Email,
                IsAdmin = payload.IsAdmin,
                IsSuperior = payload.IsSuperior,
                IsViewer = payload.IsViewer,
                SuperiorId = payload.Superior,
                State = payload.State
            };

            await _userRepository.AddUserAsync(user);
            var budget = new Budget
            {
                UserId = user.Id,
                User = user,
                Amount = payload.Amount,
                Year = payload.Year
            };

            await _budgetRepository.AddBudgetAsync(budget);
            await _unitOfWork.SaveChanges(default(CancellationToken));

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel payload, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(payload.Id, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Not a valid id");
                return NotFound("Not a valid id");
            }

            user.IsAdmin = payload.IsAdmin;
            user.IsViewer = payload.IsViewer;
            user.IsSuperior = payload.IsSuperior;
            user.SuperiorId = payload.Superior?.Id;
            user.State = payload.State;

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        // GET api/values
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(HttpContext.User.GetId(), cancellationToken);
            if (user == null)
                return StatusCode(403);

            return Ok(new UserModel
            {
                Id = user.Id,
                IsAdmin = user.IsAdmin,
                IsSuperior = user.IsSuperior,
                IsViewer = user.IsViewer,
                FirstName = user.FirstName,
                LastName = user.LastName,
                State = user.State,
                Superior = user.Superior != null ? new SuperiorModel
                {
                    Id = user.Superior.Id,
                    FirstName = user.Superior.FirstName,
                    LastName = user.Superior.LastName,
                } : null
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userRepository.GetUser(id, CancellationToken.None);

            return Ok(new UserModel
            {
                Id = user.Id,
                IsAdmin = user.IsAdmin,
                IsSuperior = user.IsSuperior,
                IsViewer = user.IsViewer,
                FirstName = user.FirstName,
                LastName = user.LastName,
                State = user.State,
                Superior = user.Superior != null ? new SuperiorModel
                {
                    Id = user.Superior.Id,
                    FirstName = user.Superior.FirstName,
                    LastName = user.Superior.LastName,
                } : null
            });
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
                } : null
            }).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName);

            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveUsers(int year, CancellationToken cancellationToken)
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
