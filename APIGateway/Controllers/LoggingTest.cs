using APIGateway.Logging;
using Logging;
using Microsoft.AspNetCore.Mvc;
using Models.DatabaseModels.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingTestController : Controller
    {
        private readonly ILogService _logger;

        public LoggingTestController(ILogService logger)
        {
            _logger = logger;
            //HttpActionContext 
        }

        // GET: api/<logging_test>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            try
            {
                //throw new Exception("Test exception for log");
            }
            catch (Exception ex)
            {
                _logger.Log(RequestConfiguration.GetRequestTraceId(this.HttpContext), ex);
            }
            return new string[] { "value1", "value2" };
        }


        // GET api/<logging_test>/5
        [HttpGet("{id}")]
        public string Get(long id)
        {
            string LogEntry = "";
            try
            {

                //FromBodyAttribute
                //_logger.Log()
                //_logger.Log("information")

            }
            catch
            {


            }
            return LogEntry;
        }

        // POST api/<logging_test>
        [HttpPost]
        public async Task<JsonResult> Post(Models.DatabaseModels.Logging.ILog data)
        {

            return Json(new ReturnData()
            {

                status = "sucess",
                message = "working fine."
            });

        }

        // PUT api/<logging_test>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<logging_test>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public class ReturnData
        {

            public string status { get; set; }
            public string message { get; set; }
        }
    }
}


/*


 */