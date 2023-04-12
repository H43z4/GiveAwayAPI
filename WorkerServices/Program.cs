using Biometric;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Profiling;
using RepositoryLayer;

namespace WorkerServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public IConfiguration Configuration { get; }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<SharedLib.Configuration.RabbitMQServerConfig>(hostContext.Configuration.GetSection("RabbitMQConfig"));

                    services.AddDbContext<AppDbContext>(options => options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Singleton);
                    services.AddTransient<IAdoNet>(x => new AdoNet(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                    services.AddTransient<IProfilingService, ProfilingService>();
                    services.AddHostedService<RabbitMQBioAssociationService>();
                    //services.AddHostedService<Worker>();
                });
    }
}
