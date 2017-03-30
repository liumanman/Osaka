using System;
using System.Collections.Generic;
using System.Reflection;
using ServiceFramwork.Serialization;

namespace ServiceFramwork.Server.ServiceDescription
{
    public class OperationDescriptor
    {
        public string Name { get; private set; }
        public ServiceDescriptor Service { get; private set; }
        public MethodInfo Operation { get; private set; }
        public ParameterInfo[] Parameters { get { return Operation.GetParameters(); } }
        public Type ReturnType { get { return Operation.ReturnType; } }
        public Type ParamsSerializationType { get; private set; }

        //public string[] URLPatterns { get; private set; }

        public OperationDescriptor(ServiceDescriptor service, MethodInfo operationInfo, OperationAttribute operationAttr)
        {
            this.Service = service;
            this.Operation = operationInfo;
            this.Name = operationAttr.Name ?? operationInfo.Name;

            var paramTable = new Dictionary<string, Type>();
            foreach(var p in this.Parameters)
            {
                paramTable.Add(p.Name, p.ParameterType);
            }
            string paramsSerializationTypeName = $"{Service.Name}_{Name}"; 
            ParamsSerializationType = new SerializationTypeCreator(paramsSerializationTypeName, paramTable).Create();
        }

        public object[] UnboxParameterValues(object serializationTypeValue)
        {
            object[] values = new object[Parameters.Length];
            for(int i = 0; i < values.Length; i++)
            {
                values[i] = ParamsSerializationType.GetProperty(Parameters[i].Name).GetValue(serializationTypeValue);
            }
            return values;
        }

        public object BoxParameterValues(params object[] values)
        {
            //object valueHolder = ParamsSerializationType.GetConstructor(Type.EmptyTypes).Invoke(null);
            object valueHolder = Activator.CreateInstance(ParamsSerializationType);
            for(int i = 0; i < values.Length; i++)
            {
                ParamsSerializationType.GetProperty(Parameters[i].Name).SetValue(valueHolder, values[i]);
            }
            return valueHolder;
        }

    }
}
