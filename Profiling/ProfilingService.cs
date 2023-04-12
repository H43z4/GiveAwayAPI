using Models.ViewModels.Identity;
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
using Models.ViewModels.VehicleRegistration.Core;
using Models.DatabaseModels.VehicleRegistration.Core;

namespace Profiling
{
    public interface IProfilingService : ICurrentUser
    {
        Task<DataSet> GetPersonByCNIC(string cnic);
        Task<VwBusiness> GetBusinessByTaxNumber(string ntn, string ftn, string stn);
        Task<Person> SavePerson(VwPerson vwPerson);
        Task<Business> SaveBusiness(VwBusiness vwBusiness);
    }

    public class ProfilingService : IProfilingService
    {
        readonly AppDbContext appDbContext;
        readonly IAdoNet adoNet;

        public VwUser VwUser { get; set; }

        public ProfilingService(AppDbContext appDbContext, IAdoNet adoNet)
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


        #region public-Methods

        public async Task<DataSet> GetPersonByCNIC(string cnic)
        {
            SqlParameter[] sql = new SqlParameter[2];
            sql[0] = new SqlParameter("@PersonId", DBNull.Value);
            sql[1] = new SqlParameter("@CNIC", cnic);

            var ds = await this.adoNet.ExecuteUsingDataAdapter("[Core].[GetPerson]", sql);

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

        #endregion

        #region Public-Save-Methods

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

        public async Task<Business> SaveBusiness(VwBusiness vwBusiness)
        {
            Business business;

            if (vwBusiness.BusinessId == 0)
            {
                business = new Business()
                {
                    BusinessName = vwBusiness.BusinessName,
                    BusinessRegNo = vwBusiness.BusinessRegNo,
                    BusinessSectorId = vwBusiness.BusinessSectorId.Value,
                    BusinessStatusId = vwBusiness.BusinessStatusId.Value,
                    BusinessTypeId = vwBusiness.BusinessTypeId.Value,
                    Email = vwBusiness.Email,
                    FTN = vwBusiness.FTN,
                    NTN = vwBusiness.NTN,
                    STN = vwBusiness.STN,
                    CreatedBy = this.VwUser.UserId
                };

                foreach (var address in vwBusiness.Addresses)
                {
                    this.appDbContext.Address.Add(new Address()
                    {
                        AddressDescription = address.AddressDescription,
                        AddressTypeId = address.AddressTypeId.Value,
                        CreatedBy = this.VwUser.UserId,
                        City = address.City,
                        DistrictId = address.DistrictId.Value,
                        PostalCode = address.PostalCode,
                        Business = business
                    });
                }

                foreach (var phoneNumber in vwBusiness.PhoneNumbers)
                {
                    this.appDbContext.PhoneNumber.Add(new PhoneNumber()
                    {
                        CountryId = phoneNumber.CountryId,
                        CreatedBy = this.VwUser.UserId,
                        Business = business,
                        PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId.Value,
                        PhoneNumberValue = phoneNumber.PhoneNumberValue
                    });
                }

                this.appDbContext.Business.Add(business);
            }
            else
            {
                business = this.appDbContext.Business.SingleOrDefault(x => x.BusinessId == vwBusiness.BusinessId);

                business.BusinessName = vwBusiness.BusinessName;
                business.BusinessRegNo = vwBusiness.BusinessRegNo;
                business.BusinessSectorId = vwBusiness.BusinessSectorId.Value;
                business.BusinessStatusId = vwBusiness.BusinessStatusId.Value;
                business.BusinessTypeId = vwBusiness.BusinessTypeId.Value;
                business.Email = vwBusiness.Email;
                business.FTN = vwBusiness.FTN;
                business.NTN = vwBusiness.NTN;
                business.STN = vwBusiness.STN;

                this.appDbContext.Entry<Business>(business).State = EntityState.Modified;

                foreach (var address in vwBusiness.Addresses)
                {
                    var addressObj = this.appDbContext.Address.SingleOrDefault(x => x.BusinessId == vwBusiness.BusinessId && x.AddressTypeId == address.AddressTypeId);

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
                            Business = business
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

                foreach (var phoneNumber in vwBusiness.PhoneNumbers)
                {
                    var phoneNumberObj = this.appDbContext.PhoneNumber.SingleOrDefault(x => x.BusinessId == vwBusiness.BusinessId && x.PhoneNumberTypeId == phoneNumber.PhoneNumberTypeId);

                    if (phoneNumberObj is null)
                    {
                        this.appDbContext.PhoneNumber.Add(new PhoneNumber()
                        {
                            CountryId = phoneNumber.CountryId,
                            CreatedBy = this.VwUser.UserId,
                            Business = business,
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

            vwBusiness.BusinessId = business.BusinessId;

            return business;
        }

        #endregion

    }

}
