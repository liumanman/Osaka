using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ServiceFramwork.Server.ServiceDescription
{
    public class ParamsTypeCreator
    {
        private static ModuleBuilder ModuleBuilder;
        static ParamsTypeCreator()
        {
            var assemblyName = new AssemblyName("ParamsTypeAssembly");
            var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder = assembly.DefineDynamicModule(assemblyName.Name); 
        }

        private OperationDescriptor _operation;
        private TypeBuilder _typeBuilder;
        public ParamsTypeCreator(OperationDescriptor operation)
        {
            _operation = operation;
        }

        public Type Create()
        {
            _typeBuilder = ModuleBuilder.DefineType($"{_operation.Service.Name}_{_operation.Name}"
                , TypeAttributes.Public);
            CreateSerializableAttr();
            foreach(var p in _operation.Parameters)
            {
                CreateProperty(p.Name, p.ParameterType);
            }

            return _typeBuilder.AsType();
        }

        private void CreateSerializableAttr()
        {
            var ctorInfo = typeof(SerializableAttribute).GetConstructor(Type.EmptyTypes);
            var attrBuilder = new CustomAttributeBuilder(ctorInfo, new object[] { });
            _typeBuilder.SetCustomAttribute(attrBuilder);
        }

        private void CreateProperty(string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = _typeBuilder.DefineField($"_{propertyName}", propertyType, FieldAttributes.Private);
            PropertyBuilder propertyBuilder = _typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);

            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            MethodBuilder getMethodBuilder = _typeBuilder.DefineMethod($"get_{propertyName}"
                , getSetAttr
                , propertyType
                , Type.EmptyTypes);
            ILGenerator getIL = getMethodBuilder.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);

            MethodBuilder setMethodBuilder = _typeBuilder.DefineMethod($"set_{propertyName}"
                , getSetAttr
                , null
                , new Type[] { propertyType });
            ILGenerator setIL = setMethodBuilder.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);
            setIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }
    }
}
