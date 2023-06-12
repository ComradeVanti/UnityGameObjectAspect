#nullable enable

using System;

namespace Dev.ComradeVanti.GameObjectAspect
{
    internal static class TypeGeneration
    {
        public static Type? TryGenerateImplementationType<T>()
            where T : class, IGameObjectAspect
        {
            var aspectType = typeof(T);

            if (!aspectType.IsInterface) return null;

            return aspectType;
        }
    }
}