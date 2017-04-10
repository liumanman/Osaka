using System;
using System.Collections.Generic;
using System.Text;

using ServiceFramework.ServiceDescription;
using ServiceFramework.Serialization;

namespace ServiceFramework.Client
{
    public class ServiceInvokerBuilder
    {
        static IServiceConfiguration _customConfiguration;
        public static void SetCustomConfiguration(IServiceConfiguration configuration)
        {
            _customConfiguration = configuration;
        }

        internal ServiceInvoker Build()
        {
            var configuration = _customConfiguration ?? new DefaultServiceConfiguration();
            var pathManager = new DefaultServicePathManager($"http://{configuration.GetServiceHost()}:{configuration.GetServicePort()}");
            var serializer = new JsonSerializer();
            return new ServiceInvoker(configuration, pathManager, serializer);
        }
    }
}
