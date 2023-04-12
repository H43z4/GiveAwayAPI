using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Logging
{
    public class Mappers
    {
        public static async Task<RequestMapper> MapRequest(HttpRequest request)
        {

            RequestMapper mr = new RequestMapper();
            try { mr.Method = request.Method; } catch { }
            try { mr.Headers = request.Headers; } catch { mr.Headers = null; }

            try
            {
                using (var content = new StreamContent(request.Body))
                {
                    mr.Body = await content.ReadAsStringAsync();
                }

                //mr.Body = request.Body; 
            }
            catch { mr.Body = null; }
            try { mr.QueryString = request.QueryString; } catch { }
            try { mr.Action = request.Path.Value ?? ""; } catch { }

            try
            {
                if (request.HasFormContentType)
                    foreach (var item in request.Form)
                        mr.Form.Add(item.Key, item.Value);
            }
            catch { }

            return mr;
            //return new RequestMapper()
            //{
            //    Method = request.Method,
            //    Headers = request.Headers,
            //    //Body = request.Body,
            //    QueryString = request.QueryString,
            //    Form = request.Form
            //};
        }

        public static async Task<ResponseMapper> MapResponse(HttpResponse response, IActionResult result)
        {


            ResponseMapper rm = new ResponseMapper();

            try { rm.ResultObject = result; } catch (Exception ex) { rm.ResultObject = null; }

            try
            {
                using (var content = new StreamContent(response.Body))
                {
                    rm.Body = await content.ReadAsStringAsync();
                }

                //mr.Body = request.Body; 
            }
            catch { rm.Body = null; }

            //try { mr.Action = null; } catch { }


            //var result = (IActionResult)rm.ResultObject;
            if (result is JsonResult json)
            {
                try
                {
                    //object val = json.Value;
                    rm.result = JsonConvert.SerializeObject(json.Value);
                    rm.resultType = ResponseMapper.ResultType.Json;
                }
                catch (Exception ex)
                {
                    rm.result = "Error:" + ex.Message;
                    rm.resultType = ResponseMapper.ResultType.Exception;
                }
            }
            if (result is ViewResult view)
            {
                try
                {
                    // I think it's better to log ViewData instead of the finally rendered template string
                    rm.result = JsonConvert.SerializeObject(view.ViewData.Model);
                    rm.resultType = ResponseMapper.ResultType.View;
                }
                catch (Exception ex)
                {
                    rm.result = "Error:" + ex.Message;
                    rm.resultType = ResponseMapper.ResultType.Exception;
                }
            }

            else
            {
                try
                {
                    rm.result = JsonConvert.SerializeObject(result);
                    rm.resultType = ResponseMapper.ResultType.Other;
                }
                catch (Exception ex)
                {
                    rm.result = "Error:" + ex.Message;
                    rm.resultType = ResponseMapper.ResultType.Exception;
                }
            }

            return rm;

        }

    }
}
