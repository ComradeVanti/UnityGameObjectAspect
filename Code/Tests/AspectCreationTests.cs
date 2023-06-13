using NUnit.Framework;
using UnityEngine;
using static Dev.ComradeVanti.GameObjectAspect.TestHelpers;

namespace Dev.ComradeVanti.GameObjectAspect
{
    public class AspectCreationTests
    {
        [Test]
        public void Each_Aspect_References_Original_GameObject()
        {
            var gameObject = new GameObject("Empty");
            var aspect = GenerateTestAspect<IEmptyAspect>(gameObject);
            Assert.AreSame(aspect.GameObject, gameObject);
        }
    }
}