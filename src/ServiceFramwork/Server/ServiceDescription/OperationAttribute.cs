using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceFramwork.Server.ServiceDescription
{
    public class OperationAttribute : Attribute
    {
        public string Name { get; private set; }
        //public string[] URLPatterns { get; private set; }

        public OperationAttribute(string name=null)
        {
            Name = name;
            //URLPatterns = urlPatterns;
        }
    }
}
