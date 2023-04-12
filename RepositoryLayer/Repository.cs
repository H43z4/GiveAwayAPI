using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
//using System.Data.Entity;

namespace RepositoryLayer
{
    public class Repository<T> : IRepository<T> where T : BaseModel
    {
        private readonly AppDbContext _dbContext;
        private DbSet<T> entities;

        public int ProcessBy { get; set; }
        public Repository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            entities = _dbContext.Set<T>();
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Remove(entity);
            _dbContext.SaveChanges();

        }


        #region Get Function
        public T Get(long id)
        {
            //throw new NotImplementedException();
            //var entry = _dbContext.Entry(T);
            //var primaryKey = entry.Metadata.FindPrimaryKey();

            //this.GetAll()
            //return entities.SingleOrDefault(x => x.Id == id);

            return null;
        }

        public T Get<T>(Expression<Func<T, bool>> predicate)
    where T : class
        {
            T item = null;
            item = _dbContext.Set<T>().FirstOrDefault(predicate);

            return item;
        }

        //public DbRawSqlQuery<T> SQLQuery<T>(string sql, params object[] parameters)
        //{
        //    return this._dbContext.Database.SqlQuery<T>(sql, parameters);
        //    _dbContext.Database.sq
        //}

        //public IEnumerable<T> GetAll(int RecordsPerPage, int currentPage)
        //{
        //    return entities.AsEnumerable();
        //}

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null,
             Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string property = "", int? pageNo = null, int? recordsPerPage = null)
        { 
        IQueryable<T> query = _dbContext.Set<T>().AsQueryable();


            //return entities.AsEnumerable();

            if (filter != null)
            {
                query = query.Where(filter);
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

        #endregion
        public void Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Add(entity);
            _dbContext.SaveChanges();
        }

        

        public void Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Remove(entity);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entities.Update(entity);
            _dbContext.SaveChanges();
        }

        public DbSet<T> GetEntities()
        {

            return entities;
        }

        public AppDbContext GetContext()
        {

            return this._dbContext;
        }

    }
}
