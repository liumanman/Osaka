﻿using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Linq;

using ServiceFramework.ServiceDescription;

namespace ServiceFramework.Server
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
                ServiceDescriptor[] sdCol = ServiceFinder.FindServices(type);
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
            var od = _collection.Where(service => service.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase)).SingleOrDefault()?.Operations
                      .Where(operation => operation.Name.Equals(operationName, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            return od;
        }
    }
}
