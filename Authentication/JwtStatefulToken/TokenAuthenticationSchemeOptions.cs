using Authentication.AuthSchemes;
using Microsoft.AspNetCore.Authentication;

namespace Authentication.JwtStatefulToken
{
    public class TokenAuthenticationSchemeOptions : AuthenticationSchemeOptions 
    {
        public const string Name = AuthenticationSchemes.JWT_BEARER_TOKEN;
    }
}