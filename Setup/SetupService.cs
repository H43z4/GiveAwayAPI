using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.DatabaseModels;
using Models.ViewModels.Setup;
//using Models.DatabaseModels.VehicleRegistration.Setup;
using RepositoryLayer;

namespace Setup
{
    public interface ISetupservice<T> where T : SetupBaseModel


    {
        public T Get(long Id);

        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null,
     Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string property = "", int? pageNo = null, int? recordsPerPage = null);

        public void AddSetup(vwSetup vsetup);

        public void UpdateSetup(vwSetup vsetup);

        public void DeleteSetup(long setupId);

        public void RemoveSetup(vwSetup vsetup);
        public void SaveChanges();

    }

    public class SetupService<T> : ISetupservice<T> where T : SetupBaseModel
    {
        
        private readonly AppDbContext _dbContext;
        private DbSet<T> entities;
        //private DbSet<T> entities;
        //private readonly AppDbContext _context;

        public SetupService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            entities = _dbContext.Set<T>();
        }


        public T Get(long Id)
        {
            return entities.SingleOrDefault(x => x.Id == Id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null,
             Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string property = "", int? pageNo = null, int? recordsPerPage = null)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();


            //return entities.AsEnumerable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (pageNo != null && recordsPerPage != null)
            {
                query = query.Skip((pageNo.Value - 1) * recordsPerPage.Value).Take(recordsPerPage.Value);
            }

            if (property != null)
            {
                foreach (var p in property.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(p);
                }
            }

            if (orderby != null)
            {
                return orderby(query);
            }
            else
            {
                return query.ToList();
            }
        }
        public void AddSetup(vwSetup vsetup) {

            
            if (vsetup==null)
                throw new ArgumentNullException("setup");
            T setup = (T)Activator.CreateInstance(typeof(T));

            setup.Name = vsetup.Name;
            setup.Abbreviation = vsetup.Abbreviation;
            setup.Description = vsetup.Description;

            setup.CreatedBy = vsetup.CreatedBy;
            setup.CreatedAt = DateTime.Now;
            
            entities.Add(setup);
            this.SaveChanges();
            
        }

        public void UpdateSetup(vwSetup vsetup) {
            if (vsetup == null)
                throw new ArgumentNullException("setup");

            //T setup = (T)Activator.CreateInstance(typeof(T));

            T setup = this.Get(vsetup.Id);
                if (setup == null)
                throw new ArgumentNullException("record not found exception.");

            setup.Name = vsetup.Name;
            setup.Abbreviation = vsetup.Abbreviation;
            setup.Description = vsetup.Description;

            entities.Update(setup);
            this.SaveChanges();
        }

        public void DeleteSetup(long setupId) {

            throw new NotImplementedException();
        }

        public void RemoveSetup(vwSetup vsetup) {
            if (vsetup == null)
                throw new ArgumentNullException("setup");

            T setup = this.Get(vsetup.Id);
            if (setup == null)
                throw new ArgumentNullException("record not found exception.");

            entities.Remove(setup);
            this.SaveChanges();
        }

        public IDictionary<long,string> GetDropDownData()
        {
            return null;
        }
        public void SaveChanges() {

            _dbContext.SaveChanges();
        }

    }
}
