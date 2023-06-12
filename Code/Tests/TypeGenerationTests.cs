﻿#nullable enable

using System;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;

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
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public interface IMethodAspect : IGameObjectAspect
        {
            void Test();
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public interface IEventAspect : IGameObjectAspect
        {
            event Action Test;
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public interface ISingleAspect<out T> : IGameObjectAspect
        {
            public T Value { get; }
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

        [Test]
        public void Primitive_Properties_Are_Not_Allowed()
        {
            var maybeType = TryGenerate<ISingleAspect<int>>();
            Assert.Null(maybeType);
        }

        [Test]
        public void Struct_Properties_Are_Not_Allowed()
        {
            var maybeType = TryGenerate<ISingleAspect<Vector3>>();
            Assert.Null(maybeType);
        }

        [Test]
        public void Object_Properties_Are_Not_Allowed()
        {
            var maybeType = TryGenerate<ISingleAspect<object>>();
            Assert.Null(maybeType);
        }

        [Test]
        public void Aspects_Must_Not_Have_Methods()
        {
            var maybeType = TryGenerate<IMethodAspect>();
            Assert.Null(maybeType);
        }

        [Test]
        public void Aspects_Must_Not_Have_Events()
        {
            var maybeType = TryGenerate<IEventAspect>();
            Assert.Null(maybeType);
        }
    }
}