using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models;
using Models.DatabaseModels;
using RepositoryLayer;

namespace ServiceLayer
{
    public class Students_UOF : IDisposable
    {
        private readonly AppDbContext context;// new AppDbContext();


        private GenericRepository<Student> studentRepo;
        private GenericRepository<StudentDept> studentDeptRepo;


        public Students_UOF(AppDbContext context)
        {
            this.context = context;
        }
        public GenericRepository<Student> DepartmentRepository
        {
            get
            {

                if (this.studentRepo == null)
                {
                    this.studentRepo = new GenericRepository<Student>(context);
                }
                return studentRepo;
            }
        }

        public GenericRepository<StudentDept> CourseRepository
        {
            get
            {

                if (this.studentDeptRepo == null)
                {
                    this.studentDeptRepo = new GenericRepository<StudentDept>(context);
                }
                return studentDeptRepo;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            //Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
