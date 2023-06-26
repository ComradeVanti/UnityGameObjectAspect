using NUnit.Framework;
using UnityEngine;

namespace Dev.ComradeVanti.GameObjectAspect
{
    public class MiscTests
    {
        [Test]
        public void Aspects_Are_Unequal_If_They_Have_Different_Same_GameObjects()
        {
            var gameObject1 = new GameObject("Aspect1");
            var gameObject2 = new GameObject("Aspect2");

            var aspect1 = gameObject1.TryAspect<IEmptyAspect>()!;
            var aspect2 = gameObject2.TryAspect<IEmptyAspect>()!;

            Assert.AreNotEqual(aspect1, aspect2);
        }

        [Test]
        public void Aspects_Are_Equal_If_They_Have_The_Same_GameObject()
        {
            var gameObject = new GameObject("Aspect");

            var aspect1 = gameObject.TryAspect<IEmptyAspect>()!;
            var aspect2 = gameObject.TryAspect<IEmptyAspect>()!;

            Assert.AreEqual(aspect1, aspect2);
        }
    }
}