using APIGateway.Logging;
using Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILog _logger;

        public LogController(ILog _logService)
        {
            _logger = _logService;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            try
            {
                throw new Exception("Test exception for log");
            }
            catch (Exception ex)
            {
                _logger.Log(RequestConfiguration.GetRequestTraceId(this.HttpContext), ex);
            }
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        public async Task<JsonResult> Post(Models.DatabaseModels.Logging.ILog data)
        {
            return new JsonResult(new
            {
                status = "sucess",
                message = "working fine."
            });
        }

    }
}
