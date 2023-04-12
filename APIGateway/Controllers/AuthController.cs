using Microsoft.AspNetCore.Mvc;
using UserManagement;
using Models.ViewModels.Identity;
using Authentication;
using System.Linq;
using SharedLib.Common;
using System.Threading.Tasks;
using Models.ViewModels.DSAuth.Setup;

namespace APIGateway.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService tokenService;
        private readonly IUserManagement userManagement;

        public AuthController(ITokenService tokenService, IUserManagement userManagement)
        {
            this.tokenService = tokenService;
            this.userManagement = userManagement;
        }

        [HttpGet]
        public JsonResult Pingme()
        {
            return new JsonResult("Hi there!");
        }

        //[HttpPost]
        //public async Task<ApiResponse> LoginMVRS(VwUser model)
        //{
        //    var authenticationResult = await this.tokenService.Authenticate(model.UserName, model.Password);

        //    var apiResponseType = authenticationResult.IsAuthenticated ? ApiResponseType.SUCCESS : ApiResponseType.FAILED;
        //    var msg = authenticationResult.IsAuthenticated ? Constants.AUTHORIZED_MESSAGE : Constants.UN_AUTHORIZED_MESSAGE;

        //    if (authenticationResult.IsAuthenticated)
        //    {
        //        var data = new
        //        {
        //            token = authenticationResult.Token,
        //            authenticationResult.User.UserName,
        //            authenticationResult.User.FullName,
        //            roles = string.Join(" | ", authenticationResult.User.UserRoles.Select(x => x.RoleName))
        //        };

        //        return ApiResponse.GetApiResponse(apiResponseType, data, msg);
        //    }

        //    return ApiResponse.GetApiResponse(apiResponseType, null, msg);
        //}

        [HttpPost]
        public async Task<ApiResponse> Login(VwDSAUser model)
        {
            var authenticationResult = await this.tokenService.AuthenticateUser(model.UserName, model.Password);

            var apiResponseType = authenticationResult.IsAuthenticated ? ApiResponseType.SUCCESS : ApiResponseType.FAILED;
            var msg = authenticationResult.IsAuthenticated ? Constants.AUTHORIZED_MESSAGE : Constants.UN_AUTHORIZED_MESSAGE;

            if (authenticationResult.IsAuthenticated)
            {
                var data = new
                {
                    token = authenticationResult.Token,
                    authenticationResult.DSUser.UserName,
                    authenticationResult.DSUser.FullName,
                    authenticationResult.DSUser.OrganizationName,
                    //roles = string.Join(" | ", authenticationResult.DSUser.UserRoles.Select(x => x.RoleName))
                };

                return ApiResponse.GetApiResponse(apiResponseType, data, msg);
            }

            return ApiResponse.GetApiResponse(apiResponseType, null, msg);
        }

        //public IActionResult Login(VwUser model)
        //{
        //    string token;
        //    Models.ViewModels.Identity.VwUser user;

        //    if (this.tokenService.Authenticate(model.UserName, model.Password, out token, out user))
        //    {
        //        return new JsonResult(new ApiResponse()
        //        {
        //            status = ApiResponseType.SUCCESS,
        //            message = "Authorized.",
        //            data = new
        //            {
        //                token = token,
        //                user.UserName,
        //                user.FullName,
        //                roles = string.Join(" | ", user.UserRoles.Select(x => x.RoleName))
        //            } 
        //        });
        //    }
        //    else
        //    {
        //        return new JsonResult(new ApiResponse() { status = ApiResponseType.FAILED, message = "Unauthorized." });
        //    }
        //}

        //[HttpPost]
        //public IActionResult Register(Register model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return new JsonResult(new ApiResponse() 
        //        {
        //            status = ApiResponseType.FAILED,
        //            message = "Validation errors!",
        //            data = new 
        //            {
        //                //ModelState.
        //            }
        //        });
        //    }

        //    var user = new Models.DatabaseModels.Authentication.User()
        //    {
        //        Address = model.Address,
        //        CNIC = model.CNIC,
        //        Email = model.Email,
        //        FullName = model.FullName,
        //        Password = model.Password,
        //        PhoneNumber = model.PhoneNumber,
        //        UserName = model.UserName
        //    };

        //    var result = this.userManagement.SaveUser(user);

        //    if (result.Status)
        //    {
        //        return Ok();
        //    }
        //    else
        //    {
        //        return new JsonResult(false);
        //    }
        //}
    }
}
