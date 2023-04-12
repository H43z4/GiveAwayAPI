using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Models.DatabaseModels.VehicleRegistration.Core;
using Models.ViewModels.VehicleRegistration.Core;
using RepositoryLayer;
using SharedLib;
using SharedLib.Common;

namespace VehicleRegistration
{
    public interface IOwnerService: SharedLib.Interfaces.BaseServiceInterface

    {
        
        public Owner Get(long Id);

        IEnumerable<Owner> GetAll(Expression<Func<Owner, bool>> filter = null,
     Func<IQueryable<Owner>, IOrderedQueryable<Owner>> orderby = null, string property = "", int? pageNo = null, int? recordsPerPage = null);

        public bool AddOwner(ICollection<VwOwner> owners);

        public bool UpdateOwner(ICollection<VwOwner> owners);

        public bool DeleteOwner(long[] owner_ids);

        public bool RemoveOwner(long[] owner_ids);

        public dynamic GetOwnersDropDowns();
        public void SaveChanges();


        
    }

    public class OwnerService : IOwnerService
    {
        private IRepository<Owner> _repository;

        
        //private DbSet<T> entities;
        //private readonly AppDbContext _context;

        public OwnerService(IRepository<Owner> repository)
        {
            _repository = repository;
           // entities = dbContext.Set<T>();
        }

        public int ProcessBy { get; set; }
        public Owner Get(long Id)
        {
            return _repository.Get<Owner>(x => x.OwnerId == Id);
        }

        public IEnumerable<Owner> GetAll(Expression<Func<Owner, bool>> filter = null,
Func<IQueryable<Owner>, IOrderedQueryable<Owner>> orderby = null, string property = "", int? pageNo = null, int? recordsPerPage = null)
        {

            //return _repository.GetEntities().Select(x=>x);

            return _repository.GetAll();
        }
        public bool AddOwner(ICollection<VwOwner> owners) {

            try
            {

                if (owners == null)
                    throw new NullReferenceException($"Owners data object are missing OR Incomplete.");

                
                foreach (var item in owners)
                {
                    
                    _repository.Insert(new Owner()
                    {
                        //CountryId = item.CountryId,
                        OwnerTypeId = item.OwnerTypeId,
                        OwnerTaxGroupId = item.OwnerTaxGroupId,
                        //CNIC = item.CNIC,
                        //Email = item.Email,
                        //FullName = item.FullName,
                        //NTN = item.NTN,
                        //OldCNIC = item.OldCNIC,
                        //PermanentAddress = item.PermanentAddress,
                        //PermanentCity = item.PermanentCity,
                        //PermanentDistrictId = item.PermanentDistrictId,
                        //PhoneNumber = item.PhoneNumber,
                        //PostalCode = item.PostalCode,
                        //PresentAddress = item.PresentAddress,
                        //PresentCity = item.PresentCity,
                        //PresentDistrictId = item.PresentDistrictId,
                        CreatedAt = CommonFunc.GetCurrentDateTime(),
                        CreatedBy = _repository.ProcessBy,

                    });
                }
                this.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public bool UpdateOwner(ICollection<VwOwner> owners) {

            try {
                if (owners == null)
                    throw new NullReferenceException($"Owners data object are missing OR Incomplete.");


                foreach (var item in owners)
                {
                    if(item == null)
                        throw new NullReferenceException($"Owners data object is Null OR Incomplete.");
                    if(item.OwnerId == 0)
                        throw new SharedLib.Exceptions.InvalidParameterValueException($"OwnerId is missing or 0 from Data Object");

                    var dbo = this.Get(item.OwnerId);
                    if (dbo == null)
                        throw new NullReferenceException($"No Data found against OwnerId = {item.OwnerId}.");

                    dbo.OwnerId = item.OwnerId;
                    //dbo.CountryId = item.CountryId;
                    dbo.OwnerTypeId = item.OwnerTypeId;
                    dbo.OwnerTaxGroupId = item.OwnerTaxGroupId;
                    //dbo.CNIC = item.CNIC;
                    //dbo.Email = item.Email;
                    //dbo.FullName = item.FullName;
                    //dbo.NTN = item.NTN;
                    //dbo.OldCNIC = item.OldCNIC;
                    //dbo.PermanentAddress = item.PermanentAddress;
                    //dbo.PermanentCity = item.PermanentCity;
                    //dbo.PermanentDistrictId = item.PermanentDistrictId;
                    //dbo.PhoneNumber = item.PhoneNumber;
                    //dbo.PostalCode = item.PostalCode;
                    //dbo.PresentAddress = item.PresentAddress;
                    //dbo.PresentCity = item.PresentCity;
                    //dbo.PresentDistrictId = item.PresentDistrictId;

                    dbo.CreatedAt = CommonFunc.GetCurrentDateTime();
                    dbo.CreatedBy = _repository.ProcessBy;

                }
                this.SaveChanges();
                return true;

            }
            catch
            {
                throw;
            }

        }

        public bool DeleteOwner(long[] owners) {

            try
            {
                throw new NotImplementedException("Not implemented");

                return true;
            }
            catch
            {

                return false;
            }
            //var dbo = _repository.GetAll(x => x.Id == setupId).FirstOrDefault();
            //if (dbo != null)
            //{
            //    _repository.Delete(dbo);
            //    this.SaveChanges();

            //    return true;
            //}

            //throw new NullReferenceException($"object not found against id = {setupId}");
        }

        public bool RemoveOwner(long[] owners) {

            try
            {
                foreach (long ownerid in owners)
                {

                    var dbo = this.Get(ownerid);
                    if (dbo != null)
                        _repository.Remove(dbo);
                    else
                        throw new NullReferenceException($"object not found against id = {ownerid}");
                }

                this.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        public void SaveChanges() {
            try
            {
                _repository.SaveChanges();
            }catch(Exception ex) {

                throw;
            }
        }


        public dynamic GetOwnersDropDowns (){

            
            var OwnerTypes = _repository.GetContext().OwnerType.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var OwnerTaxGroups = _repository.GetContext().OwnerTaxGroup.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var Countries = _repository.GetContext().Country.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var Districts = _repository.GetContext().District.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();

            
            return new
            {
                OwnerTypes = OwnerTypes,
                OwnerTaxGroups = OwnerTaxGroups,
                Countries = Countries,
                Districts = Districts
            };

        }


        class DataClasses
        {
            //public List<long,string> OwnerTypes { get; set; }
            //public List<Models.DatabaseModels.VehicleRegistration.Setup.OwnerTaxGroup> OwnerTaxGroups { get; set; }
            //public List<Models.DatabaseModels.VehicleRegistration.Setup.Country> Countries { get; set; }
            //public List<Models.DatabaseModels.VehicleRegistration.Setup.District> Districts { get; set; }

        }

    }
}
