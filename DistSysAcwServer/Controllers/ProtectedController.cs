using Microsoft.AspNetCore.Mvc;
using DistSysAcwServer.Controllers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

#region Task9
namespace DistSysAcwServer.Controllers
{

    [Authorize(Roles = "Admin, User")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProtectedController : ControllerBase
    {
        // GET: api/protected/hello
        [HttpGet("hello")]
        public IActionResult Hello()
        {
            return Ok("Hello from the protected endpoint!");
        }

        // GET: api/protected/sha1/{message}
        [HttpGet("sha1/{message}")]
        public IActionResult sha1(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Message cannot be empty");
            }

            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(message));
                string hexHash = BitConverter.ToString(hashBytes).Replace("-", "");
                return Ok(hexHash);
            }
        }

        // GET: api/protected/sha256/{message}
        [HttpGet("sha256/{message}")]
        public IActionResult sha256(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Message cannot be empty");
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
                string hexHash = BitConverter.ToString(hashBytes).Replace("-", "");
                return Ok(hexHash);
            }
        }
    }
}
#endregion Task9s


