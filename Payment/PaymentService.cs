﻿using Models.ViewModels.Identity;
using Models.ViewModels.Payment;
using RepositoryLayer;
using SharedLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Payment
{
    public interface IPaymentService : ICurrentUser
    {
        Task<DataSet> GenerateChallan(long applicationId);
        Task<DataSet> GetPayeeInfo(long applicationId);
        Task<VwPayeeInfo> GetPSId(VwPayeeInfo payeeInfo);
        Task<DataSet> SavePSId(VwPayeeInfo payeeInfo);
        Task<DataSet> SavePaymentIntimation(VwPayeeIntimation payeeIntimation);
    }

    public class PaymentService : IPaymentService
    {
        readonly IDBHelper dbHelper;
        public VwUser VwUser { get; set; }

        public PaymentService(IDBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public async Task<DataSet> GenerateChallan(long applicationId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ApplicationId", applicationId);
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Payments].[GenerateChallan]", paramDict);

            return ds;
        }

        public async Task<DataSet> GetPayeeInfo(long applicationId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ApplicationId", applicationId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Payments].[GetPayeeInfo]", paramDict);

            return ds;
        }

        public async Task<VwPayeeInfo> GetPSId(VwPayeeInfo payeeInfo)
        {
            payeeInfo.PSId = new Random().NextInt64(1, 9999999).ToString();
            return payeeInfo;
        }

        public async Task<DataSet> SavePSId(VwPayeeInfo payeeInfo)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ApplicationId", payeeInfo.ApplicationId);
            paramDict.Add("@ChallanId", payeeInfo.ChallanId);
            paramDict.Add("@PSId", payeeInfo.PSId);
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Payments].[SavePSId]", paramDict);

            return ds;
        }

        public async Task<DataSet> SavePaymentIntimation(VwPayeeIntimation payeeIntimation)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ChallanId", payeeIntimation.ChallanId);
            paramDict.Add("@PSId", payeeIntimation.PSId);
            paramDict.Add("@AmountPaid", payeeIntimation.AmountPaid);
            paramDict.Add("@PaidOn", payeeIntimation.PaidOn);
            paramDict.Add("@BankCode", payeeIntimation.BankCode);
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Payments].[SavePaymentIntimation]", paramDict);

            return ds;
        }

        //public async Task<ApiResponse> GetApplicationDetails(PayeeInfo payeeInfo)
        //{
        //    return ApiResponse.GetApiResponse(apiResponseType, taxesApplied, msg);
        //}

    }
}
