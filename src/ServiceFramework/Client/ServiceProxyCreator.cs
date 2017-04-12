using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using ServiceFramework.ServiceDescription;

namespace ServiceFramework.Client
{
    public class ServiceProxyCreator
    {
        private static ModuleBuilder ModuleBuilder;
        private static MethodInfo Invoker;
        static ServiceProxyCreator()
        {
            var assemblyName = new AssemblyName("ServiceProxyAssembly");
            var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder = assembly.DefineDynamicModule(assemblyName.Name);
            Invoker = typeof(ServiceProxyCreator).GetTypeInfo().DeclaredMethods.Where(m => m.Name == "ServiceInvoke").Single();
        }

        private static Dictionary<Type,Type> CACHE = new Dictionary<Type, Type>();

        private ServiceDescriptor _serviceDescriptor;
        private TypeBuilder _typeBuilder;
        private FieldBuilder _ServiceDescriptorFiledBuilder;

        internal ServiceProxyCreator(Type serviceSchema)
        {
            _serviceDescriptor = new ServiceDescriptor(serviceSchema);
            
            
        }

        public Type Create()
        {
            if (!CACHE.ContainsKey(_serviceDescriptor.SchemaType))
            {
                lock (CACHE)
                {
                    if (!CACHE.ContainsKey(_serviceDescriptor.SchemaType))
                    {
                        CACHE[_serviceDescriptor.SchemaType] = CreateNew();
                    }
                }
            }
            return CACHE[_serviceDescriptor.SchemaType];
        }

        private Type CreateNew()
        {
            _typeBuilder = ModuleBuilder.DefineType(
                _serviceDescriptor.SchemaType.Name,
                TypeAttributes.Public, 
                null, 
                new Type[] { _serviceDescriptor.SchemaType });

            CreateStaticMembers();

            foreach(var m in _serviceDescriptor.SchemaType.GetMethods())
            {
                //var od = _serviceDescriptor.Operations.Where(o => o.OperationInfo.Name == m.Name).SingleOrDefault();
                var od = _serviceDescriptor.Operations.GetByMethodInfo(m.Name, m.GetParameters().Select(p => p.ParameterType).ToArray());
                if (od == default(OperationDescriptor))
                {
                    CreateMethodImpl(_serviceDescriptor.SchemaType.Name, m);
                }
                else
                {
                    CreateMethodImpl(_serviceDescriptor.SchemaType.Name, od);
                }
            }

            //foreach(var m in _serviceDescriptor.Operations)
            //{
            //    CreateMethodImpl(_serviceDescriptor.SchemaType.Name, m);
            //}
            var proxyType = _typeBuilder.CreateTypeInfo().AsType();
            return SetServiceDescriptor(proxyType);
        }

        private Type SetServiceDescriptor(Type proxyType)
        {
            var filed = proxyType.GetField("ServiceDescriptor", BindingFlags.Static | BindingFlags.Public);
            filed.SetValue(null, _serviceDescriptor);
            return proxyType;
        }

        private void CreateStaticMembers()
        {
            _ServiceDescriptorFiledBuilder = _typeBuilder.DefineField("ServiceDescriptor",
                typeof(ServiceDescriptor),
                FieldAttributes.Public | FieldAttributes.Static);
        }

        private void CreateMethodImpl(string interfaceName, OperationDescriptor operationDescriptor)
        {
            MethodBuilder mb = _typeBuilder.DefineMethod(
                operationDescriptor.MethodInfo.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                operationDescriptor.ReturnType,
                (from p in operationDescriptor.Parameters
                 select p.ParameterType).ToArray());

            var ilc = mb.GetILGenerator();
            var localParams = ilc.DeclareLocal(typeof(object[]));
            //var localReturn = ilc.DeclareLocal(typeof(object));
            var localReturn = ilc.DeclareLocal(operationDescriptor.ReturnType);
            var argTypes = operationDescriptor.Parameters;

            ilc.Emit(OpCodes.Nop);
            ilc.Emit(OpCodes.Ldc_I4, argTypes.Length);
            ilc.Emit(OpCodes.Newarr, typeof(object));

            for (int i = 0; i < argTypes.Length; i++)
            {
                ilc.Emit(OpCodes.Dup);
                ilc.Emit(OpCodes.Ldc_I4, i);
                ilc.Emit(OpCodes.Ldarg, i + 1);
                Type argType = argTypes[i].ParameterType;
                if (argType.GetTypeInfo().IsValueType)
                {
                    ilc.Emit(OpCodes.Box, argType);
                }
                ilc.Emit(OpCodes.Stelem_Ref);
            }
            ilc.Emit(OpCodes.Stloc, localParams);

            ilc.Emit(OpCodes.Ldsfld, _ServiceDescriptorFiledBuilder);
            ilc.Emit(OpCodes.Ldstr, operationDescriptor.Name);
            ilc.Emit(OpCodes.Ldloc, localParams);

            ilc.Emit(OpCodes.Call, Invoker);
            if (operationDescriptor.ReturnType.GetTypeInfo().IsValueType)
            {
                ilc.Emit(OpCodes.Unbox_Any, operationDescriptor.ReturnType);
            }
            ilc.Emit(OpCodes.Stloc, localReturn);

            ilc.Emit(OpCodes.Nop);

            ilc.Emit(OpCodes.Ldloc, localReturn);

            //ilc.Emit(OpCodes.Ldc_I4_6);
            //ilc.Emit(OpCodes.Ldc_I4_4);
            //ilc.Emit(OpCodes.Add);

            ilc.Emit(OpCodes.Ret);

            _typeBuilder.DefineMethodOverride(mb, operationDescriptor.MethodInfo);
        }

        private void CreateMethodImpl(string interfaceName, MethodInfo methodDeclaration)
        {
            MethodBuilder mb = _typeBuilder.DefineMethod(
                methodDeclaration.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                methodDeclaration.ReturnType,
                (from p in methodDeclaration.GetParameters()
                 select p.ParameterType).ToArray());

            var ilc = mb.GetILGenerator();
            ilc.Emit(OpCodes.Nop);
            ilc.Emit(OpCodes.Ldstr, $"The method {interfaceName}.{methodDeclaration.Name} doesn't have Operation attribute.");
            ilc.Emit(OpCodes.Newobj, typeof(Exception).GetConstructor(new Type[] { typeof(string) }));
            ilc.Emit(OpCodes.Throw);
            _typeBuilder.DefineMethodOverride(mb, methodDeclaration);
        }

        public static object ServiceInvoke(ServiceDescriptor serviceDescriptor, string operationName, object[] @params)
        {
            var operation = serviceDescriptor.Operations.Where(op => op.Name == operationName).Single();
            var r = new ServiceInvokerBuilder().Build().Invoke(operation, @params);
            return r;
        }
    }
}
