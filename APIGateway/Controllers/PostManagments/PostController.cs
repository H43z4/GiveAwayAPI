using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.PostManagemrnts;
using SharedLib.Common;
using System;
using UserManagement;
using System.Threading.Tasks;
using PostManagement;
using Models.ViewModels.DSAuth.Setup;
using Microsoft.AspNetCore.Authorization;
using Authentication.AuthSchemes;

namespace APIGateway.Controllers.PostManagments
{

    [Route("[controller]/[action]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = AuthenticationSchemes.JWT_BEARER_TOKEN_STATELESS)]
    public class PostController : Controller
    {
        private readonly IPostManagementService _PostManagementService;
        public VwDSUser User
        {
            get
            {
                return (VwDSUser)this.Request.HttpContext.Items["User"];
            }
        }
        public PostController(IPostManagementService postManagementService)
        {
            this._PostManagementService = postManagementService;
        }
        [HttpPost]
        public async Task<ApiResponse> CreatePost(CreatePostVM createPost)
        {
            try
            {
                //listOfValuesService.VwDSUser = User;
                //DataSet resultData = await listOfValuesService.GetCategoryLOV();
                var lst = _PostManagementService.craetePost(createPost);// resultData.Tables[0].ToList<VwLOVs>();

                var apiResponseType = lst != null ? ApiResponseType.SUCCESS : ApiResponseType.NOT_FOUND;
                var msg = lst != null ? Constants.RECORD_FOUND_MESSAGE : Constants.NOT_FOUND_MESSAGE;

                return ApiResponse.GetApiResponse(apiResponseType, lst, msg);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<ApiResponse> GetPosts()
        {
            try
            {
                //listOfValuesService.VwDSUser = User;
                //DataSet resultData = await listOfValuesService.GetCategoryLOV();
                var lst = _PostManagementService.GetAllPosts();//1;// resultData.Tables[0].ToList<VwLOVs>();

                var apiResponseType = lst != null ? ApiResponseType.SUCCESS : ApiResponseType.NOT_FOUND;
                var msg = lst != null ? Constants.RECORD_FOUND_MESSAGE : Constants.NOT_FOUND_MESSAGE;

                return ApiResponse.GetApiResponse(apiResponseType, lst, msg);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<ApiResponse> GetPostById()
        {
            try
            {
                //listOfValuesService.VwDSUser = User;
                //DataSet resultData = await listOfValuesService.GetCategoryLOV();
                var lst = 1;// resultData.Tables[0].ToList<VwLOVs>();

                var apiResponseType = lst != null ? ApiResponseType.SUCCESS : ApiResponseType.NOT_FOUND;
                var msg = lst != null ? Constants.RECORD_FOUND_MESSAGE : Constants.NOT_FOUND_MESSAGE;

                return ApiResponse.GetApiResponse(apiResponseType, lst, msg);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
