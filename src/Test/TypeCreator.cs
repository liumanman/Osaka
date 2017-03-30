using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace ConsoleApp2
{
    public class CreateType
    {
        public class MyAttribute : Attribute
        {
            public string Name { get; private set; }
            public MyAttribute()
            {
            }
        }

        public static Type DynamicCreateType()
        {
            //动态创建程序集  
            AssemblyName DemoName = new AssemblyName("DynamicAssembly");
            //AssemblyBuilder dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(DemoName, AssemblyBuilderAccess.Run);
            var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(DemoName, AssemblyBuilderAccess.Run);
            //动态创建模块  
            //ModuleBuilder mb = dynamicAssembly.DefineDynamicModule(DemoName.Name, DemoName.Name + ".dll");
            var mb = dynamicAssembly.DefineDynamicModule(DemoName.Name);
            //动态创建类MyClass  
            TypeBuilder tb = mb.DefineType("MyClass", TypeAttributes.Public);

            var attrCtorParams = new Type[] { };
            var attrCtorInfo = typeof(SerializableAttribute).GetConstructor(attrCtorParams);
            var attrBuilder = new CustomAttributeBuilder(attrCtorInfo, new object[] { });
            tb.SetCustomAttribute(attrBuilder);
            //动态创建字段  
            FieldBuilder fb = tb.DefineField("myField", typeof(System.String), FieldAttributes.Private);
            FieldBuilder fb_Name = tb.DefineField("_name", typeof(string), FieldAttributes.Private);
            FieldBuilder fb_Age = tb.DefineField("_age", typeof(int), FieldAttributes.Private);
            ////动态创建构造函数  
            //Type[] clorType = new Type[] { typeof(System.String) };
            //ConstructorBuilder cb1 = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, clorType);
            ////生成指令  
            //ILGenerator ilg = cb1.GetILGenerator();//生成 Microsoft 中间语言 (MSIL) 指令  
            //ilg.Emit(OpCodes.Ldarg_0);
            //ilg.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            //ilg.Emit(OpCodes.Ldarg_0);
            //ilg.Emit(OpCodes.Ldarg_1);
            //ilg.Emit(OpCodes.Stfld, fb);
            //ilg.Emit(OpCodes.Ret);
            //动态创建属性 - name 
            PropertyBuilder pb = tb.DefineProperty("Name", PropertyAttributes.HasDefault, typeof(string), null);
            //PropertyBuilder pb2 = tb.DefineProperty("Age", PropertyAttributes.HasDefault, typeof(int), null);

            MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            MethodBuilder nameGetMethod = tb.DefineMethod("get_Name",
                                                          getSetAttr,
                                                          typeof(string),
                                                          Type.EmptyTypes);
            ILGenerator nameGetIL = nameGetMethod.GetILGenerator();
            nameGetIL.Emit(OpCodes.Ldarg_0);
            nameGetIL.Emit(OpCodes.Ldfld, fb_Name);
            nameGetIL.Emit(OpCodes.Ret);

            MethodBuilder nameSetMethod = tb.DefineMethod("set_Name",
                                                          getSetAttr,
                                                          null,
                                                          new Type[] { typeof(string) });
            ILGenerator nameSetIL = nameSetMethod.GetILGenerator();
            nameSetIL.Emit(OpCodes.Ldarg_0);
            nameSetIL.Emit(OpCodes.Ldarg_1);
            nameSetIL.Emit(OpCodes.Stfld, fb_Name);
            nameSetIL.Emit(OpCodes.Ret);

            pb.SetGetMethod(nameGetMethod);
            pb.SetSetMethod(nameSetMethod);


            //动态创建属性 - age 
            PropertyBuilder age_pb = tb.DefineProperty("Age", PropertyAttributes.HasDefault, typeof(int), null);

            MethodBuilder ageGetMethod = tb.DefineMethod("get_Age",
                                                          getSetAttr,
                                                          typeof(int),
                                                          Type.EmptyTypes);
            ILGenerator ageGetIL = ageGetMethod.GetILGenerator();
            ageGetIL.Emit(OpCodes.Ldarg_0);
            ageGetIL.Emit(OpCodes.Ldfld, fb_Age);
            ageGetIL.Emit(OpCodes.Ret);

            MethodBuilder ageSetMethod = tb.DefineMethod("set_Age",
                                                          getSetAttr,
                                                          null,
                                                          new Type[] { typeof(int) });
            ILGenerator ageSetIL = ageSetMethod.GetILGenerator();
            ageSetIL.Emit(OpCodes.Ldarg_0);
            ageSetIL.Emit(OpCodes.Ldarg_1);
            ageSetIL.Emit(OpCodes.Stfld, fb_Age);
            ageSetIL.Emit(OpCodes.Ret);

            age_pb.SetGetMethod(ageGetMethod);
            age_pb.SetSetMethod(ageSetMethod);


            //动态创建方法  
            MethodAttributes getSetAttr2 = MethodAttributes.Public | MethodAttributes.SpecialName;
            MethodBuilder myMethod = tb.DefineMethod("get_Field", getSetAttr2, typeof(string), Type.EmptyTypes);

            //生成指令  
            ILGenerator numberGetIL = myMethod.GetILGenerator();
            numberGetIL.Emit(OpCodes.Ldarg_0);
            numberGetIL.Emit(OpCodes.Ldfld, fb);
            numberGetIL.Emit(OpCodes.Ret);
            //使用动态类创建类型  
            Type classType = tb.CreateTypeInfo().AsType();
            //保存动态创建的程序集 (程序集将保存在程序目录下调试时就在Debug下)  
            //dynamicAssembly.Save(DemoName.Name + ".dll");
            //创建类  
            return classType;
        }
    }
}