using Microsoft.Data.SqlClient;
using Models.DatabaseModels.Biometric;
using Models.ViewModels.Biometric;
using Models.ViewModels.Identity;
using RepositoryLayer;
using SharedLib.Interfaces;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Biometric
{
    public interface IBiometricService : ICurrentUser
    {
        Task<DataSet> GetBusinessDropDowns();
        Task<DataSet> GetAssociations(string ntn);
        Task<DataSet> GetVehicleInfo(VwVehicleInfoIM vwVehicleInfoIM);
        Task<DataSet> SaveBiometricInfo(VwBiometricInfo vwBiometricInfo);
        Task<int> SaveNadraFranchise(VwNadraFranchise vwNadraFranchise);
    }

    public class BiometricService : IBiometricService
    {
        readonly AppDbContext appDbContext;
        readonly IAdoNet adoNet;

        public VwUser VwUser { get; set; }

        public BiometricService(AppDbContext appDbContext, IAdoNet adoNet)
        {
            this.appDbContext = appDbContext;
            this.adoNet = adoNet;
        }


        #region private-Methods

        private async Task<int> CommitTransaction()
        {
            int rowsAffected;

            using (var transaction = this.appDbContext.Database.BeginTransaction())
            {
                rowsAffected = await this.appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }

            return rowsAffected;
        }

        #endregion


        #region public-Get-Dropdowns

        public async Task<DataSet> GetBusinessDropDowns()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Setup].[GetOwnersDropDowns]", null);

            return ds;
        }

        #endregion

        #region public-Get-Methods

        public async Task<DataSet> GetVehicleInfo(VwVehicleInfoIM vwVehicleInfoIM)
        {
            SqlParameter[] sql = new SqlParameter[2];
            sql[0] = new SqlParameter("@CNIC", vwVehicleInfoIM.CNIC);
            sql[1] = new SqlParameter("@ApplicationId", vwVehicleInfoIM.MvrsTransId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Biometric].[GetVehicleInfo]", sql);

            return ds;
        }

        public async Task<DataSet> GetAssociations(string ntn)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@NTN", ntn);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Biometric].[GetAssociations]", sql);

            return ds;
        }

        #endregion

        #region Public-Save-Methods

        public async Task<DataSet> SaveBiometricInfo(VwBiometricInfo vwBiometricInfo)
        {
            SqlParameter[] sql = new SqlParameter[7];
            sql[0] = new SqlParameter("@ApplicationId", vwBiometricInfo.MvrsTransId);
            sql[1] = new SqlParameter("@CNIC", vwBiometricInfo.CNIC);
            sql[2] = new SqlParameter("@RegNo", vwBiometricInfo.RegNo);
            sql[3] = new SqlParameter("@NadraTransId", vwBiometricInfo.NadraTransId);
            sql[4] = new SqlParameter("@NadraFranchiseId", vwBiometricInfo.NadraFranchiseId);
            sql[5] = new SqlParameter("@IsVerified", vwBiometricInfo.IsVerified);
            sql[6] = new SqlParameter("@UserId", this.VwUser.UserId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Biometric].[SaveBiometricInfo]", sql);

            return ds;
        }

        public async Task<int> SaveNadraFranchise(VwNadraFranchise vwNadraFranchise)
        {
            if (vwNadraFranchise.NadraFranchiseId == 0)
            {
                this.appDbContext.NadraFranchise.Add(new Models.DatabaseModels.Biometric.NadraFranchise()
                {
                    CreatedBy = this.VwUser.UserId,
                    FranchiseCellNo = vwNadraFranchise.FranchiseCellNo,
                    FranchiseName = vwNadraFranchise.FranchiseName,
                    FranchiseShopName = vwNadraFranchise.FranchiseShopName
                });
            }
            else
            {
                var nadraFranchise = this.appDbContext.NadraFranchise.SingleOrDefault(x => x.NadraFranchiseId == vwNadraFranchise.NadraFranchiseId);

                if (nadraFranchise is not null)
                {
                    nadraFranchise.FranchiseCellNo = vwNadraFranchise.FranchiseCellNo;
                    nadraFranchise.FranchiseName = vwNadraFranchise.FranchiseName;
                    nadraFranchise.FranchiseShopName = vwNadraFranchise.FranchiseShopName;

                    this.appDbContext.Entry<NadraFranchise>(nadraFranchise).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
            }

            var rowsAffected = await this.CommitTransaction();

            return rowsAffected;
        }

        #endregion
    }

}
