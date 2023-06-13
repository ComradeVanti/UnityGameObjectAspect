#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace Dev.ComradeVanti.GameObjectAspect
{
    internal static class AspectCreation
    {
        internal static T? TryCreateAspect<T>(GameObject gameObject, Type implementationType)
            where T : class, IGameObjectAspect
        {
            GameObject? TryResolveGameObject(PropertyInfo property)
            {
                // TODO: Allow resolving parent or child game-objects
                return gameObject;
            }

            Component? TryResolveSingleComponent(PropertyInfo property)
            {
                // TODO: Allow resolving parent or child components
                var component = gameObject.GetComponent(property.PropertyType);

                return !component ? null : component;
            }

            object? TryResolvePropertyValue(PropertyInfo property)
            {
                if (property.PropertyType == typeof(GameObject))
                    return TryResolveGameObject(property);
                if (typeof(Component).IsAssignableFrom(property.PropertyType) || property.PropertyType.IsInterface)
                    return TryResolveSingleComponent(property);
                throw new ArgumentException("The given implementation type contains invalid properties!");
            }

            bool TrySetProperty(T instance, PropertyInfo property)
            {
                var value = TryResolvePropertyValue(property);
                if (value == null) return false;
                property.SetValue(instance, value);
                return true;
            }

            var instance = (T) Activator.CreateInstance(implementationType);
            return implementationType
                .GetProperties()
                .All(property => TrySetProperty(instance, property))
                ? instance
                : null;
        }
    }

    public static class AspectCreationExt
    {
        private const string AssemblyName = "AspectImplementations";


        private static readonly AssemblyBuilder assemblyBuilder =
            AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(AssemblyName), AssemblyBuilderAccess.Run);

        private static readonly ModuleBuilder moduleBuilder =
            assemblyBuilder.DefineDynamicModule(AssemblyName);

        private static readonly IDictionary<Type, Type> implementationTypes =
            new Dictionary<Type, Type>();


        public static T? TryAspect<T>(this GameObject gameObject)
            where T : class, IGameObjectAspect
        {
            var interfaceType = typeof(T);
            if (!implementationTypes.ContainsKey(interfaceType))
            {
                var maybeImplementationType = TypeGeneration.TryGenerateImplementationType<T>(moduleBuilder);
                if (maybeImplementationType == null) return null;
                implementationTypes.Add(interfaceType, maybeImplementationType);
            }

            var implementationType = implementationTypes[interfaceType];
            return AspectCreation.TryCreateAspect<T>(gameObject, implementationType);
        }

        public static T? TryAspect<T>(this Component component)
            where T : class, IGameObjectAspect =>
            component.gameObject.TryAspect<T>();

        public static T? TryAspect<T>(this IGameObjectAspect aspect)
            where T : class, IGameObjectAspect =>
            // TODO: Reuse components already cached in aspect
            aspect.GameObject.TryAspect<T>();
    }
}