using Microsoft.EntityFrameworkCore;
using Models.DatabaseModels;
using Models.DatabaseModels.Authentication;
using Models.DatabaseModels.Logging;
using Models.DatabaseModels.DSSDatabaseObjects.Setup;
using Models.DatabaseModels.DSSDatabaseObjects.Core;
using System.Linq;

namespace RepositoryLayer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Logs

            modelBuilder.Entity<HttpRequestLog>().ToTable("HttpRequestLog", "Logs");
            modelBuilder.Entity<SqlExceptionLog>().ToTable("SqlExceptionLog", "Logs");
            //modelBuilder.Entity<Log>().ToTable("Logs", "Logs");
            //modelBuilder.Entity<RequestLog>().ToTable("RequestLog", "Logs");

            #endregion

            #region Setup

            //modelBuilder.Entity<ApplicationProcessFlow>().HasIndex(x => new 
            //{ 
            //    x.CurrentApplicationStatusId, 
            //    x.CurrentBusinessPhaseId, 
            //    x.CurrentBusinessPhaseStatusId, 
            //    x.BusinessProcessId, 
            //    x.RoleId, 
            //    x.BusinessEventId,
            //    x.NextApplicationStatusId,
            //    x.NextBusinessPhaseId,
            //    x.NextBusinessPhaseStatusId
            //}).IsUnique();

            //modelBuilder.Entity<BusinessTableAccessLevel>().HasIndex(x => new 
            //{ 
            //    x.ApplicationStatusId, 
            //    x.BusinessPhaseId, 
            //    x.BusinessPhaseStatusId, 
            //    x.BusinessProcessId, 
            //    x.BusinessTableId, 
            //    x.RoleId,
            //    x.BusinessTableAccessId
            //}).IsUnique();

            modelBuilder.Entity<DSOrganization>().ToTable("DSOrganization", "Setup");
            modelBuilder.Entity<DSRole>().ToTable("DSRole", "Setup");
            modelBuilder.Entity<DSUser>().ToTable("DSUser", "Setup");
            modelBuilder.Entity<DSUserRole>().ToTable("DSUserRole", "Setup");

            modelBuilder.Entity<ProductType>().ToTable("ProductType", "Setup");
            modelBuilder.Entity<ProductSize>().ToTable("ProductSize", "Setup");
            modelBuilder.Entity<Product>().ToTable("Product", "Setup");
            modelBuilder.Entity<ProductDetail>().ToTable("ProductDetail", "Setup");

            #endregion
            #region Posts
            modelBuilder.Entity<Post>().ToTable("Post", "Setup");
            modelBuilder.Entity<Pictures>().ToTable("Pictures", "Setup");
            modelBuilder.Entity<Review>().ToTable("Review", "Setup");
            modelBuilder.Entity<Rating>().ToTable("Rating", "Setup");
            modelBuilder.Entity<Chats>().ToTable("Chats", "Setup");
            modelBuilder.Entity<Messages>().ToTable("Messages", "Setup");



            #endregion
            #region Core


            modelBuilder.Entity<DSPerson>().ToTable("DSPerson", "Core");
            modelBuilder.Entity<Country>().ToTable("Country", "Setup");
            modelBuilder.Entity<Category>().ToTable("Category", "Setup");
            modelBuilder.Entity<City>().ToTable("City", "Setup");
            modelBuilder.Entity<Profession>().ToTable("Profession", "Setup");
            #endregion


            #region General

            var entityTypes = modelBuilder.Model.GetEntityTypes();

            foreach (var relationship in entityTypes.SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }

            foreach (var entity in entityTypes)
            {
                entity.FindProperty("CreatedAt")?.SetDefaultValueSql("GETDATE()");
                entity.FindProperty("IsDeleted")?.SetDefaultValueSql("0");
            }

            #endregion
        }

        

        #region Authentication

        public DbSet<GlobalRight> GlobalRight { get; set; }
        public DbSet<Resource> Resource { get; set; }
        public DbSet<ResourceAction> ResourceAction { get; set; }
        public DbSet<ResourceController> ResourceController { get; set; }
        public DbSet<ResourceType> ResourceType { get; set; }
        public DbSet<ResourceTypeDefaultRight> ResourceTypeDefaultRight { get; set; }
        public DbSet<ResourceTypeRight> ResourceTypeRight { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RoleAppProcessFlow> RoleAppProcessFlow { get; set; }
        public DbSet<RoleResource> RoleResource { get; set; }
        public DbSet<RoleResourceRight> RoleResourceRight { get; set; }
        public DbSet<RoleResourceRightsHistory> RoleResourceRightsHistory { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<UserRoleHistory> UserRoleHistory { get; set; }
        public DbSet<UserStatus> UserStatus { get; set; }
        public DbSet<UserType> UserType { get; set; }
        public DbSet<DSUser> DSUser{ get; set; }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Pictures> Pictures { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<Chats> Chat { get; set; }
        public DbSet<Messages> Message { get; set; }

        #endregion

        #region Logs

        public DbSet<HttpRequestLog> HttpRequestLog { get; set; }
        public DbSet<SqlExceptionLog> SqlExceptionLog { get; set; }
        //public DbSet<RequestLog> RequestLogs { get; set; }
        //public DbSet<Log> Logs { get; set; }

        #endregion

        




    }
}
