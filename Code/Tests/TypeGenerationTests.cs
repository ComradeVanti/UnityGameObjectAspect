using NUnit.Framework;

namespace Dev.ComradeVanti.GameObjectAspect
{
    public class TypeGenerationTests
    {
        private abstract class ClassAspect : IGameObjectAspect
        {
        }

        private interface ITestAspect : IGameObjectAspect
        {
        }


        [Test]
        public void Aspects_Must_Not_Be_Classes()
        {
            var maybeType = TypeGeneration.TryGenerateImplementationType<ClassAspect>();
            Assert.Null(maybeType);
        }

        [Test]
        public void Aspects_May_Be_Interfaces()
        {
            var maybeType = TypeGeneration.TryGenerateImplementationType<ITestAspect>();
            Assert.NotNull(maybeType);
        }
    }
}