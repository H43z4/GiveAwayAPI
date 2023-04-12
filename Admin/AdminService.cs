using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
//using Models.DatabaseModels.VehicleRegistration.Core;
using Models.ViewModels.Identity;
using Models.ViewModels.Setup;
using RepositoryLayer;
using SharedLib.Interfaces;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Admin
{
    public interface IAdminService : ICurrentUser
    {
        Task<DataSet> GetPerson(long? personId, string cnic);
        Task<DataSet> GetBusiness(long? businessId, string ntn, string ftn, string stn);
    }

    public class AdminService : IAdminService
    {
        readonly AppDbContext appDbContext;
        readonly IAdoNet adoNet;

        public VwUser VwUser { get; set; }

        public AdminService(AppDbContext appDbContext, IAdoNet adoNet)
        {
            this.appDbContext = appDbContext;
            this.adoNet = adoNet;
        }


        #region private-Methods

        private async Task CommitTransaction()
        {
            using (var transaction = this.appDbContext.Database.BeginTransaction())
            {
                var rowsAffected = await this.appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }

        #endregion


        #region public-Get-Dropdowns


        #endregion

        #region public-Get-Methods

        public async Task<DataSet> GetPerson(long? personId, string cnic)
        {
            SqlParameter[] sql = new SqlParameter[2];
            sql[0] = new SqlParameter("@PersonId", personId);
            sql[1] = new SqlParameter("@CNIC", cnic);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Profiling].[GetPerson]", sql);

            return ds;
        }

        public async Task<DataSet> GetBusiness(long? businessId, string ntn, string ftn, string stn)
        {
            SqlParameter[] sql = new SqlParameter[4];
            sql[0] = new SqlParameter("@BusinessId", businessId);
            sql[1] = new SqlParameter("@NTN", ntn);
            sql[2] = new SqlParameter("@FTN", ftn);
            sql[3] = new SqlParameter("@STN", stn);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Profiling].[GetBusiness]", sql);

            return ds;
        }


        #endregion

        #region Public-Save-Methods

        //public Task<long> SaveBusiness(VwBusiness vwBusiness)
        //{
        //    throw new NotImplementedException();
        //}
        
        //public Task<long> SaveBusinessRep(VwBusinessRep vwBusinessRep)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

    }

}
