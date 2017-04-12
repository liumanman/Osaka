using System;
using System.Collections;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Reflection;

using ServiceFramework.ServiceDescription;
using ServiceFramework.Serialization;

namespace ServiceFramework.Client
{
    class ServiceInvoker
    {
        IServiceConfiguration _configuration;
        IServicePathManager _pathManager;
        ISerializer _serializer;
        public ServiceInvoker(IServiceConfiguration configuration, IServicePathManager pathManager, ISerializer serializer)
        {
            _configuration = configuration;
            _pathManager = pathManager;
            _serializer = serializer;
        }

        public object Invoke(OperationDescriptor operation, object[] parameters)
        {
            using (var client = new HttpClient())
            {
                var paramsHolder = operation.BoxParameterValues(parameters);
                var data = _serializer.Serialize(paramsHolder);
                using (var ms = new MemoryStream(data))
                {
                    var requestContent = new StreamContent(ms);
                    var url = GetServiceUrl(operation);
                    var requestTask = client.PostAsync(url, requestContent);
                    var readDataTask = requestTask.Result.Content.ReadAsByteArrayAsync();
                    var responseContent = readDataTask.Result;

                    if (operation.IsIterator)
                    {
                        return ReadFromIterator(operation.ReturnType, responseContent);
                    }
                    else
                    {
                        return _serializer.Deserialize(operation.ReturnType, responseContent);
                    }
                }
            }
        }

        public IEnumerable ReadFromIterator(Type returnType, byte[] content)
        {
            var elementType = returnType.GetGenericArguments()[0];
            int index = 0;
            while (index < content.Length)
            {
                //var dataLength = new ArraySegment<byte>(content, index, 4).ToArray();
                var dataLength = BitConverter.ToInt32(content, index);
                index += 4;
                var data = new ArraySegment<byte>(content, index, dataLength).ToArray();
                index += dataLength;

                yield return _serializer.Deserialize(elementType, data);
            }
        }


        private string GetServiceUrl(OperationDescriptor operation)
        {
            var pathPattern = _configuration.GetServicePathPattern();
            var host = _configuration.GetServiceHost();
            var port = _configuration.GetServicePort();

            var path = _pathManager.GetServicePath(operation.Service.Name, operation.Name, pathPattern);
            return $"http://{host}:{port}{path}";
        }
    }
}
