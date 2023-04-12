using Models.ViewModels.DSAuth.Setup;
using RepositoryLayer;
using SharedLib.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace Setup
{
    public interface ILovService : ICurrentDSUser
    {
        Task<DataSet> GetCitiesLOV();
        Task<DataSet> GetCountriesLOV();
        Task<DataSet> GetDistrictsLOV();
        Task<DataSet> GetManufacturersLOV();
        Task<DataSet> GetProductsLOV();
        Task<DataSet> GetProductUnitsLOV();
        Task<DataSet> GetProfessionsLOV();
    }
    public class LovService : ILovService
    {
        readonly AppDbContext appDbContext;
        readonly IAdoNet adoNet;
        readonly IDBHelper dbHelper;
        public VwDSUser VwUser { get; set; }
        public VwDSUser VwDSUser { get; set; }

        public LovService(AppDbContext appDbContext, IAdoNet adoNet, IDBHelper dbHelper)
        {
            this.appDbContext = appDbContext;
            this.adoNet = adoNet;
            this.dbHelper = dbHelper;
        }

        public async Task<DataSet> GetCitiesLOV()
        {
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Setup].[GetCitiesLOV]", null);

            return ds;
        }

        public async Task<DataSet> GetCountriesLOV()
        {
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Setup].[GetCountriesLOV]", null);

            return ds;
        }

        public async Task<DataSet> GetDistrictsLOV()
        {
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Setup].[GetDistrictsLOV]", null);

            return ds;
        }

        public async Task<DataSet> GetManufacturersLOV()
        {
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Setup].[GetManufacturersLOV]", null);

            return ds;
        }

        public async Task<DataSet> GetProductsLOV()
        {
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Setup].[GetProductsLOV]", null);

            return ds;
        }

        public async Task<DataSet> GetProductUnitsLOV()
        {
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Setup].[GetProductUnitsLOV]", null);

            return ds;
        }
        
        public async Task<DataSet> GetProfessionsLOV()
        {
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Setup].[GetProfessionsLOV]", null);

            return ds;
        }
    }
}
