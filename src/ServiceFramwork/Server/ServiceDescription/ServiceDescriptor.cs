using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceFramwork.Server.ServiceDescription
{
    public class ServiceDescriptor
    {
        public static ServiceDescriptor[] FindServices(Type type)
        {
            List<ServiceDescriptor> list = new List<ServiceDescriptor>();
            TypeInfo typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsClass || typeInfo.IsAbstract)
            {
                return list.ToArray();
            }

            foreach (var interType in type.GetInterfaces())
            {
                var serviceAttr = interType.GetTypeInfo().GetCustomAttribute<ServiceAttribute>();
                if (serviceAttr != null)
                {
                    list.Add(new ServiceDescriptor(interType, type, serviceAttr));
                }

            }
            return list.ToArray();
        }

        public Type InterfaceType { get; private set; }
        public Type ServiceType { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<OperationDescriptor> Operations { get; private set; }

        internal ServiceDescriptor(Type interfaceType, Type serviceType, ServiceAttribute serviceAttr = null)
        {
            if (!interfaceType.GetTypeInfo().IsInterface)
            {
                throw new Exception("serviceType can be a interface type only.");
            }

            InterfaceType = interfaceType;
            ServiceType = serviceType;
            Name = serviceAttr.Name ?? interfaceType.Name;
            List<OperationDescriptor> operations = new List<OperationDescriptor>();
            foreach (var operation in interfaceType.GetTypeInfo().DeclaredMethods)
            {
                foreach (var operationAttr in operation.GetCustomAttributes<OperationAttribute>())
                {
                    operations.Add(new OperationDescriptor(this, operation, operationAttr));
                }
            }

            Operations = operations;
        }
    }

}
