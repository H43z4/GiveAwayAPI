using Models.DatabaseModels.Logging;
using Models.ViewModels.Identity;
using RepositoryLayer;
using SharedLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public interface ILoggingService
    {
        Task<bool> LogHttpRequest(HttpRequestLog httpRequestLog);
    }

    public class LoggingService : ILoggingService
    {
        readonly AppDbContext appDbContext;

        public LoggingService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<bool> LogHttpRequest(HttpRequestLog httpRequestLog)
        {
            try
            {
                this.appDbContext.HttpRequestLog.Add(httpRequestLog);

                var rowsAffected = await this.appDbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
