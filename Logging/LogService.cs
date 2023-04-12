using System;
using System.Collections.Generic;
using System.Threading.Tasks;

//using Microsoft.AspNetCore.Http;
////using Microsoft.AspNetCore.Mvc;

using Models.DatabaseModels.Logging;
using RepositoryLayer;

namespace Logging
{
    public class LogService : ILog // ILogService, IRequestLogService
    {
        private IRepository<Log> _iLogRepo;
        private IRepository<RequestLog> _iRequestLogRepo;
        private readonly AppDbContext _context;


        public LogService(AppDbContext context)
        {
            //_repository = repository;
            _context = context;
            _iLogRepo = new Repository<Log>(_context);
            _iRequestLogRepo = new Repository<RequestLog>(_context);
        }

        public IRepository<Log> ILogRepository
        {
            get
            {

                if (_iLogRepo == null)
                    _iLogRepo = new Repository<Log>(_context);

                return _iLogRepo;
            }
        }

        public IRepository<RequestLog> IRequestLogRepository
        {
            get
            {

                if (_iRequestLogRepo == null)
                    _iRequestLogRepo = new Repository<RequestLog>(_context);

                return _iRequestLogRepo;
            }
        }


        public void InsertRequestLog(RequestLog logData)
        {
            _iRequestLogRepo.Insert(logData);

        }

        public void InsertLog(Log logData)
        {
            _iLogRepo.Insert(logData);
        }

        public async Task<bool> LogRequest(string logBySource, RequestMapper request)
        {
            try
            {


                //Task.Delay(5000);
                string msg = await ClsCommon.ConvertAttributesToJson(request);
                string parameters = "";

                parameters = ClsCommon.ConvertAttributesToJson(request.Arguments).Result;

                this.IRequestLogRepository.Insert(new RequestLog()
                {
                    TraceId = request.TraceId,
                    RequestCategory = "NOT-DEFINED",
                    Method = request.Method,
                    IP = request.IP,
                    Agent = request.Agenet,
                    ActionName = request.Action,
                    Parameters = parameters,
                    Body = request.Body,
                    CreatedAt = DateTime.Now,
                    IsExceptionResponse = false,
                    Json = msg,
                    LogType = RequestLogTypes.Request.ToString(),

                    LogBySource = logBySource,
                    RefType = request.RefType,
                    RefValue = request.RefValue

                }); ;

                return true;
            }
            catch (Exception ex)
            {

                //--Log Exception 
                throw ex;
                //return false;
            }
        }

        public async Task<bool> LogResponse(string logBySource, ResponseMapper response)
        {
            try
            {


                //Task.Delay(5000);

                //string requestData = Task.Run(async () =>
                //{
                //    RequestMapper rm = await RequestMapper.MapRequest(request);
                //string msg = await ClsCommon.ConvertAttributesToJson(response);
                //    return msg;
                //}).Result;

                string parameters = "";
                //parameters = ClsCommon.ConvertAttributesToJson(request.Arguments).Result;

                this.IRequestLogRepository.Insert(new RequestLog()
                {
                    LogBySource = logBySource,
                    RequestCategory = "NOT-DEFINED",
                    ActionName = response.Action,
                    Body = response.Body,
                    CreatedAt = DateTime.Now,
                    IsExceptionResponse = false,
                    Parameters = null,
                    Json = response.result,
                    TraceId = response.TraceId,
                    LogType = RequestLogTypes.Response.ToString()
                });

                return true;
            }
            catch (Exception ex)
            {

                //--Log Exception 
                throw ex;
                //return false;
            }
        }
        public async Task<bool> Log(VLog log)
        {
            try
            {


                //Task.Delay(5000);



                this.ILogRepository.Insert(new Log()
                {
                    TraceId = log.TraceId,
                    LogType = log.LogType,
                    Source = log.Source,
                    Parameters = log.Parameters,
                    Message = log.Message,
                    StackTrace = log.StackTrace,
                    CreatedAt = DateTime.Now,
                    RefType = log.RefType,
                    RefValue = log.RefValue



                }); ;

                return true;
            }
            catch (Exception ex)
            {

                //--Log Exception 
                throw ex;
                //return false;
            }
        }

        public async Task<string> Log(string TraceId, Exception ex)
        {
            try
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                //Task.Delay(5000);

                string message = ex.Message;
                if (ex.InnerException != null)
                    message += Environment.NewLine + "INNER EXCEPTION: " + Environment.NewLine + ex.InnerException.Message;


                this.ILogRepository.Insert(new Log()
                {
                    TraceId = TraceId,
                    LogType = LogTypes.Exception.ToString(),
                    Source = "",
                    Parameters = null,
                    Message = message,
                    StackTrace = ex.StackTrace,
                    CreatedAt = DateTime.Now,
                    RefType = null,
                    RefValue = null

                });

                return TraceId;
            }
            catch (Exception exx)
            {

                //--Log Exception 
                throw ex;
                //return false;
            }
        }

        public IEnumerable<RequestLog> GetAllRequestLogs(int RecordsPerPage, int currentPage)
        {

            //_iRequestLogRepo.GetAll();
            throw new NotImplementedException();
        }

        public IEnumerable<Log> GetAllLogs(int RecordsPerPage, int currentPage)
        {
            //return _iLogRepo.GetAll();
            throw new NotImplementedException();
        }

        public Log GetLog(long Id)
        {

            //_iLogRepo.Get(id);
            throw new NotImplementedException();
        }

        public RequestLog GetRequestLog(long Id)
        {

            //_iRequestLogRepo.Get(id);
            throw new NotImplementedException();
        }



    }
}
