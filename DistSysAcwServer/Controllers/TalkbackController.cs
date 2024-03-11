using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DistSysAcwServer.Controllers
{
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
        [HttpGet("hello")]
        public IActionResult Hello()
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
        [HttpGet("Sort")]
        public IActionResult Sort([FromQuery] string sortValue)
        {
            if (string.IsNullOrEmpty(sortValue))
            {
                return BadRequest("Please provide a value to sort.");
            }

            // Split the input string by comma or other separators
            var stringValues = sortValue.Split(',');

            // List to store integers
            List<int> integers = new List<int>();

            // Loop through strings and convert to integers
            foreach (var value in stringValues)
            {
                int intValue;
                if (int.TryParse(value, out intValue))
                {
                    integers.Add(intValue);
                }
                else
                {
                    // Handle invalid integer case (log error or return bad request)
                    return BadRequest("Invalid integer value found.");
                }
            }

            // Sort the list of integers
            integers.Sort();

            // Return the sorted integers (convert back to string if needed)
            return Ok(string.Join(",", integers));
        }
        #endregion
    }
}
