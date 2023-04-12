using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Models.DatabaseModels.VehicleRegistration.Setup;
using Models.DatabaseModels.VehicleRegistration.Core;
using Models.ViewModels.VehicleRegistration.Core;
using RepositoryLayer;
using SharedLib;
using SharedLib.Common;

namespace VehicleRegistration
{
    public interface IVehicleService: SharedLib.Interfaces.BaseServiceInterface

    {
        
        public Vehicle Get(long Id);

        IEnumerable<Vehicle> GetAll(Expression<Func<Vehicle, bool>> filter = null,
     Func<IQueryable<Vehicle>, IOrderedQueryable<Vehicle>> orderby = null, string property = "", int? pageNo = null, int? recordsPerPage = null);

        public bool AddVehicle(ICollection<VwVehicle> vehicles);

        public bool UpdateVehicle(ICollection<VwVehicle> vehicles);

        public bool DeleteVehicle(long[] vehicle_ids);

        public bool RemoveVehicle(long[] vehicle_ids);

        public  Task<dynamic> GetVehiclesDropDowns();
        public void SaveChanges();


        
    }

    public class VehicleService : IVehicleService
    {
        private IRepository<Vehicle> _repository;

        
        //private DbSet<T> entities;
        //private readonly AppDbContext _context;

        public VehicleService(IRepository<Vehicle> repository)
        {
            _repository = repository;
           // entities = dbContext.Set<T>();
        }

        public int ProcessBy { get; set; }
        public Vehicle Get(long Id)
        {
            return _repository.Get<Vehicle>(x => x.VehicleId == Id);
        }

