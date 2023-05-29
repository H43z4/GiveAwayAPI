using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using RepositoryLayer;
using UserManagement;
using Logging;
using Setup;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Authentication.JwtStatelessToken;
using Authentication;
using APIGateway.Middleware;
using Wkhtmltopdf.NetCore;
using Person;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http.Features;
using PostManagement;
using ReviewAndRating;
using ChatManagement;

namespace APIGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SharedLib.Configuration.RabbitMQServerConfig>(Configuration.GetSection("RabbitMQConfig"));


            services.AddCors();

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(Logging.OnActionExecutedAttribute));
            });

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);
            services.AddTransient<IAdoNet>(x => new AdoNet(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IDBHelper>(x => new DBHelper(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ITokenService, Authentication.JwtStatelessToken.TokenService>();
            //services.AddTransient<ILog, LogService>();
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddTransient<IUserManagement, UserManagement.UserManagement>();
            #region Setup-Services

         

            services.AddTransient(typeof(ISetupservice<>), typeof(SetupService<>));
           

            #endregion


            #region DonationSystem-Services
            services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<ILovService, LovService>();
            services.AddTransient<IPostManagementService, PostManagementService>();
            services.AddTransient<IReviewServise, ReviewServise>();
            services.AddTransient<IChatServise, ChatServices>();

            #endregion
            services.AddStatelessTokenAuthentication();

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            //services.AddControllers().AddNewtonsoftJson(options => { options.SerializerSettings. });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIGateway", Version = "v1" });
            });

            services.Configure<ApiBehaviorOptions>(o =>
            {
                o.InvalidModelStateResponseFactory = actionContext =>
                    new BadRequestObjectResult(ApiResponse.GetValidationErrorResponse(ApiResponseType.VALIDATION_ERROR, actionContext.ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage), null, null, null));
            });
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<HttpLoggingMiddleware>();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseSwagger();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "swagger";
            });

            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Attachments")),
            //    RequestPath = new PathString("/Attachments")
            //});
            app.UseRouting();


            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("/index.html");
            });
          
        }
    }
}
