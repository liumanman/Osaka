using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceFramework.Client
{
    public interface IServiceConfiguration
    {
        string GetServiceHost();
        int GetServicePort();
        string GetServicePathPattern();
    }
}
