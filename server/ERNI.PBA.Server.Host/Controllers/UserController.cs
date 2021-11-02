using System;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Commands.Users;
using ERNI.PBA.Server.Business.Queries.Users;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Users;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Users;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Security;
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

        public UserController(
            Lazy<IGetCurrentUserQuery> getCurrentUserQuery,
            Lazy<IGetActiveUsersQuery> getActiveUsersQuery,
            Lazy<IRegisterUserCommand> registerUserCommand)
        {
            _getCurrentUserQuery = getCurrentUserQuery;
            _getActiveUsersQuery = getActiveUsersQuery;
            _registerUserCommand = registerUserCommand;
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
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand.UpdateUserModel payload,
            [FromServices] UpdateUserCommand updateUserCommand, CancellationToken cancellationToken)
        {
            await updateUserCommand.ExecuteAsync(payload, HttpContext.User, cancellationToken);

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

        [HttpPost("{id}/activate")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Activate(int id, [FromServices] SetUserStateCommand query, CancellationToken cancellationToken)
        {
            await query.ExecuteAsync((id, UserState.Active), HttpContext.User, cancellationToken);

            return Ok();
        }

        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Deactivate(int id, [FromServices] SetUserStateCommand query, CancellationToken cancellationToken)
        {
            await query.ExecuteAsync((id, UserState.Inactive), HttpContext.User, cancellationToken);

            return Ok();
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
