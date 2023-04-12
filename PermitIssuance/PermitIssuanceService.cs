//using Models.DatabaseModels.VehicleRegistration.Core;
using Models.ViewModels.Identity;
using Models.ViewModels.VehicleRegistration.Core;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer;
using SharedLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using SharedLib.Common;
using Models.DatabaseModels.PermitIssuance.Setup;
using Models.DatabaseModels.PermitIssuance.Core;
//using Models.DatabaseModels.VehicleRegistration.Setup;
using Models.ViewModels.PermitIssuance.Core;
using Models.ViewModels.PermitIssuance.Setup;

namespace PermitIssuance
{
    public interface IPermitIssuanceService : ICurrentEPRSUser

    {
        Task<DataSet> SavePermit(VwPermitIssueApplication permitApp);
        Task<DataSet> GetPermitApplicationListById (int id);
        Task<DataSet> GetPermitList();

    }
    public class PermitIssuanceService : IPermitIssuanceService
    {
        readonly AppDbContext appDbContext;
        readonly IAdoNet adoNet;
        readonly IDBHelper dbHelper;
        public VwEPRSUser VwUser { get; set; }
        public VwEPRSUser VwEPRSUser { get; set; }

        public PermitIssuanceService(AppDbContext appDbContext, IAdoNet adoNet, IDBHelper dbHelper)
        {
            this.appDbContext = appDbContext;
            this.adoNet = adoNet;
            this.dbHelper = dbHelper;
        }

        public async Task<DataSet> SavePermit(VwPermitIssueApplication permitApp)
        {

            try
            {
                Dictionary<string, object> paramDict = new Dictionary<string, object>();

                paramDict.Add("@ApplicationId", permitApp.ApplicationId);
                paramDict.Add("@OldPermitNo", permitApp.OldPermitNo.HasValue ? permitApp.OldPermitNo.Value : null);
                paramDict.Add("@PermitTypeId", permitApp.PermitTypeId);
                paramDict.Add("@PersonName", permitApp.PersonName);
                paramDict.Add("@FatherHusbandName", permitApp.FatherHusbandName);
                paramDict.Add("@Address", permitApp.Address);
                paramDict.Add("@DateofBirth", permitApp.DateofBirth);
                paramDict.Add("@CellNo", permitApp.CellNo);
                paramDict.Add("@CityId", permitApp.CityId.HasValue ? permitApp.CityId.Value : null);
                paramDict.Add("@DistrictId", permitApp.DistrictId.HasValue ? permitApp.DistrictId.Value : null);
                paramDict.Add("@ProfessionId", permitApp.ProfessionId.HasValue ? permitApp.ProfessionId.Value : null);
                paramDict.Add("@ProfessionName", permitApp.ProfessionName.ToString());
                paramDict.Add("@CountryId", permitApp.CountryId.HasValue ? permitApp.CountryId : null);
                paramDict.Add("@CNIC", permitApp.CNIC);
                paramDict.Add("@PassportNo", permitApp.PassportNo);
                paramDict.Add("@Nationality", permitApp.Nationality /*"Christian"*/);
                paramDict.Add("@VisaExpiryDate", permitApp.VisaExpiryDate.HasValue ? permitApp.VisaExpiryDate.ToString() : null);
                paramDict.Add("@SponsorCompanyNTN", permitApp.SponsorCompanyNTN);
                paramDict.Add("@SponsorCompanyName", permitApp.SponsorCompanyName);
                paramDict.Add("@SponsorPersonCNIC", permitApp.SponsorPersonCNIC);
                paramDict.Add("@SponsorPersonName", permitApp.SponsorPersonName);
                paramDict.Add("@SponsorTypeID", permitApp.SponsorTypeID.HasValue ? permitApp.SponsorTypeID : null);
                //paramDict.Add("@UserId", 1);
                paramDict.Add("@UserId", this.VwEPRSUser.UserId);

                var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[SavePermitApplication]", paramDict);

                return ds;
            }
            catch (Exception e)
            {

                throw;
            }
         
        }


        public async Task<DataSet> GetPermitList()
        {
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[GetPermitApplicationList]", null);
            return ds;
        }

        public async Task<DataSet> GetPermitApplicationListById(int Id)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@Id", Id);
            
            var ds = await dbHelper.GetDataSetByStoredProcedure("[Core].[GetPermitApplicationListById]", paramDict);

            return ds;
        }
    }
}