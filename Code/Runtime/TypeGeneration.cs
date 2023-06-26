#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Dev.ComradeVanti.GameObjectAspect
{
    internal static class TypeGeneration
    {
        private const TypeAttributes ImplementationTypeAttributes =
            TypeAttributes.Public | TypeAttributes.Class;

        private const MethodAttributes GetSetMethodAttributes =
            MethodAttributes.Public | MethodAttributes.SpecialName |
            MethodAttributes.HideBySig;

        private static bool IsConventionalInterfaceName(string interfaceName) =>
            interfaceName.StartsWith("I") &&
            interfaceName.Length >= 2 &&
            char.IsUpper(interfaceName[1]);

        // ReSharper disable once SuggestBaseTypeForParameter
        private static string TypeNameFor(Type interfaceType)
        {
            var interfaceName = interfaceType.Name;
            return IsConventionalInterfaceName(interfaceName)
                ? interfaceName[1..] + "Implementation" // Skip the leading I
                : interfaceName + "Implementation";
        }


        // ReSharper disable once SuggestBaseTypeForParameter
        private static string FieldNameFor(PropertyInfo property) =>
            $"_{property.Name}";

        private static bool IsPropertyTypeSupported(Type propertyType) =>
            !propertyType.IsValueType && propertyType != typeof(object);

        public static Type? TryGenerateImplementationType<T>(ModuleBuilder moduleBuilder)
            where T : class, IGameObjectAspect
        {
            var interfaceType = typeof(T);

            // Must be interface
            if (!interfaceType.IsInterface) return null;

            var allInterfaceTypes =
                interfaceType.GetInterfaces()
                    .Prepend(interfaceType)
                    .ToArray();

            // Must not have methods
            var allMethods = allInterfaceTypes
                .SelectMany(type => type.GetMethods())
                .Where(method => !method.IsSpecialName && method.Name != "Equals");
            if (allMethods.Any()) return null;

            // Must not have events 
            var allEvents = allInterfaceTypes.SelectMany(type => type.GetEvents());
            if (allEvents.Any()) return null;

            var typeName = TypeNameFor(interfaceType);
            var typeBuilder = moduleBuilder.DefineType(
                typeName, ImplementationTypeAttributes,
                typeof(object), new[] {interfaceType});

            FieldBuilder AddBackingFieldFor(PropertyInfo property) =>
                typeBuilder!.DefineField(
                    FieldNameFor(property), property.PropertyType,
                    FieldAttributes.Private);

            PropertyBuilder AddPropertyFor(PropertyInfo property, MethodBuilder getter, MethodBuilder setter)
            {
                var propertyBuilder = typeBuilder!.DefineProperty(property.Name, PropertyAttributes.HasDefault,
                    property.PropertyType, null);

                propertyBuilder.SetGetMethod(getter);
                propertyBuilder.SetSetMethod(setter);

                typeBuilder.DefineMethodOverride(getter, property.GetMethod);

                return propertyBuilder;
            }

            MethodBuilder AddGetterFor(PropertyInfo property, FieldInfo backingField)
            {
                var getterBuilder = typeBuilder!.DefineMethod(property.GetMethod.Name,
                    GetSetMethodAttributes | MethodAttributes.Virtual, property.PropertyType, Type.EmptyTypes);

                var getterIlGenerator = getterBuilder.GetILGenerator();

                getterIlGenerator.Emit(OpCodes.Ldarg_0);
                getterIlGenerator.Emit(OpCodes.Ldfld, backingField);
                getterIlGenerator.Emit(OpCodes.Ret);

                return getterBuilder;
            }

            MethodBuilder AddSetterFor(PropertyInfo property, FieldInfo backingField)
            {
                var setterBuilder = typeBuilder!.DefineMethod($"set_{property.Name}",
                    GetSetMethodAttributes, null, new[] {property.PropertyType});

                var setterIlGenerator = setterBuilder.GetILGenerator();

                setterIlGenerator.Emit(OpCodes.Ldarg_0);
                setterIlGenerator.Emit(OpCodes.Ldarg_1);
                setterIlGenerator.Emit(OpCodes.Stfld, backingField);
                setterIlGenerator.Emit(OpCodes.Ret);

                return setterBuilder;
            }

            bool TryAddProperty(PropertyInfo property)
            {
                if (!IsPropertyTypeSupported(property.PropertyType)) return false;

                var backingFieldBuilder = AddBackingFieldFor(property);
                var getterBuilder = AddGetterFor(property, backingFieldBuilder);
                var setterBuilder = AddSetterFor(property, backingFieldBuilder);
                _ = AddPropertyFor(property, getterBuilder, setterBuilder);

                return true;
            }

            var allProperties = allInterfaceTypes.SelectMany(type => type.GetProperties());
            if (!allProperties.All(TryAddProperty)) return null;

            typeBuilder.AddInterfaceImplementation(interfaceType);
            return typeBuilder.CreateType();
        }
    }
}