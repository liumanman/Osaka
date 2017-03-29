using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Linq;

using ServiceFramwork.Server.ServiceDescription;

namespace ServiceFramwork.Server.Http
{
    public class DefaultServiceManager : IServiceManager
    {
        private List<ServiceDescriptor> _collection;
        private List<OperationDescriptor> _allOperations;

        public DefaultServiceManager(string assemblyPath)
        {
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
            BuildServiceDescription(assembly.GetTypes());
        }

        public DefaultServiceManager(Type[] types)
        {
            BuildServiceDescription(types);
        }

        private void BuildServiceDescription(Type[] types)
        {
            _collection = new List<ServiceDescriptor>();
            _allOperations = new List<OperationDescriptor>();
            foreach (var type in types)
            {
                ServiceDescriptor[] sdCol = ServiceDescriptor.FindServices(type);
                foreach(var sd in sdCol)
                {
                    _collection.Add(sd);
                    _allOperations.AddRange(sd.Operations);
                }
            }
        }



        public OperationDescriptor[] GetAllOperations()
        {
            return _allOperations.ToArray();
        }

        public OperationDescriptor GetOperation(string serviceName, string operationName)
        {
            var od = _collection.Where(service => service.Name.Equals(serviceName))?.First().Operations
                      .Where(operation => operation.Name.Equals(operationName))?.First();
            return od;
        }

        public OperationDescriptor[] MatchWithPath(string url, IServicePathManager pathManager)
        {
            return _allOperations.Where(o => o.URLPatterns.Any(p => pathManager.IsMatch(url, p))).ToArray();
        }
    }
}
