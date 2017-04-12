using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace ServiceFramework.ServiceDescription
{
    public class ServiceDescriptor
    {
        public static bool IsService(Type type)
        {
            if (!type.GetTypeInfo().IsInterface) return false;
            return type.GetTypeInfo().GetCustomAttribute<ServiceAttribute>() != null;
        }

        public Type SchemaType { get; private set; }
        public string Name { get; private set; }
        public OperationDescriptor[] Operations { get; private set; }
        public Type ImplType { get; set; }

        public ServiceDescriptor(Type serviceSchema, Type serviceImpl = null)
        {
            if (!IsService(serviceSchema))
            {
                throw new Exception($"{serviceSchema.Name} is not a service schema");
            }

            ImplType = serviceImpl;

            var serviceAttr = serviceSchema.GetTypeInfo().GetCustomAttribute<ServiceAttribute>();

            SchemaType = serviceSchema;
            Name = serviceAttr.Name ?? serviceSchema.Name;
            List<OperationDescriptor> operationList = new List<OperationDescriptor>();
            foreach (var operation in serviceSchema.GetTypeInfo().DeclaredMethods)
            {
                foreach (var operationAttr in operation.GetCustomAttributes<OperationAttribute>())
                {
                    var od = new OperationDescriptor(this, operation, operationAttr);
                    if (operationList.Count(o => o.Name.Equals(od.Name, StringComparison.OrdinalIgnoreCase)) > 0)
                    {
                        throw new Exception($"Duplicate operation:{this.Name}.{od.Name}");
                    }
                    operationList.Add(new OperationDescriptor(this, operation, operationAttr));
                }
            }
            Operations = operationList.ToArray();
        }
    }
}
