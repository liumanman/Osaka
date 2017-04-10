using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceFramework.Client
{
    public class ServiceFactory
    {
        public static T CreateInstance<T>()
        {
            var serviceProxyType = new ServiceProxyCreator(typeof(T)).Create();
            return (T)Activator.CreateInstance(serviceProxyType);
        }
    }
}
