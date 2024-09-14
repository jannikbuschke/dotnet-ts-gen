module TinyTypeGen.DynamicType

open System
open System.Reflection
open System.Reflection.Emit

let createBuilder (name: string) =
  let assemblyName = AssemblyName("DynamicAssembly")

  let assemblyBuilder =
    AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)

  let moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule")
  let typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public)
  typeBuilder

let createProperty (propertyName: string, propertyType: Type) (typeBuilder: TypeBuilder) =
  let fieldBuilder =
    typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private)

  let propertyBuilder =
    typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null)

  let getMethodBuilder =
    typeBuilder.DefineMethod(
      "get_" + propertyName,
      MethodAttributes.Public
      ||| MethodAttributes.SpecialName
      ||| MethodAttributes.HideBySig,
      propertyType,
      Array.empty<Type>
    )

  let getILGenerator = getMethodBuilder.GetILGenerator()
  getILGenerator.Emit(OpCodes.Ldarg_0)
  getILGenerator.Emit(OpCodes.Ldfld, fieldBuilder)
  getILGenerator.Emit(OpCodes.Ret)

  let setMethodBuilder =
    typeBuilder.DefineMethod(
      "set_" + propertyName,
      MethodAttributes.Public
      ||| MethodAttributes.SpecialName
      ||| MethodAttributes.HideBySig,
      null,
      [| propertyType |]
    )

  let setILGenerator = setMethodBuilder.GetILGenerator()
  setILGenerator.Emit(OpCodes.Ldarg_0)
  setILGenerator.Emit(OpCodes.Ldarg_1)
  setILGenerator.Emit(OpCodes.Stfld, fieldBuilder)
  setILGenerator.Emit(OpCodes.Ret)

  propertyBuilder.SetGetMethod(getMethodBuilder)
  propertyBuilder.SetSetMethod(setMethodBuilder)
