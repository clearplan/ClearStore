using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Kiota.Abstractions.Authentication;

namespace ClearStore.Graph
{
    public class TokenProvider : IAccessTokenProvider
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string[] scopes;

        public TokenProvider(ITokenAcquisition tokenAcquisition, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            scopes = configuration["MicrosoftGraph:Scopes"]!.Split(' ');
        }

        public AllowedHostsValidator AllowedHostsValidator { get; } = new AllowedHostsValidator();

        public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var token = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                return token;
            }
            catch (MsalUiRequiredException ex)
            {
                throw new MicrosoftIdentityWebChallengeUserException(ex, scopes);
            }
        }
    }
}
