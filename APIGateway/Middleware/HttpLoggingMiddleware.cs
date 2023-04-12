using APIGateway.Logging;
using Logging;
using Microsoft.AspNetCore.Http;
using Models.DatabaseModels.Logging;
using Models.ViewModels.DSAuth.Setup;
using Models.ViewModels.Identity;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIGateway.Middleware
{
    public class HttpLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ILoggingService loggingService;

        private VwUser GetVwUser(HttpContext httpContext)
        {
            return (VwUser)httpContext.Items["User"];
        }
        private VwDSUser GetVwDSUser(HttpContext httpContext)
        {
            return (VwDSUser)httpContext.Items["User"];
        }
        public HttpLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILoggingService loggingService)
        {
            HttpRequestLog httpRequestLog = new HttpRequestLog();

            try
            {
                httpRequestLog.Method = context.Request.Method;
                httpRequestLog.Path = context.Request.PathBase + context.Request.Path;
                httpRequestLog.RequestHeaders = JsonConvert.SerializeObject(context.Request.Headers);
                httpRequestLog.ClientIP = context.Connection.RemoteIpAddress.ToString();

                context.Request.EnableBuffering();

                using (var reader = new StreamReader(
                    context.Request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 4096,
                    leaveOpen: true))
                {
                    httpRequestLog.RequestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                try
                {
                    await _next(context);

                    //httpRequestLog.ResponseBody = context.Response.ContentType.Contains("json") ? context.Items["body"] as string : String.Empty;
                    //httpRequestLog.ResponseBody = context.Items["body"] as string;
                    httpRequestLog.ResponseBody = context.Items["jsonbody"] as string;
                    httpRequestLog.ResponseHeaders = JsonConvert.SerializeObject(context.Response.Headers);
                    httpRequestLog.ResponseStatusCode = context.Response.StatusCode;

                }
                catch (Exception ex)
                {
                    httpRequestLog.IsExceptionRaised = true;
                    httpRequestLog.StackTrace = ex.StackTrace;

                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                    }

                    httpRequestLog.ExceptionMessage = ex.Message;
                    httpRequestLog.TraceId = Guid.NewGuid().ToString();

                    context.Response.StatusCode = 500;
                    //var responseBody = JsonConvert.SerializeObject(ApiResponse.GetExceptionGenericResponse(ApiResponseType.EXCEPTION, httpRequestLog.TraceId));
                    var responseBody = ApiResponse.GetExceptionGenericResponse(ApiResponseType.EXCEPTION, httpRequestLog.TraceId);
                    await context.Response.WriteAsJsonAsync(responseBody);
                }
                finally
                {
                    var currentUser = context.Items["User"] as VwDSUser;
                    httpRequestLog.CreatedBy = (long)(currentUser is not null ? currentUser.UserId : 1);

                    //context.Response.OnCompleted(async () =>
                    //{
                    //    await this.loggingService.LogHttpRequest(httpRequestLog);
                    //});

                    await loggingService.LogHttpRequest(httpRequestLog);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
            }
        }
    }
}
