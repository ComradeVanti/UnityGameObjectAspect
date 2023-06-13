using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using static Dev.ComradeVanti.GameObjectAspect.TestHelpers;

namespace Dev.ComradeVanti.GameObjectAspect
{
    public class AspectCreationTests
    {
        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public interface IPhysicsObject : IGameObjectAspect
        {
            public Rigidbody Rigidbody { get; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public interface ITestComponentObject : IGameObjectAspect
        {
            public ITestComponent Component { get; }
        }


        [Test]
        public void Each_Aspect_References_Original_GameObject()
        {
            var gameObject = new GameObject("Empty");
            var aspect = GenerateTestAspect<IEmptyAspect>(gameObject);
            Assert.AreSame(gameObject, aspect.GameObject);
        }

        [Test]
        public void Aspect_Creation_Returns_Null_If_Self_Component_Is_Not_Found()
        {
            var gameObject = new GameObject("Empty"); // No rigidbody!
            var aspect = TryGenerateTestAspect<IPhysicsObject>(gameObject);
            Assert.Null(aspect);
        }

        [Test]
        public void Components_On_Self_Can_Be_Resolved()
        {
            var gameObject = new GameObject("Physics");
            var rigidBody = gameObject.AddComponent<Rigidbody>();
            var aspect = TryGenerateTestAspect<IPhysicsObject>(gameObject);
            Assert.NotNull(aspect);
            Assert.AreSame(rigidBody, aspect.Rigidbody);
        }

        [Test]
        public void Aspect_Creation_Returns_Null_If_Self_Interface_Is_Not_Found()
        {
            var gameObject = new GameObject("Empty"); // No rigidbody!
            var aspect = TryGenerateTestAspect<ITestComponentObject>(gameObject);
            Assert.Null(aspect);
        }

        [Test]
        public void Interface_On_Self_Can_Be_Resolved()
        {
            var gameObject = new GameObject("Physics");
            var component = (ITestComponent) gameObject.AddComponent<TestComponent>();
            var aspect = TryGenerateTestAspect<ITestComponentObject>(gameObject);
            Assert.NotNull(aspect);
            Assert.AreSame(component, aspect.Component);
        }
    }
}