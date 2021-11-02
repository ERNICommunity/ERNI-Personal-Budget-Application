using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Commands.Users;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Users;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Users;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Payloads;
using ERNI.PBA.Server.Domain.Security;
using ERNI.PBA.Server.Business.Queries.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly Lazy<IGetCurrentUserQuery> _getCurrentUserQuery;
        private readonly Lazy<IGetActiveUsersQuery> _getActiveUsersQuery;
        private readonly Lazy<IRegisterUserCommand> _registerUserCommand;
        private readonly Lazy<IUpdateUserCommand> _updateUserCommand;

        public UserController(
            Lazy<IGetCurrentUserQuery> getCurrentUserQuery,
            Lazy<IGetActiveUsersQuery> getActiveUsersQuery,
            Lazy<IRegisterUserCommand> registerUserCommand,
            Lazy<IUpdateUserCommand> updateUserCommand)
        {
            _getCurrentUserQuery = getCurrentUserQuery;
            _getActiveUsersQuery = getActiveUsersQuery;
            _registerUserCommand = registerUserCommand;
            _updateUserCommand = updateUserCommand;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(CancellationToken cancellationToken)
        {
            var userModel = await _registerUserCommand.Value.ExecuteAsync(new RegisterUserModel(), HttpContext.User, cancellationToken);

            return Ok(userModel);
        }

        [HttpPost("create")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand.CreateUserModel payload,
            [FromServices] CreateUserCommand createUserCommand, CancellationToken cancellationToken)
        {
            await createUserCommand.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel payload, CancellationToken cancellationToken)
        {
            await _updateUserCommand.Value.ExecuteAsync(payload, HttpContext.User, cancellationToken);

            return Ok();
        }

        // GET api/values
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            var userModel = await _getCurrentUserQuery.Value.ExecuteAsync(HttpContext.User, cancellationToken);

            return Ok(userModel);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Get(int id, [FromServices] GetUserQuery query, CancellationToken cancellationToken)
        {
            var userModel = await query.ExecuteAsync(id, HttpContext.User, cancellationToken);

            return Ok(userModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubordinateUsers([FromServices] GetSubordinateUsersQuery query, CancellationToken cancellationToken)
        {
            var userModels = await query.ExecuteAsync(HttpContext.User, cancellationToken);

            return Ok(userModels);
        }

        [HttpGet("active")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetActiveUsers(CancellationToken cancellationToken)
        {
            var userModels = await _getActiveUsersQuery.Value.ExecuteAsync(HttpContext.User, cancellationToken);

            return Ok(userModels);
        }
    }
}
