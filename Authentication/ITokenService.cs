using Microsoft.AspNetCore.Authentication;
using Models.ViewModels.DSAuth.Setup;
using Models.ViewModels.Identity;
using SharedLib.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace Authentication
{
    public interface ITokenService
    {
        Task<AuthenticationResult> Authenticate(string username, string password);
        Task<AuthenticationResult> AuthenticateUser(string username, string password);
        TokenInfo ValidateToken(string token);
    }
}