        public IEnumerable<Vehicle> GetAll(Expression<Func<Vehicle, bool>> filter = null,
Func<IQueryable<Vehicle>, IOrderedQueryable<Vehicle>> orderby = null, string property = "", int? pageNo = null, int? recordsPerPage = null)
        {

            //return _repository.GetEntities().Select(x=>x);

            return _repository.GetAll();
        }
        public bool AddVehicle(ICollection<VwVehicle> vehicles) {

            try
            {

                if (vehicles == null)
                    throw new NullReferenceException($"vehicles data object are missing OR Incomplete.");

                
                foreach (var item in vehicles)
                {
                    Vehicle vehicle = new Vehicle();

                    var v = vehicle;

                    _repository.Insert(new Vehicle()
                    {
                        RegistrationNo = item.RegistrationNo,
                        RegistrationDistrictId = item.DistrictId.Value,
                        VehicleBodyConventionId = item.VehicleBodyConventionId,
                        VehicleBodyTypeId = item.VehicleBodyTypeId,
                        VehicleCategoryId = item.VehicleCategoryId,
                        VehicleClassId = item.VehicleClassId,
                        VehicleClassificationId = item.VehicleClassificationId,
                        VehicleColorId = item.VehicleColorId,
                        VehicleEngineTypeId = item.VehicleEngineTypeId,
                        VehicleFuelTypeId = item.VehicleFuelTypeId,
                        VehicleMakeId = item.VehicleMakeId,
                        VehicleMakerId = item.VehicleMakerId,
                        VehiclePurchaseTypeId = item.VehiclePurchaseTypeId,
                        //VehicleRCStatusId = item.VehicleRCStatusId,
                        //VehicleSchemeId = item.VehicleSchemeId,
                        VehicleStatusId = item.VehicleStatusId,
                        VehicleUsageId = item.VehicleUsageId,
                        CreatedAt = CommonFunc.GetCurrentDateTime(),
                        CreatedBy = _repository.ProcessBy

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

        public bool UpdateVehicle(ICollection<VwVehicle> vehicles) {

            try {
                if (vehicles == null)
                    throw new NullReferenceException($"vehicles data object are missing OR Incomplete.");


                foreach (var item in vehicles)
                {
                    if(item == null)
                        throw new NullReferenceException($"vehicles data object is Null OR Incomplete.");
                    if(item.VehicleId == 0)
                        throw new SharedLib.Exceptions.InvalidParameterValueException($"VehicleId is missing or 0 from Data Object");

                    var dbo = this.Get(item.VehicleId.Value);
                    if (dbo == null)
                        throw new NullReferenceException($"No Data found against VehicleId = {item.VehicleId}.");

                    
                    dbo.RegistrationNo = item.RegistrationNo;
                    dbo.RegistrationDistrictId = item.DistrictId.Value;
                    dbo.VehicleBodyConventionId = item.VehicleBodyConventionId;
                    dbo.VehicleBodyTypeId = item.VehicleBodyTypeId;
                    dbo.VehicleCategoryId = item.VehicleCategoryId;
                    dbo.VehicleClassId = item.VehicleClassId;
                    dbo.VehicleClassificationId = item.VehicleClassificationId;
                    dbo.VehicleColorId = item.VehicleColorId;
                    dbo.VehicleEngineTypeId = item.VehicleEngineTypeId;
                    dbo.VehicleFuelTypeId = item.VehicleFuelTypeId;
                    dbo.VehicleMakeId = item.VehicleMakeId;
                    dbo.VehicleMakerId = item.VehicleMakerId;
                    dbo.VehiclePurchaseTypeId = item.VehiclePurchaseTypeId;
                    //dbo.VehicleRCStatusId = item.VehicleRCStatusId;
                    //dbo.VehicleSchemeId = item.VehicleSchemeId;
                    dbo.VehicleStatusId = item.VehicleStatusId;
                    dbo.VehicleUsageId = item.VehicleUsageId;

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

        public bool DeleteVehicle(long[] vehicles) {

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

        public bool RemoveVehicle(long[] vehicles) {

            try
            {
                foreach (long ownerid in vehicles)
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

            _repository.SaveChanges();
        }


        public async Task<dynamic> GetVehiclesDropDowns (){

            //DataClasses dbo = new DataClasses();

            var Districts = _repository.GetContext().District.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList(); 
            var VehicleBodyConventions = _repository.GetContext().VehicleBodyConvention.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList(); 
            var VehicleBodyTypes = _repository.GetContext().VehicleBodyType.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var VehicleCategories = _repository.GetContext().VehicleCategory.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();

            var VehicleClasses = _repository.GetContext().VehicleClass.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var VehicleClassifications = _repository.GetContext().VehicleClassification.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var VehicleColors = _repository.GetContext().VehicleColor.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var VehicleEngineTypes = _repository.GetContext().VehicleEngineType.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var VehicleFuelTypes = _repository.GetContext().VehicleFuelType.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var VehicleMakes = _repository.GetContext().VehicleMake.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var VehicleMakers = _repository.GetContext().VehicleMaker.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var VehiclePurchaseTypes = _repository.GetContext().VehiclePurchaseType.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var VehicleRCStatuses = _repository.GetContext().VehicleRCStatus.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var VehicleSchemes = _repository.GetContext().VehicleScheme.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            
            var VehicleStatuses = _repository.GetContext().VehicleStatus.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();
            var VehicleUsages = _repository.GetContext().VehicleUsage.Select(x => new { Value = x.Id, Display = x.Name }).AsNoTracking().ToList();


            return new
            {
                District = Districts,
                VehicleBodyConvention = VehicleBodyConventions,
                VehicleBodyType = VehicleBodyTypes,
                VehicleCategory = VehicleCategories,
                VehicleClass = VehicleClasses,
                VehicleClassification = VehicleClassifications,
                VehicleColor = VehicleColors,
                VehicleEngineType = VehicleEngineTypes,
                VehicleFuelType = VehicleFuelTypes,
                VehicleMake = VehicleMakes,
                VehicleMaker = VehicleMakers,
                VehiclePurchaseType = VehiclePurchaseTypes,
                VehicleRCStatus = VehicleRCStatuses,
                VehicleScheme = VehicleSchemes,
                VehicleStatus = VehicleStatuses,
                VehicleUsage = VehicleUsages
            };


        }


        class DataClasses
        {
            //public ICollection<Models.DatabaseModels.VehicleRegistration.Setup.Vehicletype> VehicleTypes { get; set; }
            //public ICollection<Models.DatabaseModels.VehicleRegistration.Setup.VehicleTaxGroup> VehicleTaxGroups { get; set; }
            //public ICollection<Models.DatabaseModels.VehicleRegistration.Setup.Country> Countries { get; set; }
            //public ICollection<Models.DatabaseModels.VehicleRegistration.Setup.District> Districts { get; set; }

        }

    }
}
