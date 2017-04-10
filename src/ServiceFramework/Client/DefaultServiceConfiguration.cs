using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceFramework.Client
{
    class DefaultServiceConfiguration : IServiceConfiguration
    {
        public string GetServiceHost()
        {
            return "localhost";
        }

        public string GetServicePathPattern()
        {
            return "/test/abc/v2/{service}/{operation}";
        }

        public int GetServicePort()
        {
            return 5000;
        }
    }
}
