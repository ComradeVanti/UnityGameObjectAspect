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
            var allMethods = allInterfaceTypes.SelectMany(type => type.GetMethods());
            if (allMethods.Any()) return null;

            // Must not have events 
            var allEvents = allInterfaceTypes.SelectMany(type => type.GetEvents());
            if (allEvents.Any()) return null;

            var typeName = TypeNameFor(interfaceType);
            var typeBuilder = moduleBuilder.DefineType(
                typeName, ImplementationTypeAttributes,
                typeof(object), new[] {interfaceType});

            bool TryAddProperty(PropertyInfo property)
            {
                return IsPropertyTypeSupported(property.PropertyType);
            }

            var allProperties = allInterfaceTypes.SelectMany(type => type.GetProperties());
            return allProperties.All(TryAddProperty)
                ? typeBuilder.CreateType()
                : null;
        }
    }
}