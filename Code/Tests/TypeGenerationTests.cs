#nullable enable

using System;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Dev.ComradeVanti.GameObjectAspect
{
    public class TypeGenerationTests
    {
        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public abstract class ClassAspect : IGameObjectAspect
        {
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public interface IEmptyAspect : IGameObjectAspect
        {
        {
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        // ReSharper disable once InconsistentNaming
        public interface UnconventionalAspect : IGameObjectAspect
        {
        }


        private static Type? TryGenerate<T>() where T : class, IGameObjectAspect
        {
            var testAssemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(
                    new AssemblyName("TestsAssembly"), AssemblyBuilderAccess.Run);

            var testModuleBuilder =
                testAssemblyBuilder.DefineDynamicModule("TestModule");

            return TypeGeneration.TryGenerateImplementationType<T>(testModuleBuilder);
        }

        [Test]
        public void Aspects_Must_Not_Be_Classes()
        {
            var maybeType = TryGenerate<ClassAspect>();
            Assert.Null(maybeType);
        }

        [Test]
        public void Aspects_May_Be_Interfaces()
        {
            var maybeType = TryGenerate<IEmptyAspect>();
            Assert.NotNull(maybeType);
        }

        [Test]
        public void Conventional_Interface_Names_Get_Correct_Implementation_Name()
        {
            // NOTE: The pattern is the interface name without the leading I and "Implementation" appended

            var type = TryGenerate<IEmptyAspect>()!;
            Assert.AreEqual(type.Name, "EmptyAspectImplementation");
        }

        [Test]
        public void Unconventional_Interface_Names_Get_Correct_Implementation_Name()
        {
            // NOTE: The pattern is the interface name with "Implementation" appended

            var type = TryGenerate<UnconventionalAspect>()!;
            Assert.AreEqual(type.Name, "UnconventionalAspectImplementation");
        }
    }
}