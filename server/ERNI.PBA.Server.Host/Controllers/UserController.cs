using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;

        // GET api/values
        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            var user = HttpContext.User;

            return Ok(new UserModel {
                FirstName = user.Claims.Single(c => c.Type == Claims.FirstName).Value,
                LastName = user.Claims.Single(c => c.Type == Claims.LastName).Value,
                Roles = user.FindAll(c => c.Type == Claims.Role).Select(_ => _.Value).ToArray()
            });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
