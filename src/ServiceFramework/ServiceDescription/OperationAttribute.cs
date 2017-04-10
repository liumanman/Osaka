using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceFramework.ServiceDescription
{
    public class OperationAttribute : Attribute
    {
        public string Name { get; private set; }
        public bool IsIterator { get; private set; }

        public OperationAttribute(string name=null, bool isIterator=false)
        {
            Name = name;
        }
    }
}
