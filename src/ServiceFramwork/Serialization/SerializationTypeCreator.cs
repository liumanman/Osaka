using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ServiceFramwork.Serialization
{
    public class SerializationTypeCreator
    {
        private static ModuleBuilder ModuleBuilder;
        static SerializationTypeCreator()
        {
            var assemblyName = new AssemblyName("ParamsTypeAssembly");
            var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder = assembly.DefineDynamicModule(assemblyName.Name); 
        }

        private Dictionary<string, Type> _subTypes;
        private string _typeName;
        private TypeBuilder _typeBuilder;
        public SerializationTypeCreator(string typeName, Dictionary<string, Type> subTypes)
        {
            _subTypes = subTypes;
            _typeName = typeName;
        }

        private Dictionary<string, Type> TYPE_CACHE = new Dictionary<string, Type>();

        public Type Create()
        {
            if (!TYPE_CACHE.ContainsKey(_typeName))
            {
                _typeBuilder = ModuleBuilder.DefineType(_typeName, TypeAttributes.Public);
                CreateSerializableAttr();
                foreach(var t in _subTypes)
                {
                    CreateProperty(t.Key, t.Value); 
                }
                TYPE_CACHE[_typeName] = _typeBuilder.CreateTypeInfo().AsType();
            }

            return TYPE_CACHE[_typeName];
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
