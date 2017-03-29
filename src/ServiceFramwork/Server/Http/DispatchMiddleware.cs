using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ServiceFramwork.Server.ServiceDescription;

namespace ServiceFramwork.Server.Http
{
    public class DispatchMiddleware
    {
        private const string QUERY_KEY_SERVICE = "s";
        private const string QUERY_KEY_OPERATION = "o";
        private const string HEADER_KEY_SERVICE = "service";
        private const string HEADER_KEY_OPERATION = "operation";

        private RequestDelegate _next;
        //private string _templates;
        private IServiceManager _serviceManager;
        private IServicePathManager _pathManager;
        public DispatchMiddleware(RequestDelegate next, IServiceManager serviceManager, IServicePathManager pathManager)
        {
            _next = next;
            //_templates = templates;
            _serviceManager = serviceManager;
            _pathManager = pathManager;
        }

        public async Task Invoke(HttpContext context)
        {
            OperationDescriptor operation;
            OperationDescriptor[] found = _serviceManager.MatchWithPath(context.Request.Path, _pathManager);
            if (found.Length > 1)
            {
                throw new Exception($"Multiple operations found by URL:{context.Request.Path}");
            }

            if (found.Length == 1)
            {
                operation = found[0];
            }
            else //found.length == 0
            {
                operation = GetFromHeader(context);
                if (operation == default(OperationDescriptor))
                {
                    operation = GetFromQuery(context);
                    if (operation == default(OperationDescriptor))
                    {
                        throw new Exception($"Can't find operation by URL:{context.Request.Path}");
                    }
                }
            }

            context.SetDispatchOperation(operation);
            await _next(context);
        }

        private OperationDescriptor GetFrom<T>(T keyValueCol, string key_service, string key_operation)
            where T : IEnumerable<KeyValuePair<string, StringValues>>
        {
            var serviceName = (from i in keyValueCol
                               where i.Key.Equals(key_service, StringComparison.OrdinalIgnoreCase)
                               select i.Value).SingleOrDefault();
            if (serviceName == default(StringValues))
            {
                return default(OperationDescriptor);
            }

            var operationName = (from i in keyValueCol
                                 where i.Key.Equals(key_operation, StringComparison.OrdinalIgnoreCase)
                                 select i.Value).SingleOrDefault();
            if (operationName == default(StringValues))
            {
                return default(OperationDescriptor);
            }

            return _serviceManager.GetOperation(serviceName, operationName);
        }

        private OperationDescriptor GetFromQuery(HttpContext context)
        {
            return GetFrom(context.Request.Query, QUERY_KEY_SERVICE, QUERY_KEY_OPERATION);
        }

        private OperationDescriptor GetFromHeader(HttpContext context)
        {
            return GetFrom(context.Request.Headers, HEADER_KEY_SERVICE, HEADER_KEY_OPERATION);
        }
    }
}
