using Models.DatabaseModels.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Logging
{
    public interface ILogService
    {
        public IEnumerable<Log> GetAllLogs(int RecordsPerPage, int currentPage);

        public Log GetLog(long Id);
        public void InsertLog(Log logData);

        public Task<bool> Log(VLog log);

        public Task<string> Log(string TraceId, Exception ex);

    }
}
