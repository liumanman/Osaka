﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceFramework.ServiceDescription
{
    public class ServiceAttribute : Attribute
    {
        public string Name { get; private set; }
        public ServiceAttribute(string name=null)
        {
            Name = name;
        }
    }
}
