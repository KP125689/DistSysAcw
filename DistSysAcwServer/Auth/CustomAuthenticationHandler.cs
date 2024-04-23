using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using DistSysAcwServer.Models;
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
                return Unauthorized("ApiKey header is missing.");
            }

            var apiKey = apiKeyHeaderValues.FirstOrDefault();

            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized("API Key is empty.");
            }

            var user = await GetUserByApiKeyAsync(apiKey);
            if (user == null)
            {
                return Unauthorized("Check ApiKey in Header is correct.");
            }

            var claimsPrincipal = CreateClaimsPrincipal(user);

            var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        private async Task<User> GetUserByApiKeyAsync(string apiKey)
        {
            return await _userDatabaseAccess.GetUserByApiKey(apiKey);
        }

        private ClaimsPrincipal CreateClaimsPrincipal(User user)
        {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

            var claimsIdentity = new ClaimsIdentity(claims, "ApiKey");

            return new ClaimsPrincipal(claimsIdentity);
        }

        private AuthenticateResult Unauthorized(string message)
        {
            return AuthenticateResult.Fail($"Unauthorized: {message}");
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes("Unauthorized. Check ApiKey in Header is correct.");
            Context.Response.StatusCode = 401;
            Context.Response.ContentType = "application/json";
            await Context.Response.Body.WriteAsync(messageBytes, 0, messageBytes.Length);
            await Context.Response.Body.FlushAsync();
        }
    }
}