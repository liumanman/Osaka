using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ServiceFramwork.Server.ServiceDescription;
using System.Text;

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
        private string _urlPattern;
        //private IServicePathManager _pathManager;
        public DispatchMiddleware(RequestDelegate next, IServiceManager serviceManager, string urlPattern)
        {
            _next = next;
            //_templates = templates;
            _serviceManager = serviceManager;
            _urlPattern = urlPattern;
            //_pathManager = pathManager;
        }

        private static bool TryParserUrl(string url, string urlPattern, out string serviceName, out string operationName)
        {
            serviceName = null;
            operationName = null;

            url = url.ToLower();
            urlPattern = urlPattern.ToLower();

            string pattern = Regex.Replace(urlPattern, @"{(\w+)}", @"(?<$1>\w+)");
            var m = Regex.Match(url, pattern);
            if (m == Match.Empty) return false;
            serviceName = m.Groups[HEADER_KEY_SERVICE]?.Value;
            operationName = m.Groups[HEADER_KEY_OPERATION]?.Value;
            return !String.IsNullOrEmpty(serviceName) && !String.IsNullOrEmpty(operationName);
        }

        //public async Task Invoke(HttpContext context)
        //{
        //    OperationDescriptor operation;
        //    OperationDescriptor[] found = _serviceManager.MatchWithPath(context.Request.Path, _pathManager);
        //    if (found.Length > 1)
        //    {
        //        throw new Exception($"Multiple operations found by URL:{context.Request.Path}");
        //    }

        //    if (found.Length == 1)
        //    {
        //        operation = found[0];
        //    }
        //    else //found.length == 0
        //    {
        //        operation = GetFromHeader(context);
        //        if (operation == default(OperationDescriptor))
        //        {
        //            operation = GetFromQuery(context);
        //            if (operation == default(OperationDescriptor))
        //            {
        //                throw new Exception($"Can't find operation by URL:{context.Request.Path}");
        //            }
        //        }
        //    }

        //    context.SetDispatchOperation(operation);
        //    await _next(context);
        //}

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Method.Equals("Post"))
            {
                await Next(context);
                return;
            }

            string serviceName, operationName;
            if (!TryParserUrl(context.Request.Path, _urlPattern, out serviceName, out operationName))
            {
                await Next(context);
                return;
            }

            var od = _serviceManager.GetOperation(serviceName, operationName);
            if (od == default(OperationDescriptor))
            {
                await Next(context);
                return;
            }

            var b = Encoding.UTF8.GetBytes($"service:{od.Service.Name}, operation:{od.Name}");
            context.Response.Body.Write(b, 0, b.Length);
        }

        private async Task Next(HttpContext context)
        {
            await _next(context);
        }

        private OperationDescriptor FindOperation(HttpContext context)
        {
            string serviceName, operationName;
            if (TryParserUrl(context.Request.Path, _urlPattern, out serviceName, out operationName))
            {
                return _serviceManager.GetOperation(serviceName, operationName);
            }
            else
            {
                return default(OperationDescriptor);
            }

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
