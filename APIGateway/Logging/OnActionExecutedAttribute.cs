using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace APIGateway.Logging
{
    public class OnActionExecutedAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            if (context.HttpContext.Response.ContentType.Contains("json"))
            {
                context.HttpContext.Items["jsonbody"] = JsonConvert.SerializeObject(context.Result);
            }

            //context.HttpContext.Items["body"] = JsonConvert.SerializeObject(context.Result);
            //context.HttpContext.Items["jsonbody"] = JsonConvert.SerializeObject(context.Result);
        }
    }
}
