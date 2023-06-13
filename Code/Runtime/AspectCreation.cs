#nullable enable

using System;
using System.Linq;
using System.Reflection;
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
                if (typeof(Component).IsAssignableFrom(property.PropertyType))
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
}