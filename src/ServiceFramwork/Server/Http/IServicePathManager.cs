using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceFramwork.Server.Http
{
    public interface IServicePathManager
    {
        bool IsMatch(string path, string pattern);
    }
}
