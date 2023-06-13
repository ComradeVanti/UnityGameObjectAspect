#nullable enable

using System;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using static Dev.ComradeVanti.GameObjectAspect.TestHelpers;

namespace Dev.ComradeVanti.GameObjectAspect
{
    public class TypeGenerationTests
    {
        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public abstract class ClassAspect : IGameObjectAspect
        {
            public abstract GameObject GameObject { get; }
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


        [Test]
        public void Aspects_Must_Not_Be_Classes()
        {
            var maybeType = TryGenerateTestImplementation<ClassAspect>();
            Assert.Null(maybeType);
        }

        [Test]
        public void Aspects_May_Be_Interfaces()
        {
            var maybeType = TryGenerateTestImplementation<IEmptyAspect>();
            Assert.NotNull(maybeType);
        }

        [Test]
        public void Conventional_Interface_Names_Get_Correct_Implementation_Name()
        {
            // NOTE: The pattern is the interface name without the leading I and "Implementation" appended

            var type = GenerateTestImplementation<IEmptyAspect>();
            Assert.AreEqual(type.Name, "EmptyAspectImplementation");
        }

        [Test]
        public void Unconventional_Interface_Names_Get_Correct_Implementation_Name()
        {
            // NOTE: The pattern is the interface name with "Implementation" appended

            var type = GenerateTestImplementation<UnconventionalAspect>();
            Assert.AreEqual(type.Name, "UnconventionalAspectImplementation");
        }

        [Test]
        public void Primitive_Properties_Are_Not_Allowed()
        {
            var maybeType = TryGenerateTestImplementation<ISingleAspect<int>>();
            Assert.Null(maybeType);
        }

        [Test]
        public void Struct_Properties_Are_Not_Allowed()
        {
            var maybeType = TryGenerateTestImplementation<ISingleAspect<Vector3>>();
            Assert.Null(maybeType);
        }

        [Test]
        public void Object_Properties_Are_Not_Allowed()
        {
            var maybeType = TryGenerateTestImplementation<ISingleAspect<object>>();
            Assert.Null(maybeType);
        }

        [Test]
        public void Aspects_Must_Not_Have_Methods()
        {
            var maybeType = TryGenerateTestImplementation<IMethodAspect>();
            Assert.Null(maybeType);
        }

        [Test]
        public void Aspects_Must_Not_Have_Events()
        {
            var maybeType = TryGenerateTestImplementation<IEventAspect>();
            Assert.Null(maybeType);
        }

        [Test]
        public void Generated_Types_Implement_The_Interface()
        {
            var type = GenerateTestImplementation<IEmptyAspect>();
            var instance = Activator.CreateInstance(type)!;
            Assert.That(instance, Is.AssignableTo<IEmptyAspect>());
        }
    }
}