using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DistSysAcwServer.Controllers
{
    [Route("api/[Controller]/[Action]")]
    public class TalkbackController : BaseController
    {
        /// <summary>
        /// Constructs a TalkBack controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public TalkbackController(Models.UserContext dbcontext) : base(dbcontext) 
        { 
        }


        #region TASK1
        //    TODO: add api/talkback/hello response
        
        public IActionResult hello()
        {
            string greeting = "Hello world";
            return Ok(greeting);
        }
        #endregion

        #region TASK1
        //    TODO:
        //       add a parameter to get integers from the URI query
        //       sort the integers into ascending order
        //       send the integers back as the api/talkback/sort response
        //       conform to the error handling requirements in the spec

        public IActionResult Sort([FromQuery] int[] numbers) //https://localhost:44394/api/talkback/sort?sortValue=9&sortValue=3&sortValue=4 to test
        {
            try
            {
                if (numbers == null || numbers.Length == 0)
                {
                    return BadRequest("Please provide a value to sort.");
                }

                Array.Sort(numbers);
                return Ok(numbers);

            }
            catch (Exception ex)
            {
                // Handle any errors, such as invalid input
                return BadRequest($"Error: {ex.Message}");
            }
        }

        #endregion
    }
}
