using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Models.ViewModels.Identity;
using UserManagement;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SharedLib.Common;
using Models.DatabaseModels.Authentication;
using System.Linq;
using System.Threading.Tasks;
using Models.DatabaseModels.DSPerson.Core;

namespace Authentication.JwtStatefulToken
{
    public class TokenService : ITokenService 
    {
        private readonly IConfiguration _configuration;
        private readonly IUserManagement userManagement;

        public TokenService(IConfiguration configuration, IUserManagement userManagement) 
        {
            _configuration = configuration;
            this.userManagement = userManagement;
        }

        public async Task<AuthenticationResult> Authenticate(string username, string password) 
        {
            VwUser _user = await this.ValidateCredentials(username, password);

            var authenticationResult = new AuthenticationResult()
            {
                IsAuthenticated = _user is not null,
                User = _user,
            };

            if (_user != null) 
            {
                Claim[] claims = new Claim[]
                {
                    new Claim("UserId", _user.UserId.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, _user.UserName),
                };

                AuthenticationTicket ticket = this.CreateAuthenticationTicket(claims);
                authenticationResult.Token = this.CreateJwtBearerToken(claims);

                ActiveTokens._tokens.TryAdd(authenticationResult.Token, new TokenInfo() { Ticket = ticket, User = _user });
            }

            return authenticationResult;
        }

        public async Task<AuthenticationResult> AuthenticateUser(string username, string password)
        {
            VwUser _user = await this.ValidateCredentials(username, password);

            var authenticationResult = new AuthenticationResult()
            {
                IsAuthenticated = _user is not null,
                User = _user,
            };

            if (_user != null)
            {
                Claim[] claims = new Claim[]
                {
                    new Claim("UserId", _user.UserId.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, _user.UserName),
                };

                AuthenticationTicket ticket = this.CreateAuthenticationTicket(claims);
                authenticationResult.Token = this.CreateJwtBearerToken(claims);

                ActiveTokens._tokens.TryAdd(authenticationResult.Token, new TokenInfo() { Ticket = ticket, User = _user });
            }

            return authenticationResult;
        }

        private async Task<VwUser> ValidateCredentials(string username, string password)
        {
            var ds = await this.userManagement.GetUserInfo(username);

            var user = ds.Tables[0].ToList<User>().FirstOrDefault();

            if (user != null)
            {
                var passwordHasher = new PasswordHasher<Models.DatabaseModels.Authentication.User>();
                var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, password);

                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    user = null;
                }
            }

            var fullName = string.Empty;

            if (user.UserTypeId == 1)
            {
                var person = ds.Tables[1].ToList<VwPerson>().FirstOrDefault();
                fullName = person.PersonName;
            }
            //else if (user.UserTypeId == 2)
            //{
            //    var business = ds.Tables[1].ToList<VwBusiness>().FirstOrDefault();
            //    fullName = business.BusinessName;
            //}

            return new VwUser() { UserId = user.UserId, UserName = username, FullName = fullName };
        }

        private AuthenticationTicket CreateAuthenticationTicket(Claim[] claims) 
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, nameof(TokenAuthenticationHandler));
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            AuthenticationTicket authTicket = new AuthenticationTicket(claimsPrincipal, TokenAuthenticationSchemeOptions.Name);

            return authTicket;
        }

        private string CreateJwtBearerToken(Claim[] claims)
        {
            var key = Encoding.ASCII.GetBytes("PDv7DrqznYL6nv7DrqzjnQYO9JxIsWdcjnQYL6nu0f");

            var jwt = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        //public AuthenticationTicket ValidateToken(string token)
        //{
        //    TokenInfo tokenInfo = null;
        //    if (ActiveTokens._tokens.TryGetValue(token, out tokenInfo))
        //    {
        //        return tokenInfo.Ticket;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public TokenInfo ValidateToken(string token)
        {
            TokenInfo tokenInfo = null;
            if (ActiveTokens._tokens.TryGetValue(token, out tokenInfo))
            {
                return tokenInfo;
            }
            else
            {
                return null;
            }
        }
    }
}