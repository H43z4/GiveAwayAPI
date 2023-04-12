using Models.DatabaseModels.VehicleRegistration.Core;
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
using Models.DatabaseModels.VehicleRegistration.Setup;
namespace VehicleRegistration
{
    public interface ITOService : ICurrentUser
    {
        Task<DataSet> GetOwnerVehicleDetails(string VehicleRegNo);
        Task<DataSet> SavePurchaser(VwTempOwner_v1 owner);
        Task<DataSet> SaveHPA(VwHPA vwTempHPA, long UserId);
        Task<DataSet> SaveKeeper(VwKeeper vwTempKeeper);
    }

    public class TOService : ITOService
    {
        readonly AppDbContext appDbContext;
        readonly IAdoNet adoNet;
        readonly IDBHelper dbHelper;

        public VwUser VwUser { get; set; }

        public TOService(AppDbContext appDbContext, IAdoNet adoNet, IDBHelper dbHelper)
        {
            this.appDbContext = appDbContext;
            this.adoNet = adoNet;
            this.dbHelper = dbHelper;
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

        #region public-Get-Methods
        public async Task<DataSet> GetOwnerVehicleDetails(string VehicleRegNo)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@VehicleRegNo", VehicleRegNo);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetOwnerVehicleDetail]", sql);

            return ds;
        }
        #endregion

        #region Public-Save-Methods



        public async Task<DataSet> SaveKeeper(VwKeeper vwTempKeeper)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", vwTempKeeper.ApplicationId);

            paramDict.Add("@Keeper", new List<Keeper>()
            {
                new Keeper()
                {
                    KeeperId = vwTempKeeper.KeeperId.Value,
                    OwnerId = vwTempKeeper.OwnerId,
                    VehicleId = 0,
                    KeeperPersonId = vwTempKeeper.KeeperId.Value
                }
            }.ToDataTable());


            //  var person = vwTempKeeper.Person;

            //EntityMapper<VwPerson, Person>.CopyByPropertyNameAndType(vwTempKeeper.Person, person);

            paramDict.Add("@Person", new List<Person>()
            {
                new Person()
                {

                    PersonId = vwTempKeeper.Person.PersonId,
                    CountryId = vwTempKeeper.Person.CountryId,
                    PersonName = vwTempKeeper.Person.PersonName,
                    FatherHusbandName = vwTempKeeper.Person.FatherHusbandName,
                    CNIC = vwTempKeeper.Person.CNIC,
                    Email = vwTempKeeper.Person.Email,
                    OldCNIC = vwTempKeeper.Person.OldCNIC,
                    NTN = vwTempKeeper.Person.NTN,
                    FTN = vwTempKeeper.Person.FTN,
                    CreatedBy = this.VwUser.UserId
                }

            }.ToDataTable());

            var addresses = new List<Address>();

            vwTempKeeper.Person.Addresses.ForEach(x =>
            {
                var address = new Address();
                EntityMapper<VwAddress, Address>.CopyByPropertyNameAndType(x, address);
                addresses.Add(address);
            });

            var phoneNumbers = new List<PhoneNumber>();

            vwTempKeeper.Person.PhoneNumbers.ForEach(x =>
            {
                var phoneNumber = new PhoneNumber();
                EntityMapper<VwPhoneNumber, PhoneNumber>.CopyByPropertyNameAndType(x, phoneNumber);
                phoneNumbers.Add(phoneNumber);
            });

            paramDict.Add("@Address", addresses.ToDataTable());
            paramDict.Add("@PhoneNumber", phoneNumbers.ToDataTable());
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[TO_SaveKeeper]", paramDict);

