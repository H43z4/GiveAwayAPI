﻿using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Authentication.JwtStatelessToken
{
    public class TokenAuthenticationHandler : AuthenticationHandler<TokenAuthenticationSchemeOptions>
    {
        private ITokenService _tokenService;
        private const string AUTHORIZATION_TOKEN_HEADER = "AuthToken";

        public TokenAuthenticationHandler(IOptionsMonitor<TokenAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ITokenService tokenService) : base(options, logger, encoder, clock)
        {
            _tokenService = tokenService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authToken = this.Request.Headers[AUTHORIZATION_TOKEN_HEADER];
            if (string.IsNullOrEmpty(authToken))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization header not found"));
            }

            var tokenInfo = _tokenService.ValidateToken(authToken);

            if (tokenInfo == null)
            {
                return Task.FromResult(AuthenticateResult.Fail(""));
            }

            this.Request.HttpContext.Items["User"] = tokenInfo.DSUser;

            return Task.FromResult(AuthenticateResult.Success(tokenInfo.Ticket));
        }
    }
}
