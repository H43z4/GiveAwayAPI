using Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.ViewModels.Identity;
using RepositoryLayer;
using System;
using System.Threading.Tasks;

namespace APIGateway.Logging
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        private readonly ILoggingService _logService;

        private VwUser GetVwUser(HttpContext httpContext)
        {
            return (VwUser)httpContext.Items["User"];
        }

        public ErrorHandlingFilter(ILoggingService _logService)
        {
            this._logService = _logService;
        }

        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            
            string TraceId = string.IsNullOrEmpty(context.HttpContext.TraceIdentifier)?Guid.NewGuid().ToString():context.HttpContext.TraceIdentifier;

            this._logService.VwUser = this.GetVwUser(context.HttpContext);

            //--Log Exception
            Task.Run(async () =>
            {
                await this._logService.Log(new VLog()
                {
                    TraceId = TraceId,
                    LogType = LogTypes.Exception.ToString(),
                    Source = "AUTO-HANDLER",
                    Parameters = "",
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    CreatedDate = DateTime.Now,
                    RefType = null,
                    RefValue = null
                });

            });

            context.ExceptionHandled = false; //optional 
        }
    }
}
