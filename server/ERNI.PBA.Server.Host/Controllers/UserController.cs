using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
{
    [Route("api/[controller]")]
    // [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET api/values
        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            var user = _userRepository.GetUser(HttpContext.User.GetId());

            return Ok(new UserModel
            {
                // FirstName = user.Claims.Single(c => c.Type == Claims.FirstName).Value,
                // LastName = user.Claims.Single(c => c.Type == Claims.LastName).Value,
                // Roles = user.FindAll(c => c.Type == Claims.Role).Select(_ => _.Value).ToArray()
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userRepository.GetUser(id);

            return Ok(new
            {
                Id = user.Id,
                IsAdmin = user.IsAdmin,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Superior = user.Superior != null ? new
                {
                    Id = user.Superior.Id,
                    FirstName = user.Superior.FirstName,
                    LastName = user.Superior.LastName,
                } : null
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetUsers(cancellationToken);

            var result = users.Select(_ => new
            {
                Id = _.Id,
                IsAdmin = _.IsAdmin,
                FirstName = _.FirstName,
                LastName = _.LastName,
                Superior = _.Superior != null ? new
                {
                    Id = _.Superior.Id,
                    FirstName = _.Superior.FirstName,
                    LastName = _.Superior.LastName,
                } : null
            });

            return Ok(result);
        }
    }
}
