using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


#region Task4
namespace DistSysAcwServer.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("New")]
        public IActionResult NewGet(string username)
        {
            // Logic for handling GET request with parameter from URI
            return Ok($"Creating user '{username}' via GET request.");
        }

        [HttpPost("New")]
        public IActionResult NewPost([FromBody] string username)
        {
            // Logic for handling POST request with JSON string parameter from the body
            return Ok($"Creating user '{username}' via POST request.");
        }
    }
}
#endregion