using System;
using System.Collections.Generic;
using System.Reflection;

namespace ServiceFramwork.Server.ServiceDescription
{
    public class OperationDescriptor
    {
        public string Name { get; private set; }
        public ServiceDescriptor Service { get; private set; }
        public MethodInfo Operation { get; private set; }
        public ParameterInfo[] Parameters { get { return Operation.GetParameters(); } }
        public Type ReturnType { get { return Operation.ReturnType; } }
        public Type ParamsType { get; private set; }

        //public string[] URLPatterns { get; private set; }

        public OperationDescriptor(ServiceDescriptor service, MethodInfo operationInfo, OperationAttribute operationAttr)
        {
            this.Service = service;
            this.Operation = operationInfo;
            this.Name = operationAttr.Name ?? operationInfo.Name;

            //List<string> patterns = new List<string>() { $"{service.Name}/{Name}" };
            //if (operationAttr.URLPatterns != null)
            //{
            //    patterns.AddRange(operationAttr.URLPatterns);
            //}
            ////URLPatterns = new string[] { $"{service.Name}/{Name}" };
            //URLPatterns = patterns.ToArray();
            ParamsType = new ParamsTypeCreator(this).Create();
        }
    }
}
