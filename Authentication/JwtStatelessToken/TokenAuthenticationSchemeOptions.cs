using Authentication.AuthSchemes;
using Microsoft.AspNetCore.Authentication;

namespace Authentication.JwtStatelessToken
{
    public class TokenAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public const string Name = AuthenticationSchemes.JWT_BEARER_TOKEN_STATELESS;
    }
}
