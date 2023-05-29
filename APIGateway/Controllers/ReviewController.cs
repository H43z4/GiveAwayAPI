﻿using Authentication.AuthSchemes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DatabaseModels.Authentication;
using Models.ViewModels.DSAuth.Setup;
using Models.ViewModels.PostManagemrnts;
using Models.ViewModels.ReviewManegment;
using Person;
using PostManagement;
using ReviewAndRating;
using SharedLib.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UserManagement;

namespace APIGateway.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = AuthenticationSchemes.JWT_BEARER_TOKEN_STATELESS)]
    public class ReviewController : ControllerBase
    {
        public IReviewServise _ReviewServise { get; }

        public ReviewController(IReviewServise Reviws)
        {
            this._ReviewServise = Reviws;
        }
        [HttpPost]
        public async Task<ApiResponse> ReviewRequest([FromForm] CreateReviewVM createReviw)
        {
            try
            {
                var ds = await _ReviewServise.ReviewRequest(createReviw);
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
        public async Task<ApiResponse> GetUserReviwToApprove(int userId)
        {
            try
            {
                var ds = await _ReviewServise.GetUserReviwToApprove(userId);
                if (ds.Count>0)
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
        public async Task<ApiResponse> ApproveReview(int userId)
        {
            try
            {
                var ds = await _ReviewServise.ApproveReview(userId);
                if (ds==1)
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
        public async Task<ApiResponse> RatingToSender(createRatingVM craeteRating)
        {
            try
            {
                var ds = await _ReviewServise.RatingToSender(craeteRating);
                if (ds==1)
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
