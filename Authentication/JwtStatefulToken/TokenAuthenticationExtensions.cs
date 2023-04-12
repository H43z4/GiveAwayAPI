using Authentication.AuthSchemes;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.JwtStatefulToken
{
    public static class TokenAuthenticationExtensions 
    {
        public static void AddTokenAuthentication(this IServiceCollection services) 
        {
            services.AddAuthentication(options => {
                options.DefaultScheme = AuthenticationSchemes.JWT_BEARER_TOKEN;
            })
            .AddScheme<TokenAuthenticationSchemeOptions, TokenAuthenticationHandler>(
                TokenAuthenticationSchemeOptions.Name, option => {}
            );
        }
    }
}