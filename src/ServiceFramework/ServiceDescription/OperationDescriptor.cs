using System;
using System.Collections;
using System.Reflection;
using System.Linq;

using ServiceFramework.Serialization;

namespace ServiceFramework.ServiceDescription
{
    public class OperationDescriptor
    {
        public string Name { get; private set; }
        public ServiceDescriptor Service { get; private set; }
        public MethodInfo MethodInfo { get; private set; }
        public ParameterInfo[] Parameters { get { return MethodInfo.GetParameters(); } }
        public Type ReturnType { get { return MethodInfo.ReturnType; } }
        public Type ParamsSerializationType { get; private set; }
        public bool IsIterator { get; private set; }
        //public Type ReturnGenericype { get; private set; }
        //public bool IsReturnEnumerable { get; private set; }
        //public bool IsReturnHasGeneric { get; private set; }

        //public string[] URLPatterns { get; private set; }

        public OperationDescriptor(ServiceDescriptor service, MethodInfo operationInfo, OperationAttribute operationAttr)
        {
            this.Service = service;
            this.MethodInfo = operationInfo;
            this.Name = operationAttr.Name ?? operationInfo.Name;
            IsIterator = operationAttr.IsIterator;

            //IsReturnHasGeneric = operationInfo.ReturnType.GetGenericArguments().Length > 0 ; 
            
            //if(operationInfo.ReturnType != typeof(string) && (operationInfo.ReturnType == typeof(IEnumerable) ||
            //    operationInfo.ReturnType.GetTypeInfo().ImplementedInterfaces.Count(t => t == typeof(IEnumerable)) > 0))
            //{
            //    IsReturnEnumerable = true;
            //    if (!IsReturnHasGeneric)
            //    {
            //        if (operationAttr.ElementType == null)
            //        {
            //            throw new Exception($"The return type of method {service.Name}.{Name} is not a generic IEnumerable, please set ElementType in method attribute.");
            //        }
            //        else
            //        {
            //            ReturnElementType = operationAttr.ElementType;
            //        }
            //    }
            //    else
            //    {
            //        ReturnElementType = operationInfo.ReturnType.GetGenericArguments()[0];
            //    }
                
            //}


            //var paramTable = new Dictionary<string, Type>();
            //foreach (var p in this.Parameters)
            //{
            //    paramTable.Add(p.Name, p.ParameterType);
            //}
            //string paramsSerializationTypeName = $"{Service.Name}_{Name}";
            ParamsSerializationType = new SerializationTypeCreator(this).Create();
        }

        public object[] UnboxParameterValues(object serializationTypeValue)
        {
            object[] values = new object[Parameters.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = ParamsSerializationType.GetProperty(Parameters[i].Name).GetValue(serializationTypeValue);
            }
            return values;
        }

        public object BoxParameterValues(params object[] values)
        {
            //object valueHolder = ParamsSerializationType.GetConstructor(Type.EmptyTypes).Invoke(null);
            object valueHolder = Activator.CreateInstance(ParamsSerializationType);
            for (int i = 0; i < values.Length; i++)
            {
                ParamsSerializationType.GetProperty(Parameters[i].Name).SetValue(valueHolder, values[i]);
            }
            return valueHolder;
        }


    }

    public static class OperationDescriptorExtension
    {
        public static OperationDescriptor GetByMethodInfo(this OperationDescriptor[] operations, string methodName, Type[] paramsType)
        {
            return operations.Where(op => op.MethodInfo.Name == methodName && op.Parameters.Select(p => p.ParameterType).ToArray().Equals2(paramsType)).SingleOrDefault();
        }

        public static bool Equals2(this Type[] types1, Type[] types2)
        {
            types2 = types2 ?? Type.EmptyTypes;
            if (types1.Length != types2.Length) return false;

            for(int i = 0; i < types1.Length; i++)
            {
                if(types1[i] != types2[i])
                {
                    return false;
                }
            }

            return true;

        }

    }
}
