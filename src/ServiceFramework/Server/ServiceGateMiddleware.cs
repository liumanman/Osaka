using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.IO;


using ServiceFramework.Serialization;
using ServiceFramework.ServiceDescription;

namespace ServiceFramework.Server
{
    public class ServiceGateMiddleware
    {
        private const string QUERY_KEY_SERVICE = "s";
        private const string QUERY_KEY_OPERATION = "o";
        private const string HEADER_KEY_SERVICE = "service";
        private const string HEADER_KEY_OPERATION = "operation";

        private RequestDelegate _next;
        private IServiceManager _serviceManager;
        private string _urlPattern;
        private ISerializer _serializer;
        public ServiceGateMiddleware(RequestDelegate next, IServiceManager serviceManager, string urlPattern, ISerializer serializer = null)
        {
            _next = next;
            _serviceManager = serviceManager;
            _urlPattern = urlPattern;
            _serializer = serializer == null ? new JsonSerializer() : serializer;
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

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Method.Equals("Post", StringComparison.OrdinalIgnoreCase))
            {
                await Next(context);
                return;
            }

            var od = this.FindOperation(context);
            if (od == default(OperationDescriptor))
            {
                await Next(context);
                return;
            }

            object[] paramsValues = this.ReadParams(context, od);

            var service = Activator.CreateInstance(od.Service.ImplType);
            var r = od.OperationInfo.Invoke(service, paramsValues);

            await WriteResponseStream(od,context.Response.Body, r);
        }

        private async Task WriteResponseStream(OperationDescriptor operationDescriptor, Stream stream, object content)
        {
            if (operationDescriptor.IsIterator)
            {
                await WriteResponseStreamWithBuffer(stream,  content as IEnumerable);
            }
            else
            {
                var b = _serializer.Serialize(content);
                await stream.WriteAsync(b, 0, b.Length);
            }

        }

        private async Task WriteResponseStreamWithBuffer(Stream stream, IEnumerable content)
        {
            int maxBufferSize = 10 * 1024 ;
            List<byte[]> buffer = new List<byte[]>();

            int currentSize = 0;
            foreach(object i in content as IEnumerable)
            {
                var b_data = _serializer.Serialize(i);
                var b_len = BitConverter.GetBytes(b_data.Length);

                if(currentSize > 0 && (currentSize + b_data.Length + b_len.Length) > maxBufferSize)
                {
                    foreach(var j in buffer)
                    {
                        await stream.WriteAsync(j, 0, j.Length);

                    }
                    buffer.Clear();
                    currentSize = 0;
                }

                buffer.Add(b_len);
                buffer.Add(b_data);
                currentSize += b_len.Length + b_data.Length;

                //Console.WriteLine(currentSize);
            }

            foreach(var j in buffer)
            {
                await stream.WriteAsync(j, 0, j.Length);
            }
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


        private object[] ReadParams(HttpContext context, OperationDescriptor descriptor)
        {
            byte[] parmasData;
            using (MemoryStream ms = new MemoryStream())
            {
                context.Request.Body.CopyTo(ms);
                parmasData = ms.ToArray();
            }

            var paramsHolder = _serializer.Deserialize(descriptor.ParamsSerializationType, parmasData);
            return descriptor.UnboxParameterValues(paramsHolder);
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
