using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel payload, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(payload.Id, cancellationToken);

            if (user == null)
            {
                return BadRequest("Not a valid id");
            }

            user.IsAdmin = payload.IsAdmin;
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

            return Ok(new UserModel
            {
                Id = user.Id,
                IsAdmin = user.IsAdmin,
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
                FirstName = _.FirstName,
                LastName = _.LastName,
                State = _.State,
                Superior = _.Superior != null ? new SuperiorModel
                {
                    Id = _.Superior.Id,
                    FirstName = _.Superior.FirstName,
                    LastName = _.Superior.LastName,
                } : null
            });

            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveUsers(int year, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllUsers(_=>_.State == UserState.Active, cancellationToken);

            var result = users.Select(_ => new UserModel
            {
                Id = _.Id,
                FirstName = _.FirstName,
                LastName = _.LastName,
            });

            return Ok(result);
        }
    }
}
