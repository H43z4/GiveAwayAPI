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
    public interface INRTService : ICurrentUser
    {
        Task<DataSet> GetOwnersDropDowns();
        Task<DataSet> GetVehiclesDropDowns();
        Task<DataSet> GetPurchaseDropDowns();
        Task<DataSet> GetDocumentDropDowns();

        Task<DataSet> GetApplications(long? applicationId, long applicationTypeId, long applicationStatusId);
        Task<DataSet> GetApplicationDetails(long applicationId);
        Task<DataSet> GetOwnerDetails(long ownerId);
        Task<DataSet> GetVehicle(long vehicleId);
        Task<DataSet> GetVehiclePurchaseInfo(long vehicleId);
        Task<DataSet> GetVehicleLocalPurchaseInfo(long vehicleId);
        Task<DataSet> GetVehicleImportInfo(long vehicleId);
        Task<DataSet> GetVehicleAuctionInfo(long vehicleId);
        Task<DataSet> GetHPA(long applicationId);
        Task<DataSet> GetKeeper(long ownerId);


        Task<DataSet> GetPersonByCNIC(string cnic);
        Task<VwBusiness> GetBusinessByTaxNumber(string ntn, string ftn, string stn);
        Task<DataSet> GetRemarks(long applicationId);
        Task<DataSet> SaveOwner(VwTempOwner_v1 owner);
        Task<DataSet> SaveSeller(VwTempOwner_v1 owner);
        Task<DataSet> SaveVehicle(VwVehicle vehicle, long UserId);
        Task<DataSet> SaveVehiclePurchaseInfo(VwVehiclePurchaseInfo vehiclePurchaseInfo, long UserId);
        Task<DataSet> SaveVehicleDocument(VwVehicleDocument vwVehicleDocument, long UserId);
        Task<DataSet> SaveHPA(VwHPA vwTempHPA, long UserId);
        Task<DataSet> SaveKeeper(VwKeeper vwTempKeeper, long UserId);
        Task<DataSet> SaveRemarks(VwRemarks remarks);
        //Task<long> SaveApplicationStatus(long applicationId, long applicationStatusId);
        //Task<long> SaveApplicationPhase(VwApplicationPhaseChange vwApplicationPhaseChange);
    }

    public class NRTService : INRTService
    {
        readonly AppDbContext appDbContext;
        readonly IAdoNet adoNet;
        readonly IDBHelper dbHelper;

        public VwUser VwUser { get; set; }

        public NRTService(AppDbContext appDbContext, IAdoNet adoNet, IDBHelper dbHelper)
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


        #region public-Get-Dropdowns

        public async Task<DataSet> GetOwnersDropDowns()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Setup].[GetOwnersDropDowns]", null);

            return ds;
        }
        public async Task<DataSet> GetVehiclesDropDowns()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Setup].[GetVehiclesDropDowns]", null);

            return ds;
        }
        public async Task<DataSet> GetPurchaseDropDowns()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Setup].[GetPurchaseDropDowns]", null);

            return ds;
        }
        public async Task<DataSet> GetDocumentDropDowns()
        {
            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Setup].[GetDocumentDropDowns]", null);

            return ds;
        }

        #endregion

        #region public-Get-Methods

        public async Task<DataSet> GetApplications(long? applicationId, long businessProcessId, long applicationStatusId)
        {
            SqlParameter[] sql = new SqlParameter[6];
            sql[0] = new SqlParameter("@ApplicationId", applicationId);
            sql[1] = new SqlParameter("@ApplicationStatusId", applicationStatusId);
            sql[2] = new SqlParameter("@BusinessProcessId", businessProcessId);
            sql[3] = new SqlParameter("@BusinessPhaseId", DBNull.Value);
            sql[4] = new SqlParameter("@BusinessPhaseStatusId", DBNull.Value);
            sql[5] = new SqlParameter("@CreatedBy", this.VwUser.UserId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetApplications]", sql);

            return ds;
        }

        public async Task<DataSet> GetApplicationDetails(long applicationId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@ApplicationId", applicationId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetApplicationDetail]", sql);

            return ds;
        }

        private async Task<DataSet> GetBusinessProcessInitialState(long businessProcessId)
        {
            SqlParameter[] sql = new SqlParameter[2];
            sql[0] = new SqlParameter("@BusinessProcessId", businessProcessId);
            sql[1] = new SqlParameter("@CreatedBy", this.VwUser.UserId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetBusinessProcessInitialState]", sql);

            return ds;
        }

        public async Task<DataSet> GetOwnerDetails(long ownerId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@OwnerId", ownerId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetOwnerDetail]", sql);

            return ds;
        }

        public async Task<DataSet> GetVehicle(long vehicleId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@VehicleId", vehicleId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetVehicle]", sql);

            return ds;
        }

        public async Task<DataSet> GetVehiclePurchaseInfo(long vehicleId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@VehicleId", vehicleId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetVehiclePurchaseInfo]", sql);

            return ds;
        }

        public async Task<DataSet> GetVehicleLocalPurchaseInfo(long vehicleId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@VehicleId", vehicleId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetVehicleLocalPurchaseInfo]", sql);

            return ds;
        }

        public async Task<DataSet> GetVehicleImportInfo(long vehicleId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@VehicleId", vehicleId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetVehicleImportInfo]", sql);

            return ds;
        }

        public async Task<DataSet> GetVehicleAuctionInfo(long vehicleId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@VehicleId", vehicleId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetVehicleAuctionInfo]", sql);

            return ds;
        }

        public async Task<DataSet> GetHPA(long applicationId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@ApplicationId", applicationId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetHPA]", sql);

            return ds;
        }

        public async Task<DataSet> GetKeeper(long ownerId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@OwnerId", ownerId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetKeeper]", sql);

            return ds;
        }



        public async Task<DataSet> GetPersonByCNIC(string cnic)
        {
            SqlParameter[] sql = new SqlParameter[2];
            sql[0] = new SqlParameter("@PersonId", DBNull.Value);
            sql[1] = new SqlParameter("@CNIC", cnic);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetPerson]", sql);

            return ds;
        }

        public async Task<DataSet> GetChallan(long applicationId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@ApplicationId", applicationId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetChallan]", sql);

            return ds;
        }

        public Task<VwBusiness> GetBusinessByTaxNumber(string ntn, string ftn, string stn)
        {
            //var business = this.appDbContext.Business.SingleOrDefault(x => x.NTN == ntn || x.FTN == ftn || x.STN == stn);
            var business = this.appDbContext.Business.FirstOrDefault(x => x.NTN == ntn);

            var vwBusiness = new VwBusiness()
            {
                Addresses = new List<VwAddress>(),
                PhoneNumbers = new List<VwPhoneNumber>()
            };

            if (business is not null)
            {
                SharedLib.Common.EntityMapper<Business, VwBusiness>.CopyByPropertyName(business, vwBusiness);

                var addresses = this.appDbContext.Address.Where(x => x.BusinessId == business.BusinessId).ToList();
                var phoneNumbers = this.appDbContext.PhoneNumber.Where(x => x.BusinessId == business.BusinessId).ToList();

                foreach (var address in addresses)
                {
                    VwAddress vwTempAddress = new VwAddress();

                    SharedLib.Common.EntityMapper<Address, VwAddress>.CopyByPropertyName(address, vwTempAddress);

                    vwBusiness.Addresses.Add(vwTempAddress);
                }

                foreach (var phoneNumber in phoneNumbers)
                {
                    VwPhoneNumber vwTempPhoneNumber = new VwPhoneNumber();

                    SharedLib.Common.EntityMapper<PhoneNumber, VwPhoneNumber>.CopyByPropertyName(phoneNumber, vwTempPhoneNumber);

                    vwBusiness.PhoneNumbers.Add(vwTempPhoneNumber);
                }
            }
            else
            {
                vwBusiness = null;
            }

            return Task.FromResult(vwBusiness);
        }

        public async Task<DataSet> GetRemarks(long applicationId)
        {
            SqlParameter[] sql = new SqlParameter[2];
            sql[0] = new SqlParameter("@ApplicationId", applicationId);
            sql[1] = new SqlParameter("@UserId", this.VwUser.UserId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetRemarks]", sql);

            return ds;
        }
        public async Task<DataSet> GetAssessment(long applicationId)
        {
            SqlParameter[] sql = new SqlParameter[1];
            sql[0] = new SqlParameter("@ApplicationId", applicationId);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetAssessment]", sql);

            return ds;
        }

        #endregion

        #region Public-Save-Methods


        public async Task<DataSet> SaveSeller(VwTempOwner_v1 vwOwner)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            
            paramDict.Add("@Application", new List<Application>() 
            { 
                new Application()             
                {
                    ApplicationId = vwOwner.ApplicationId.Value,
                    BusinessProcessId = 22,
                    DistrictId = this.VwUser.UserDistrictId,
                    CreatedBy = this.VwUser.UserId,
                    ReceivedAt = DateTime.Now

                }
            }.ToDataTable());
            
            paramDict.Add("@Owner", new List<Owner>()
            {
                new Owner()
                {
                    OwnerId = vwOwner.OwnerId.Value,
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

            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NRT_SaveSeller]", paramDict);

            return ds;
        }
        
        public async Task<DataSet> SaveOwner(VwTempOwner_v1 vwOwner)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            
            paramDict.Add("@ApplicationId", vwOwner.ApplicationId);

            paramDict.Add("@Owner", new List<Owner>()
            {
                new Owner()
                {
                    OwnerId = vwOwner.OwnerId.Value,
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

            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NRT_SaveOwner]", paramDict);

            return ds;
        }
        
        public async Task<DataSet> SaveVehicle(VwVehicle vwVehicle, long UserId)
        {
         
                Dictionary<string, object> paramDict = new Dictionary<string, object>();
            
                paramDict.Add("@Vehicle", new List<Vehicle>()
            {
                new Vehicle()
                {
                    VehicleId = vwVehicle.VehicleId.Value,
                    VehicleBodyConventionId = vwVehicle.VehicleBodyConventionId,
                    VehicleBodyTypeId = vwVehicle.VehicleBodyTypeId,
                    VehicleCategoryId = vwVehicle.VehicleCategoryId,
                    VehicleClassId = vwVehicle.VehicleClassId,
                    VehicleClassificationId = vwVehicle.VehicleClassificationId,
                    VehicleColorId = vwVehicle.VehicleColorId,
                    VehicleEngineTypeId = vwVehicle.VehicleEngineTypeId,
                    VehicleFuelTypeId = vwVehicle.VehicleFuelTypeId,
                    VehicleMakeId = vwVehicle.VehicleMakeId,
                    VehicleMakerId = vwVehicle.VehicleMakerId,
                    VehiclePurchaseTypeId = vwVehicle.VehiclePurchaseTypeId,
                    VehicleTypeId =  vwVehicle.VehicleTypeId.Value,
                    VehicleStatusId = vwVehicle.VehicleStatusId,
                    VehicleUsageId = vwVehicle.VehicleUsageId,
                    CreatedBy =UserId,
                    RegistrationDistrictId = vwVehicle.DistrictId.Value,
                    ChasisNo = vwVehicle.ChasisNo,
                    NoOfCylinder = vwVehicle.NoOfCylinder.Value,
                    EngineNo = vwVehicle.EngineNo,
                    EngineSize = vwVehicle.EngineSize.Value,
                    HorsePower = vwVehicle.HorsePower.Value,
                    LadenWeight = vwVehicle.LadenWeight.Value,
                    UnLadenWeight = vwVehicle.UnLadenWeight.Value,
                    Price = vwVehicle.Price.Value,
                    PurchaseDate = vwVehicle.PurchaseDate.Value,
                    RegistrationDate=System.DateTime.Now,
                    ManufacturingYear = vwVehicle.ManufacturingYear.Value,
                    SeatingCapacity = vwVehicle.SeatingCapacity.Value,
                    WheelBase = vwVehicle.WheelBase
                }
            }.ToDataTable());
            paramDict.Add("@VehicleAdditionalInfo", new List<VehicleAdditionalInfo>()
            {
                new VehicleAdditionalInfo()
                {
                    VehicleId = vwVehicle.VehicleId.Value,
                    VehicleRCStatusId = vwVehicle.VehicleRCStatusId,
                    IsIncomeTaxExempted = vwVehicle.IsIncomeTaxExempted,
                    IsHPA = false,
                    IsTaxExempted = vwVehicle.IsTaxExempted,
                    TaxFrequency = vwVehicle.TaxFrequency,
                    FitnessCertValidFrom = vwVehicle.FitnessCertValidFrom,
                    FitnessCertValidTo = vwVehicle.FitnessCertValidTo,
                    CreatedBy = UserId
                }
            }.ToDataTable());

            paramDict.Add("@ApplicationId", vwVehicle.ApplicationId);
            paramDict.Add("@UserId", UserId);
          
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NRT_SaveVehicle]", paramDict);

            return ds;
        }


        public async Task<DataSet> SaveVehiclePurchaseInfo(VwVehiclePurchaseInfo vehiclePurchaseInfo, long UserId)
        {

            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            if (vehiclePurchaseInfo.AuctionInfo == null)
            {
                    paramDict.Add("@AuctionInfo", new List<VehicleAuctionInfo>() { }.ToDataTable());
            }
            else
            {
                    paramDict.Add("@AuctionInfo", new List<VehicleAuctionInfo>()
                {
                    new VehicleAuctionInfo()
                    {
                       VehicleAuctionInfoId = vehiclePurchaseInfo.AuctionInfo.AuctionInfoId,
                       ApplicationId = vehiclePurchaseInfo.ApplicationId.Value,
                       VehicleId = vehiclePurchaseInfo.VehicleId.Value,
                       LotNo = vehiclePurchaseInfo.AuctionInfo.LotNo,
                       BatchNo = vehiclePurchaseInfo.AuctionInfo.BatchNo,
                       CategoryNo = vehiclePurchaseInfo.AuctionInfo.CategoryNo,
                       VoucherNo = vehiclePurchaseInfo.AuctionInfo.VoucherNo,
                       VoucherDated = vehiclePurchaseInfo.AuctionInfo.VoucherDated.Value,
                       CreatedBy = UserId
                    }
                }.ToDataTable());
            }
            
            if (vehiclePurchaseInfo.ImportInfo == null)
            {
                paramDict.Add("@ImportInfo", new List<VehicleImportInfo>() { }.ToDataTable());

            }
            else
            {
                    paramDict.Add("@ImportInfo", new List<VehicleImportInfo>()
                {
                    new VehicleImportInfo()
                    {
                       VehicleImportInfoId = vehiclePurchaseInfo.ImportInfo.ImportInfoId,
                       ApplicationId = vehiclePurchaseInfo.ApplicationId.Value,
                       VehicleId = vehiclePurchaseInfo.VehicleId.Value,
                       ImporterName = vehiclePurchaseInfo.ImportInfo.ImporterName,
                       ImporterAddress = vehiclePurchaseInfo.ImportInfo.ImporterAddress,
                       IGMNo = vehiclePurchaseInfo.ImportInfo.IGMNo,
                       IGMDate = vehiclePurchaseInfo.ImportInfo.IGMDate,
                       IndexNo = vehiclePurchaseInfo.ImportInfo.IndexNo.Value,
                       PlaceOfIssue = vehiclePurchaseInfo.ImportInfo.PlaceOfIssue,
                       ImportPermitNo = vehiclePurchaseInfo.ImportInfo.ImportPermitNo,
                       PermitIssueDate    = vehiclePurchaseInfo.ImportInfo.PermitIssueDate.Value,
                       PortId = vehiclePurchaseInfo.ImportInfo.PortId.Value,
                       ClearingAgentId = vehiclePurchaseInfo.ImportInfo.ClearingAgentId.Value,
                       CustomCollectorateId = vehiclePurchaseInfo.ImportInfo.CustomCollectorateId.Value,
                       CountryId = vehiclePurchaseInfo.ImportInfo.CountryId.Value,
                       VehicleSchemeId = vehiclePurchaseInfo.ImportInfo.VehicleSchemeId.Value,
                       ImportValue = vehiclePurchaseInfo.ImportInfo.ImportValue.Value,
                       CustomDuty = vehiclePurchaseInfo.ImportInfo.CustomDuty.Value,
                       SalesTax = vehiclePurchaseInfo.ImportInfo.SalesTax.Value,
                       ImportLicenseFee = vehiclePurchaseInfo.ImportInfo.ImportLicenseFee.Value,
                       Insurrance = vehiclePurchaseInfo.ImportInfo.Insurrance.Value,
                       AnyOtherCost = vehiclePurchaseInfo.ImportInfo.AnyOtherCost.Value,
                       LandedCost = vehiclePurchaseInfo.ImportInfo.LandedCost.Value,
                       PaymentDate = vehiclePurchaseInfo.ImportInfo.PaymentDate.Value,
                       BankId = vehiclePurchaseInfo.ImportInfo.BankId.Value,
                       CreatedBy = UserId
                    }
                }.ToDataTable());
            }

            if (vehiclePurchaseInfo.LocalPurchaseInfo == null)
            {
                paramDict.Add("@PurchaseInfo", new List<VehiclePurchaseInfo>() { }.ToDataTable());
            }
            else
            {
                        paramDict.Add("@PurchaseInfo", new List<VehiclePurchaseInfo>()
                    {
                        new VehiclePurchaseInfo()
                        {

                           VehiclePurchaseInfoId = vehiclePurchaseInfo.LocalPurchaseInfo.LocalPurchaseInfoId,
                            ApplicationId  = vehiclePurchaseInfo.ApplicationId.Value,
                            VehicleId = vehiclePurchaseInfo.VehicleId.Value,
                            VehicleSchemeId = vehiclePurchaseInfo.LocalPurchaseInfo.VehicleSchemeId.Value,
                            DealerId = vehiclePurchaseInfo.LocalPurchaseInfo.DealerId.Value,
                            CertificateNo = vehiclePurchaseInfo.LocalPurchaseInfo.CertificateNo,
                            CertificateDated = vehiclePurchaseInfo.LocalPurchaseInfo.CertificateDated.Value,
                            InvoiceNo = vehiclePurchaseInfo.LocalPurchaseInfo.InvoiceNo,
                            InvoiceDated = vehiclePurchaseInfo.LocalPurchaseInfo.InvoiceDated.Value,
                            CreatedBy = UserId
                        }
                     }.ToDataTable());
            }

            

            paramDict.Add("@ApplicationId", vehiclePurchaseInfo.ApplicationId.Value);
            paramDict.Add("@VehiclePurchaseTypeId", vehiclePurchaseInfo.VehiclePurchaseTypeId);
            paramDict.Add("@UserId", UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NRT_SavePurchaseInfo]", paramDict);
            return ds;
        }
        public async Task<Person> SavePerson(VwPerson vwPerson)
        {
            Person person;

            if (vwPerson.PersonId == 0)
            {
                person = new Person()
                {
                    CountryId = vwPerson.CountryId,
                    CNIC = vwPerson.CNIC,
                    Email = vwPerson.Email,
                    FatherHusbandName = vwPerson.FatherHusbandName,
                    PersonName = vwPerson.PersonName,
                    NTN = vwPerson.NTN,
                    OldCNIC = vwPerson.OldCNIC,
                    CreatedBy = this.VwUser.UserId
                };

                foreach (var address in vwPerson.Addresses)
                {
                    this.appDbContext.Address.Add(new Address()
                    {
                        AddressDescription = address.AddressDescription,
                        AddressTypeId = address.AddressTypeId.Value,
                        CreatedBy = this.VwUser.UserId,
                        City = address.City,
                        DistrictId = address.DistrictId.Value,
                        PostalCode = address.PostalCode,
                        Person = person
                    });
                }

                foreach (var phoneNumber in vwPerson.PhoneNumbers)
                {
                    this.appDbContext.PhoneNumber.Add(new PhoneNumber()
                    {
                        CountryId = phoneNumber.CountryId,
                        CreatedBy = this.VwUser.UserId,
                        Person = person,
                        PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId.Value,
                        PhoneNumberValue = phoneNumber.PhoneNumberValue
                    });
                }

                this.appDbContext.Person.Add(person);
            }
            else
            {
                person = this.appDbContext.Person.SingleOrDefault(x => x.PersonId == vwPerson.PersonId);

                person.CountryId = vwPerson.CountryId;
                //person.CNIC = vwPerson.CNIC;
                person.Email = vwPerson.Email;
                //person.FatherHusbandName = vwPerson.FatherHusbandName;
                //person.PersonName = vwPerson.PersonName;
                person.NTN = vwPerson.NTN;
                person.OldCNIC = vwPerson.OldCNIC;

                this.appDbContext.Entry<Person>(person).State = EntityState.Modified;

                foreach (var address in vwPerson.Addresses)
                {
                    var addressObj = this.appDbContext.Address.SingleOrDefault(x => x.PersonId == vwPerson.PersonId && x.AddressTypeId == address.AddressTypeId);

                    if (addressObj is null)
                    {
                        this.appDbContext.Address.Add(new Address()
                        {
                            AddressDescription = address.AddressDescription,
                            AddressTypeId = address.AddressTypeId.Value,
                            CreatedBy = this.VwUser.UserId,
                            City = address.City,
                            DistrictId = address.DistrictId.Value,
                            PostalCode = address.PostalCode,
                            Person = person
                        });
                    }
                    else
                    {
                        addressObj.AddressDescription = address.AddressDescription;
                        addressObj.AddressTypeId = address.AddressTypeId.Value;
                        addressObj.City = address.City;
                        addressObj.DistrictId = address.DistrictId.Value;
                        addressObj.TehsilId = address.TehsilId;
                        addressObj.PostalCode = address.PostalCode;

                        this.appDbContext.Entry<Address>(addressObj).State = EntityState.Modified;
                    }
                }

                foreach (var phoneNumber in vwPerson.PhoneNumbers)
                {
                    var phoneNumberObj = this.appDbContext.PhoneNumber.SingleOrDefault(x => x.PersonId == vwPerson.PersonId && x.PhoneNumberTypeId == phoneNumber.PhoneNumberTypeId);

                    if (phoneNumberObj is null)
                    {
                        this.appDbContext.PhoneNumber.Add(new PhoneNumber()
                        {
                            CountryId = phoneNumber.CountryId,
                            CreatedBy = this.VwUser.UserId,
                            Person = person,
                            PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId.Value,
                            PhoneNumberValue = phoneNumber.PhoneNumberValue
                        });
                    }
                    else
                    {
                        phoneNumberObj.CountryId = phoneNumber.CountryId;
                        phoneNumberObj.PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId.Value;
                        phoneNumberObj.PhoneNumberValue = phoneNumber.PhoneNumberValue;

                        this.appDbContext.Entry<PhoneNumber>(phoneNumberObj).State = EntityState.Modified;
                    }
                }
            }

            await this.CommitTransaction();

            vwPerson.PersonId = person.PersonId;

            return person;
        }

        public async Task<DataSet> SaveVehiclePurchaseInfo(VwVehiclePurchaseInfo vwVehiclePurchaseInfo)
        {
            var vehiclePurchaseInfos = new List<VehiclePurchaseInfo>();
            var vehicleImportInfos = new List<VehicleImportInfo>();
            var vehicleAuctionInfos = new List<VehicleAuctionInfo>();

            if (vwVehiclePurchaseInfo.LocalPurchaseInfo is not null)
            {
                var vehiclePurchaseInfo = new VehiclePurchaseInfo();
                EntityMapper<VwVehicleLocalPurchaseInfo, VehiclePurchaseInfo>.CopyByPropertyNameAndType(vwVehiclePurchaseInfo.LocalPurchaseInfo, vehiclePurchaseInfo);
                vehiclePurchaseInfos.Add(vehiclePurchaseInfo);
            }

            if (vwVehiclePurchaseInfo.ImportInfo is not null)
            {
                var vehicleImportInfo = new VehicleImportInfo();
                EntityMapper<VwVehicleImportInfo, VehicleImportInfo>.CopyByPropertyNameAndType(vwVehiclePurchaseInfo.ImportInfo, vehicleImportInfo);
                vehicleImportInfos.Add(vehicleImportInfo);
            }

            if (vwVehiclePurchaseInfo.AuctionInfo is not null)
            {
                var vehicleAuctionInfo = new VehicleAuctionInfo();
                EntityMapper<VwVehicleAuctionInfo, VehicleAuctionInfo>.CopyByPropertyNameAndType(vwVehiclePurchaseInfo.AuctionInfo, vehicleAuctionInfo);
                vehicleAuctionInfos.Add(vehicleAuctionInfo);
            }

            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", vwVehiclePurchaseInfo.ApplicationId);
            paramDict.Add("@VehicleId", vwVehiclePurchaseInfo.VehicleId);
            paramDict.Add("@VehiclePurchaseTypeId", vwVehiclePurchaseInfo.VehiclePurchaseTypeId);
            paramDict.Add("@AuctionInfo", vehicleAuctionInfos.ToDataTable());
            paramDict.Add("@ImportInfo", vehicleImportInfos.ToDataTable());
            paramDict.Add("@PurchaseInfo", vehiclePurchaseInfos.ToDataTable());
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NRT_SavePurchaseInfo]", paramDict);

            return ds;
        }

        public async Task<DataSet> SaveRemarks(VwRemarks vwRemarks)
        {
            var remarksList = new List<Remarks>();
            var remarks = new Remarks();
            EntityMapper<VwRemarks, Remarks>.CopyByPropertyNameAndType(vwRemarks, remarks);
            remarksList.Add(remarks);

            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", vwRemarks.ApplicationId);
            paramDict.Add("@Remarks", remarksList.ToDataTable());
            paramDict.Add("@UserId", this.VwUser.UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NRT_SaveRemarks]", paramDict);

            return ds;
        }
        
        public async Task<DataSet> SaveVehicleDocument(VwVehicleDocument vwVehicleDocument, long UserId)
        {
            var vehicleDocuments = new List<VehicleDocument>();

            vwVehicleDocument.VehicleDocumentDetail.ForEach(x =>
            {
                var vehicleDocument = new VehicleDocument();
                EntityMapper<VwVehicleDocumentDetail, VehicleDocument>.CopyByPropertyNameAndType(x, vehicleDocument);
                vehicleDocuments.Add(vehicleDocument);
            });

            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", vwVehicleDocument.ApplicationId);
            paramDict.Add("@VehicleId", vwVehicleDocument.VehicleId);
            paramDict.Add("@VehicleDocument", vehicleDocuments.ToDataTable());
            paramDict.Add("@UserId", UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NRT_SaveDocuments]", paramDict);

            return ds;
        }

        public async Task<DataSet> SaveHPA(VwHPA vwTempHPA, long UserId)
        {
            //var hPAs = new List<HPA>();
            //HPA hPA = new HPA() { CreatedBy = this.VwUser.UserId };

            //EntityMapper<VwHPA, HPA>.CopyByPropertyNameAndType(vwTempHPA, hPA);
            //hPAs.Add(hPA);

            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            //paramDict.Add("@HPA", hPAs.ToDataTable());
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
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NRT_SaveHPA]", paramDict);

            return ds;
        }

        public async Task<DataSet> SaveKeeper(VwKeeper vwTempKeeper, long UserId)
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
                    CreatedBy = UserId
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
            paramDict.Add("@UserId", UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[NRT_SaveKeeper]", paramDict);

            return ds;
        }


        public async Task<long> SaveApplicationPhase(VwApplicationPhaseChange vwApplicationPhaseChange)
        {
            SqlParameter[] sql = new SqlParameter[2];
            sql[0] = new SqlParameter("@ApplicationId", vwApplicationPhaseChange.ApplicationId);
            sql[1] = new SqlParameter("@ApplicationStatusId", vwApplicationPhaseChange.BusinessEventId);
            sql[2] = new SqlParameter("@UserRoles", string.Join(",", this.VwUser.UserRoles.Select(x => x.RoleId)));

            var rowsAffected = await this.adoNet.ExecuteNonQuery("[Core].[SaveApplicationStatus]", sql);

            return rowsAffected;
        }

        #endregion

    }

}
