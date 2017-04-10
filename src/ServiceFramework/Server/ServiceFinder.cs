using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

using ServiceFramework.ServiceDescription;

namespace ServiceFramework.Server
{
    public class ServiceFinder
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
                if (!ServiceDescriptor.IsService(interType)) continue;
                list.Add(new ServiceDescriptor(interType, type));
            }
            return list.ToArray();
        }

    }

}
