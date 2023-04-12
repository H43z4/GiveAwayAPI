using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Logging;
using Models.ViewModels.Identity;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Logging
{
    public class InterceptionAttribute : ActionFilterAttribute
    {
        private readonly ILoggingService _logService;
        private readonly IConfiguration _configuration;

        public InterceptionAttribute(IConfiguration configuration, ILoggingService _logService)
        {
            _configuration = configuration;
            this._logService = _logService;
        }

        public override async void OnActionExecuting(ActionExecutingContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            RequestMapper rm = Mappers.MapRequest(actionContext.HttpContext.Request).Result;
            rm.Action = actionContext.ActionDescriptor.DisplayName;
            rm.Arguments = actionContext.ActionArguments;
            rm.TraceId = actionContext.HttpContext.TraceIdentifier = Guid.NewGuid().ToString();

            //Task.Run(async () =>
            //{
            //    await this._logService.LogRequest("AutoLog", rm);
            //});

            await this._logService.LogRequest("AutoLog", rm);
        }

        public override async void OnResultExecuted(ResultExecutedContext context)
        {
            //ResponseMapper rm = Mappers.MapResponse(context.HttpContext.Response, context.Result).Result;

            //rm.Action = context.ActionDescriptor.DisplayName;
            //rm.TraceId = context.HttpContext.TraceIdentifier;

            //this._logService.VwUser = this.GetVwUser(context.HttpContext);

            //await this._logService.LogResponse("AUTO-LOG", rm);

            //context.HttpContext.Items["body"] = JsonConvert.SerializeObject(((JsonResult)context.Result).Value);
            context.HttpContext.Items["body"] = JsonConvert.SerializeObject(context.Result);
        }
    }
}
