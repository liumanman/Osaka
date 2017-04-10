using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceFramework.ServiceDescription
{
    public interface IServicePathManager
    {
        bool IsMatch(string path, string pattern);
        string GetServicePath(string serviceName, string operationName, string pattern);
    }
}
