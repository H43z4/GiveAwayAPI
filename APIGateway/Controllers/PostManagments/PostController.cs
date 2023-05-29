using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.PostManagemrnts;
using SharedLib.Common;
using System;
using UserManagement;
using System.Threading.Tasks;
using PostManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Validations.Rules;
using System.Collections.Generic;
using NSwag.Annotations;
using Models.ViewModels.DSAuth.Setup;
using Microsoft.AspNetCore.Authorization;
using Authentication.AuthSchemes;

namespace APIGateway.Controllers.PostManagments
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JWT_BEARER_TOKEN_STATELESS)]
    public class PostController : Controller
    {
        public IPostManagementService postManagement { get; }
        public VwDSUser User
        {
            get
            {
                return (VwDSUser)this.Request.HttpContext.Items["User"];
            }
        }
        public PostController(IPostManagementService PostManagement)
        {
            this.postManagement = PostManagement;
        }
        [HttpPost]
        public async Task<ApiResponse> CreatePost([FromForm] CreatePostVM createPost)
        {
            try
            {
                postManagement.VwDSUser = User;
                var ds =await this.postManagement.craetePost(createPost);

                if (ds == 1)
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, ds, Constants.DATA_SAVED_MESSAGE);
                }
                else
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.DATA_NOT_SAVED_MESSAGE);
                }
            }
            catch (Exception)
            {
                throw;
            }
        } 
        [HttpPut]
        public async Task<ApiResponse> UpdatePost([FromForm] UpdatePostVM updatePost)
        {
            try
            {
                var ds = await this.postManagement.UpdatePost(updatePost);

                if (ds == 1)
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, ds, Constants.DATA_SAVED_MESSAGE);
                }
                else
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.DATA_NOT_SAVED_MESSAGE);

                }
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
                var ds =await this.postManagement.GetAllPosts();
                if (ds.Count > 0&& ds!=null)
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, ds, Constants.RECORD_FOUND_MESSAGE);
                }
                else
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<ApiResponse> GetUserByIdWithPosts(int userId)
        {
            try
            {
                var ds =await this.postManagement.GetUserByIdWithPosts(userId);
                if ( ds!=null)
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, ds, Constants.RECORD_FOUND_MESSAGE);
                }
                else
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<ApiResponse> GetPostById(int id)
        {
            try
            {
                var ds =await this.postManagement.GetPostsById(id);
                if (ds != null)
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, ds, Constants.RECORD_FOUND_MESSAGE);
                }
                else
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<ApiResponse> SearchPostsbyTitle(string searchstr, int catogaryid=0)
        {
            try
            {
                var ds = await this.postManagement.SearchPostsbyTitle(searchstr,catogaryid);
                if (ds.Count > 0 && ds != null)
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, ds, Constants.RECORD_FOUND_MESSAGE);
                }
                else
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
                }
            }
            catch (Exception)
            {
                throw;
            }
        } 
        [HttpDelete]
        public async Task<ApiResponse> DeletePostsbyId(int Id)
        {
            try
            {
                var ds = await this.postManagement.DeletePost(Id);
                if (ds == 1)
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, ds, Constants.RECORD_FOUND_MESSAGE);
                }
                else
                {
                    return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
