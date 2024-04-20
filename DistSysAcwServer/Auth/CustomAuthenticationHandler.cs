using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DistSysAcwServer.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace DistSysAcwServer.Auth
{
    /// <summary>
    /// Authenticates clients by API Key
    /// </summary>
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>, IAuthenticationHandler
    {
        private Models.UserContext DbContext { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }

        private readonly UserDatabaseAccess _userDatabaseAccess;


        public CustomAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                                           ILoggerFactory logger,
                                           UrlEncoder encoder,
                                           ISystemClock clock,
                                           UserDatabaseAccess userDatabaseAccess)
            : base(options, logger, encoder, clock)
        {
            _userDatabaseAccess = userDatabaseAccess;
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("ApiKey", out var apiKeyHeaderValues))
            {
                // ApiKey header is missing
                return AuthenticateResult.Fail("Unauthorized. ApiKey header is missing.");
            }

            string apiKey = apiKeyHeaderValues.FirstOrDefault();

            // Check if the API Key is valid
            var user = await _userDatabaseAccess.GetUserByApiKey(apiKey);
            if (user == null)
            {
                // API Key is not valid
                return AuthenticateResult.Fail("Unauthorized. Check ApiKey in Header is correct.");
            }

            // API Key is valid, create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "ApiKey");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        /* 

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            byte[] messagebytes = Encoding.ASCII.GetBytes("Task 5 Incomplete");
            Context.Response.StatusCode = 401;
            Context.Response.ContentType = "application/json";
            await Context.Response.Body.WriteAsync(messagebytes, 0, messagebytes.Length);
            await HttpContextAccessor.HttpContext.Response.CompleteAsync();
        }*/
    }
}