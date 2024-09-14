using System.Reflection;
using System.Reflection.Emit;

namespace WebApplication2;

public static class Test
{
    public static Type CreateDynamicType()
    {
        AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
        TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicType", TypeAttributes.Public); 
        CreateProperty(typeBuilder, "HasActivityInPeriod", typeof(bool));
        Type t = typeBuilder.CreateType();
        return t;
    }
    
    public static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
    {
        FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

        PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

        MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + propertyName,
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
            propertyType, Type.EmptyTypes);

        ILGenerator getILGenerator = getMethodBuilder.GetILGenerator();
        getILGenerator.Emit(OpCodes.Ldarg_0);
        getILGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
        getILGenerator.Emit(OpCodes.Ret);

        MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + propertyName,
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
            null, new Type[] { propertyType });

        ILGenerator setILGenerator = setMethodBuilder.GetILGenerator();
        setILGenerator.Emit(OpCodes.Ldarg_0);
        setILGenerator.Emit(OpCodes.Ldarg_1);
        setILGenerator.Emit(OpCodes.Stfld, fieldBuilder);
        setILGenerator.Emit(OpCodes.Ret);

        propertyBuilder.SetGetMethod(getMethodBuilder);
        propertyBuilder.SetSetMethod(setMethodBuilder);
    }
    
}
