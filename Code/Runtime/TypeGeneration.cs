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

        private static bool IsConventionalInterfaceName(string interfaceName) =>
            interfaceName.StartsWith("I") &&
            interfaceName.Length >= 2 &&
            char.IsUpper(interfaceName[1]);

        private static string TypeNameFor(Type interfaceType)
        {
            var interfaceName = interfaceType.Name;
            return IsConventionalInterfaceName(interfaceName)
                ? interfaceName[1..] + "Implementation" // Skip the leading I
                : interfaceName + "Implementation";
        }

        private static bool IsPropertyTypeSupported(Type propertyType)
        {
            return !propertyType.IsValueType && propertyType != typeof(object);
        }

        public static Type? TryGenerateImplementationType<T>(ModuleBuilder moduleBuilder)
            where T : class, IGameObjectAspect
        {
            var interfaceType = typeof(T);

            if (!interfaceType.IsInterface) return null;

            var typeName = TypeNameFor(interfaceType);
            var typeBuilder = moduleBuilder.DefineType(
                typeName, ImplementationTypeAttributes,
                typeof(object), new[] {interfaceType});

            var allInterfaceTypes = interfaceType.GetInterfaces().Prepend(interfaceType);
            var allProperties = allInterfaceTypes.SelectMany(type => type.GetProperties());

            bool TryAddProperty(PropertyInfo property)
            {
                return IsPropertyTypeSupported(property.PropertyType);
            }

            return allProperties.All(TryAddProperty)
                ? typeBuilder.CreateType()
                : null;
        }
    }
}