            return ds;
        }

        public async Task<DataSet> SaveHPA(VwHPA vwTempHPA, long UserId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@HPA", new List<HPA>()
                    {
                        new HPA()
                        {

                          HPAId = vwTempHPA.HPAId.Value,
                          ApplicationId = vwTempHPA.ApplicationId.Value,
                          OwnerId = vwTempHPA.OwnerId.Value,
                          VehicleId = 0,
                          SponserBusinessId = vwTempHPA.SponserBusinessId.Value,
                          HPADate = vwTempHPA.HPADate,
                          HPAStartDate = vwTempHPA.StartDate.Value,
                          HPAEndDate = vwTempHPA.EndDate,
                          LetterNo = vwTempHPA.HPALetterNo,
                          LetterDate = vwTempHPA.LetterDate.Value,
                          Terms = vwTempHPA.HPATerms,
                          HPACurrentStatusId = vwTempHPA.HPAStatusId.Value,
                          HPAStatusDated = vwTempHPA.HPAStatusDated.Value,
                          CreatedBy = UserId
                        }
                     }.ToDataTable());

            paramDict.Add("@UserId", UserId);
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[TO_SaveHPA]", paramDict);

            return ds;
        }

        public async Task<DataSet> SavePurchaser(VwTempOwner_v1 vwOwner)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@Application", new List<Application>()
            {
                new Application()
                {
                    ApplicationId = vwOwner.ApplicationId.Value,
                    BusinessProcessId = 43,
                    DistrictId = this.VwUser.UserDistrictId,
                    CreatedBy = this.VwUser.UserId,
                    ReceivedAt = DateTime.Now

                }
            }.ToDataTable());

            paramDict.Add("@Owner", new List<Owner>()
            {
                new Owner()
                {
                    OwnerTypeId = vwOwner.OwnerTypeId.Value,
                    OwnerTaxGroupId = vwOwner.OwnerTaxGroupId.Value,
                    CreatedBy = this.VwUser.UserId

                }
            }.ToDataTable());

            if (vwOwner.OwnerTypeId == 1)
            {
                var person = vwOwner.Persons.FirstOrDefault();

                var personDataModel = new Person();
                EntityMapper<VwPerson, Person>.CopyByPropertyNameAndType(person, personDataModel);

                var personList = new List<Person>();
                personList.Add(personDataModel);

                var addresses = new List<Address>();

                person.Addresses.ForEach(x =>
                {
                    var address = new Address();
                    EntityMapper<VwAddress, Address>.CopyByPropertyNameAndType(x, address);
                    addresses.Add(address);
                });

                var phoneNumbers = new List<PhoneNumber>();

                person.PhoneNumbers.ForEach(x =>
                {
                    var phoneNumber = new PhoneNumber();
                    EntityMapper<VwPhoneNumber, PhoneNumber>.CopyByPropertyNameAndType(x, phoneNumber);
                    phoneNumbers.Add(phoneNumber);
                });

                paramDict.Add("@Person", personList.ToDataTable());
                paramDict.Add("@Business", null);
                paramDict.Add("@Address", addresses.ToDataTable());
                paramDict.Add("@PhoneNumber", phoneNumbers.ToDataTable());
            }
            else if (vwOwner.OwnerTypeId == 2)
            {
                var business = new Business();
                EntityMapper<VwBusiness, Business>.CopyByPropertyNameAndType(vwOwner.Business, business);
                var businessList = new List<Business>();
                businessList.Add(business);

                var addresses = new List<Address>();

                vwOwner.Business.Addresses.ForEach(x =>
                {
                    var address = new Address();
                    EntityMapper<VwAddress, Address>.CopyByPropertyNameAndType(x, address);
                    addresses.Add(address);
                });

                var phoneNumbers = new List<PhoneNumber>();

                vwOwner.Business.PhoneNumbers.ForEach(x =>
                {
                    var phoneNumber = new PhoneNumber();
                    EntityMapper<VwPhoneNumber, PhoneNumber>.CopyByPropertyNameAndType(x, phoneNumber);
                    phoneNumbers.Add(phoneNumber);
                });

                paramDict.Add("@Person", null);
                paramDict.Add("@Business", businessList.ToDataTable());
                paramDict.Add("@Address", addresses.ToDataTable());
                paramDict.Add("@PhoneNumber", phoneNumbers.ToDataTable());
            }

            paramDict.Add("@VehicleId", vwOwner.VehicleId.Value);
            paramDict.Add("@SellerId", vwOwner.SellerId.Value);
            paramDict.Add("@UserId", this.VwUser.UserId);
            
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[TO_SavePurchaser]", paramDict);

            return ds;
        }

        #endregion
    }
}