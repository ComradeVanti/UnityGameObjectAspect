#nullable enable
using System;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using UnityEngine;

namespace Dev.ComradeVanti.GameObjectAspect
{
    internal static class TestHelpers
    {
        internal static Type? TryGenerateTestImplementation<T>() where T : class, IGameObjectAspect
        {
            var testAssemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(
                    new AssemblyName("TestsAssembly"), AssemblyBuilderAccess.Run);

            var testModuleBuilder =
                testAssemblyBuilder.DefineDynamicModule("TestModule");

            return TypeGeneration.TryGenerateImplementationType<T>(testModuleBuilder);
        }

        internal static Type GenerateTestImplementation<T>() where T : class, IGameObjectAspect
        {
            var type = TryGenerateTestImplementation<T>();
            Assert.NotNull(type, "Implementation generation failed!");
            return type!;
        }
    }
}