using Microsoft.Data.SqlClient;
using Models.ViewModels.Identity;
using Models.ViewModels.SeriesNumberPool;
using Models.ViewModels.SeriesNumberPool.Core;
using RepositoryLayer;
using SharedLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesNumberPool
{
    public interface ISeriesNumberPoolService : ICurrentUser
    {
        Task<DataSet> GetAuctionNumbers(long seriesCategoryId, long seriesId);
        Task<DataSet> GetAuctionSchedule();
        Task<int> SaveAuctionResult(List<VwAuctionResult> vwAuctionResult);

        Task<DataSet> GetAuctionScheduleSeriesCount(int seriesCategoryId);
        Task<DataSet> GetAuctionScheduleSeries(int seriesCategoryId, int pageNo, int RecordsCount);
        Task<DataSet> GetSeriesCategoryDropDowns();
        Task<DataSet> GetDistrictsDropDowns();
        Task<DataSet> GetSeriesConsumerDropDowns();
        Task<DataSet> GetSeriesInfoByCategoryId(int seriesCategoryId);
        Task<DataSet> GetSeriesInfoBySeriesId(int seriesCategoryId, long seriesId);

        Task<DataSet> GetSeriesAssignmentCount(int seriesConsumerId);
        Task<DataSet> GetSeriesAssignmentInfoByConsumerId(long seriesConsumerId, int pageNo, int recordsCount);

        Task<DataSet> GetCustomerCreditInfo(string ChasisNo);

        Task<long> SaveSeriesSchedule(VwSeries vwSeries);

        Task<long> SaveSeriesAssignment(VwSeriesAssignment vwSeriesAssignment);
        Task<long> ActivateSeriesforMTC(long AuctionScheduleId, long SeriesId);

        Task<long> SaveCustomerPaymentIntimation(VwAdvancePayment vwAdvancePayment);
        
    }

    public class SeriesNumberPoolService : ISeriesNumberPoolService
    {
        readonly AppDbContext appDbContext;
        readonly IAdoNet adoNet;

        public VwUser VwUser { get; set; }

        public SeriesNumberPoolService(AppDbContext appDbContext, IAdoNet adoNet)
        {
            this.appDbContext = appDbContext;
            this.adoNet = adoNet;
        }


        #region private-Methods

        private async Task<int> CommitTransaction()
        {
            var rowsAffected = 0;

            using (var transaction = this.appDbContext.Database.BeginTransaction())
            {
                rowsAffected = await this.appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }

            return rowsAffected;
        }

        #endregion

        #region public-Get-Methods

        public async Task<DataSet> GetAuctionSchedule()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[SRNRPL].[GetAuctionSchedule]", null);

            return ds;
        }
        
        public async Task<DataSet> GetAuctionNumbers(long seriesCategoryId, long seriesId)
        {
            SqlParameter[] sql = new SqlParameter[2];
            sql[0] = new SqlParameter("@SeriesCategoryId", seriesCategoryId);
            sql[1] = new SqlParameter("@SeriesId", seriesId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[SRNRPL].[GetAuctionNumbers]", null);

            return ds;
        }

        public async Task<DataSet> GetAuctionScheduleSeriesCount(int seriesCategoryId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@SeriesCategoryId", seriesCategoryId);
            

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[SRNRPL].[GetSeriesCount]", sql);

            return ds;
        }
        public async Task<DataSet> GetAuctionScheduleSeries(int seriesCategoryId, int pageNo, int RecordsCount)
        {
            SqlParameter[] sql = new SqlParameter[3];
            sql[0] = new SqlParameter("@SeriesCategoryId", seriesCategoryId);
            sql[1] = new SqlParameter("@PageNo", pageNo);
            sql[2] = new SqlParameter("@NoOfRecords", RecordsCount);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[SRNRPL].[GetAuctionScheduleSeries]", sql);

            return ds;
        }
        public async Task<DataSet> GetSeriesCategoryDropDowns()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[SRNRPL].[GetSeriesCategories]", null);

            return ds;
        }

        public async Task<DataSet> GetDistrictsDropDowns()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Setup].[GetDistrictList]", null);

            return ds;
        }

        public async Task<DataSet> GetSeriesConsumerDropDowns()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Auth].[GetMTCUsersList]", null);

            return ds;
        }

        public async Task<DataSet> GetSeriesInfoByCategoryId(int seriesCategoryId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@SeriesCategoryId", seriesCategoryId);
           

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[SRNRPL].[GetSeriesInfoByCategoryId]", sql);

            return ds;
        }
        public async Task<DataSet> GetSeriesInfoBySeriesId(int seriesCategoryId, long seriesId)
        {
            SqlParameter[] sql = new SqlParameter[2];
            sql[0] = new SqlParameter("@SeriesCategoryId", seriesCategoryId);
            sql[1] = new SqlParameter("@SeriesId", seriesId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[SRNRPL].[GetSeriesInfoBySeriesId]", sql);

            return ds;
        }

        public async Task<DataSet> GetSeriesAssignmentCount(int seriesConsumerId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@ConsumerId", seriesConsumerId);


            var ds = await this.adoNet.ExecuteUsingDataAdapter("[SRNRPL].[GetSeriesAssignmentCount]", sql);

            return ds;
        }

        public async Task<DataSet> GetSeriesAssignmentInfoByConsumerId(long seriesConsumerId, int pageNo, int recordsCount)
        {
            SqlParameter[] sql = new SqlParameter[3];
            sql[0] = new SqlParameter("@ConsumerId", seriesConsumerId);
            sql[1] = new SqlParameter("@PageNo", pageNo);
            sql[2] = new SqlParameter("@NoOfRecords", recordsCount);
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[SRNRPL].[GetSeriesAssignmentInfo]", sql);

            return ds;
        }

        public async Task<DataSet> GetCustomerCreditInfo(string ChasisNo)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@ChasisNo", ChasisNo);


            var ds = await this.adoNet.ExecuteUsingDataAdapter("[SRNRPL].[GetCustomerCreditInfo]", sql);

            return ds;
        }

        #endregion

        #region public-Post-Methods

        public async Task<int> SaveAuctionResult(List<VwAuctionResult> vwAuctionResult)
        {
            foreach (var result in vwAuctionResult)
            {
                this.appDbContext.AuctionResult.Add(new Models.DatabaseModels.SeriesNumberPool.AuctionResult()
                {
                    Address = result.Address,
                    AuctionedValue = result.AuctionedValue,
                    ChasisNumber = result.ChasisNumber,
                    CNIC = result.CNIC,
                    Email = result.Email,
                    FatherHusbandName = result.FatherHusbandName,
                    NTN = result.NTN,
                    OwnerName = result.OwnerName,
                    PhoneNumber = result.PhoneNumber,
                    SeriesCategoryId = result.SeriesCategoryId,
                    SeriesId = result.SeriesId,
                    SeriesNumberId = result.SeriesNumberId,
                    WinnerAIN = result.WinnerAIN,
                    WinnerName = result.WinnerName
                });
            }

            var rowsAffected = await this.CommitTransaction();

            return rowsAffected;
        }

        public async Task<long> SaveSeriesSchedule(VwSeries vwSeries)
        {

            SqlParameter[] sql = new SqlParameter[8];
            sql[0] = new SqlParameter("@AuctionScheduleId", vwSeries.AuctionScheduleId);
            sql[1] = new SqlParameter("@SeriesId", vwSeries.SeriesId);
            sql[2] = new SqlParameter("@SeriesCategoryId", vwSeries.SeriesCategoryId);
            sql[3] = new SqlParameter("@RegStartDateTime", vwSeries.RegStartDateTime);
            sql[4] = new SqlParameter("@RegEndDateTime", vwSeries.RegEndDateTime);
            sql[5] = new SqlParameter("@AuctionStartDateTime", vwSeries.AuctionStartDateTime);
            sql[6] = new SqlParameter("@AuctionEndDateTime", vwSeries.AuctionEndDateTime);
            sql[7] = new SqlParameter("@Updatedby", this.VwUser.UserId);

            var rowsAffected = await this.adoNet.ExecuteNonQuery("[SRNRPL].[SaveSeriesSchedule]", sql);
            return rowsAffected;
        }

        public async Task<long> SaveSeriesAssignment(VwSeriesAssignment vwSeriesAssignment)
        {

            SqlParameter[] sql = new SqlParameter[5];
            sql[0] = new SqlParameter("@SeriesAssignmentId", vwSeriesAssignment.SeriesAssignmentId);
            sql[1] = new SqlParameter("@SeriesCategoryId", vwSeriesAssignment.SeriesCategoryId);
            sql[2] = new SqlParameter("@SeriesId", vwSeriesAssignment.SeriesId);
            sql[3] = new SqlParameter("@ConsumerId", vwSeriesAssignment.SeriesConsumerId);
            sql[4] = new SqlParameter("@CreatedBy", this.VwUser.UserId);

            var rowsAffected = await this.adoNet.ExecuteNonQuery("[SRNRPL].[SaveSeriesAssignment]", sql);
            return rowsAffected;
        }

        public async Task<long> ActivateSeriesforMTC(long AuctionScheduleId, long SeriesId)
        {

            SqlParameter[] sql = new SqlParameter[3];
            sql[0] = new SqlParameter("@AuctionScheduleId", AuctionScheduleId);
            sql[1] = new SqlParameter("@SeriesId", SeriesId);
            sql[2] = new SqlParameter("@CreatedBy", this.VwUser.UserId);

            var rowsAffected = await this.adoNet.ExecuteNonQuery("[SRNRPL].[ActivateSeries]", sql);
            return rowsAffected;
        }

        public async Task<long> SaveCustomerPaymentIntimation(VwAdvancePayment vwAdvancePayment)
        {

            SqlParameter[] sql = new SqlParameter[13];
            sql[0] = new SqlParameter("@SeriesCategoryId", vwAdvancePayment.SeriesCategoryId);
            sql[1] = new SqlParameter("@SeriesId", vwAdvancePayment.SeriesId);
            sql[2] = new SqlParameter("@SeriesNumberId", vwAdvancePayment.SeriesNumberId);
            sql[3] = new SqlParameter("@AdvancePaymentStatusId", vwAdvancePayment.AdvancePaymentStatusId);
            sql[4] = new SqlParameter("@ChasisNumber", vwAdvancePayment.ChasisNumber);
            sql[5] = new SqlParameter("@BasePrice", vwAdvancePayment.BasePrice);
            sql[6] = new SqlParameter("@PSId", vwAdvancePayment.PSId);
            sql[7] = new SqlParameter("@PaidOn", vwAdvancePayment.PaidOn);
            sql[8] = new SqlParameter("@BankCode", vwAdvancePayment.BankCode);
            sql[9] = new SqlParameter("@OwnerName", vwAdvancePayment.OwnerName);
            sql[10] = new SqlParameter("@CNIC", vwAdvancePayment.CNICNTN);
            sql[11] = new SqlParameter("@NTN", vwAdvancePayment.CNICNTN);
            sql[12] = new SqlParameter("@CreatedBy", this.VwUser.UserId);

            var rowsAffected = await this.adoNet.ExecuteNonQuery("[SRNRPL].[SaveCustomerPaymentIntimation]", sql);
            return rowsAffected;
        }
        #endregion



    }
}
