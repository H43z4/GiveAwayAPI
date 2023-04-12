using Microsoft.EntityFrameworkCore;
using Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RepositoryLayer
{
    public interface IRepository<T> where T : BaseModel
    {
        public int ProcessBy { get; set; }
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null,
             Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string property="", int? pageNo = null, int? recordsPerPage = null);

        T Get(long id);

        T Get<T>(Expression<Func<T, bool>> predicate) where T : class;

        void Insert(T entity);

        void Update(T entity);

        void Delete(T entity);

        //void SoftDelete(T entity);

        void Remove(T entity);
        void SaveChanges();

        public DbSet<T> GetEntities();

        public AppDbContext GetContext();
    }
}
