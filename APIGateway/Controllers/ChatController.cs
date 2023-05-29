using ChatManagement;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.ReviewManegment;
using ReviewAndRating;
using System.Threading.Tasks;
using System;
using SharedLib.Common;
using Models.ViewModels.ChatManegement;
using Microsoft.AspNetCore.Authorization;
using Authentication.AuthSchemes;
using Models.ViewModels.DSAuth.Setup;

namespace APIGateway.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JWT_BEARER_TOKEN_STATELESS)]

    public class ChatController : Controller
    {        
        public IChatServise _ChatServise { get; }
        public VwDSUser User
        {
            get
            {
                return (VwDSUser)this.Request.HttpContext.Items["User"];
            }
        }
        public ChatController(IChatServise chat)
        {
            this._ChatServise = chat;
        }
        [HttpPost]
        public async Task<ApiResponse> PostMassege([FromForm] SendMessegeVM SendMessege)
        {
            try
            {
                var ds = await _ChatServise.PostMassege(SendMessege);
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
        [HttpPost]
        public async Task<ApiResponse> Chatbox([FromForm] SendMessegeVM SendMessege)
        {
            try
            {
                _ChatServise.VwDSUser = User;
                var ds = await _ChatServise.Chatbox(SendMessege);
                if (ds != null)
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
        public async Task<ApiResponse> ChatsWithUserIdOnAllPost()
        {
            try
            {
                _ChatServise.VwDSUser = User;
                var ds = await _ChatServise.ChatsWithUserID();
                if (ds.Count > 0)
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
        public async Task<ApiResponse> ChatsWithUserIdOnPost(int userId,int postId)
        {
            try
            {
                var ds = await _ChatServise.ChatsWithUserIdOnPost(userId, postId);
                if (ds != null)
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
    }
}
