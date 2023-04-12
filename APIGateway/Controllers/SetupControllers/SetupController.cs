using Authentication.AuthSchemes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DatabaseModels.Authentication;
using Models.ViewModels.DSAuth.Setup;
using Person;
using SharedLib.Common;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UserManagement;

namespace APIGateway.Controllers.SetupControllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JWT_BEARER_TOKEN_STATELESS)]
    public class SetupController : ControllerBase
    {
        private readonly IUserManagement userManagementService;

        public VwDSUser User
        {
            get
            {
                return (VwDSUser)this.Request.HttpContext.Items["User"];
            }
        }
        public SetupController(IUserManagement userManagementService)
        {
            this.userManagementService = userManagementService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResponse> CreateUser(VwDSAUser userObj)
        {
            this.userManagementService.VwDSUser = this.User;

            var passwordHasher = new PasswordHasher<User>();
            userObj.Password = passwordHasher.HashPassword(new Models.DatabaseModels.Authentication.User(), userObj.Password);

            var ds = await this.userManagementService.CreateUser(userObj);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows[0][0].ToString() != "3")
            {
                return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, null, Constants.DATA_SAVED_MESSAGE);
            }
            else
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.DATA_NOT_SAVED_MESSAGE + ds.Tables[0].Rows[0][1].ToString());
            }
        }
    }
}
