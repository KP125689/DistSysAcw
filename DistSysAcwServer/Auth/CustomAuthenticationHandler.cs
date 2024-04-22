using System.Security.Claims;
using System.Text.Encodings.Web;
using DistSysAcwServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;




namespace DistSysAcwServer.Auth
{
    /// <summary>
    /// Authenticates clients by API Key
    /// </summary>
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly UserDatabaseAccess _userDatabaseAccess;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            UserDatabaseAccess userDatabaseAccess)
            //IHttpContextAccessor httpContextAccessor)
            : base(options, logger, encoder, clock)
        {
            _userDatabaseAccess = userDatabaseAccess;
          //  _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("ApiKey", out var apiKeyHeaderValues))
            {
                // ApiKey header is missing
                return AuthenticateResult.Fail("Unauthorized. ApiKey header is missing.");
            }

            var apiKey = apiKeyHeaderValues.FirstOrDefault();


            // Check if the API Key is valid
            var user = await _userDatabaseAccess.GetUserByApiKey(apiKey);
            if (user == null)
            {
                // API Key is not valid
                return AuthenticateResult.Fail("Unauthorized. Check ApiKey in Header is correct.");
            }
            if (string.IsNullOrEmpty(apiKey))
            {
                return AuthenticateResult.Fail("Unauthorized: API Key is empty.");
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

       /* protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes("Unauthorized. Check ApiKey in Header is correct.");
            Context.Response.StatusCode = 401;
            Context.Response.ContentType = "application/json";
            await Context.Response.Body.WriteAsync(messageBytes, 0, messageBytes.Length);
            await Context.Response.Body.FlushAsync();
        }*/
    }
}