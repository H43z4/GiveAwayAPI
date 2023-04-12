using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DatabaseModels.Logging;

namespace Logging
{
    public interface IRequestLogService
    {
        public IEnumerable<RequestLog> GetAllRequestLogs(int RecordsPerPage, int currentPage);
        public RequestLog GetRequestLog(long Id);
        public void InsertRequestLog(RequestLog logData);

        public Task<bool> LogRequest(string logBySource, RequestMapper request);
        public Task<bool> LogResponse(string logBySource, ResponseMapper response);
    }
}

