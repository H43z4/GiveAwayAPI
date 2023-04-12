using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Logging
{
    public enum RequestLogTypes
    {

        Request = 0,
        Response = 1,
        Exception = 2
    }

    public enum LogTypes
    {
        Information = 0,
        Warning = 1,
        Exception = 2,
        General = 3
    }


    public class VLog
    {
        public long Id { get; set; }
        [Required]
        public string TraceId { get; set; }
        [Required]
        public string LogType { get; set; }
        [Required]
        public string Source { get; set; }
        public string Parameters { get; set; }
        [Required]
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RefType { get; set; }
        public string RefValue { get; set; }
    }
    public class RequestMapper
    {
        public string TraceId = null;
        public string Method = null;
        public string IP = null;
        public string Agenet = null;


        public IHeaderDictionary Headers = null;
        public string Body = null;
        public QueryString QueryString;
        public IDictionary<string, string> Form = null;

        public string Action;
        public IDictionary<string, object> Arguments = null;

        public string RefType { get; set; }
        public string RefValue { get; set; }

    }

    public class ResponseMapper
    {
        public string Action = null;
        public string TraceId = null;
        public string result = null;
        public object ResultObject = null;
        public string Body = null;
        public ResultType resultType = ResultType.Json;

        public enum ResultType
        {
            Json = 0,
            View = 1,
            Exception = 2,
            Other = 3
        }
    }

    public class ClsCommon
    {
        public static async Task<string> ConvertAttributesToJson<T>(T entity)
        {


            //JsonSerializerOptions options= new JsonSerializerOptions();
            try
            {
                return JsonConvert.SerializeObject(entity);
            }
            catch
            {

                throw;
            }
        }

        public static async Task<string> LogError(Exception ex)
        {
            string errorId = Guid.NewGuid().ToString();


            try
            {
                Task.Run(async () =>
                {
                    //--Handle Error
                });

            }
            catch (Exception e)
            {
                throw e;
            }

            return errorId;
        }

        //public string ConvertAttributesToJson(Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary viewData)
        //{
        //    throw new NotImplementedException();
        //}

        public static string GetTraceId()
        {
            return Guid.NewGuid().ToString();

        }
    }
}
