﻿using System.Collections.Generic;
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
        public TalkbackController(Models.UserContext dbcontext) : base(dbcontext) { }


        #region TASK1
        //    TODO: add api/talkback/hello response
        [HttpGet("Hello")]
        public IActionResult Hello()
        {
            string greet = "Hello World!";
            return Ok(greet);   
        }
        #endregion

        #region TASK1
        //    TODO:
        //       add a parameter to get integers from the URI query
        //       sort the integers into ascending order
        //       send the integers back as the api/talkback/sort response
        //       conform to the error handling requirements in the spec
        [HttpGet("Sort")]
        public IActionResult NumberSorting([FromQuery] int[] integers) 
        {
            try
            {
                //array can't be empty 
                if (integers ==  null || integers.Length == 0)
                {
                    return BadRequest("please enter a valid input ");
                }
                
                //sort array in ascending order
                Array.Sort(integers);
                return Ok(integers);
            }

            catch (Exception ex)
            {
                // error handling
                return BadRequest("Error: " + ex.Message);
            }
        }
        #endregion
    }
}
