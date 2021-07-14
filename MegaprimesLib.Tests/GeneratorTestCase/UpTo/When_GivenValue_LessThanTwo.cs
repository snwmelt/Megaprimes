using NUnit.Framework;

namespace MegaprimesLib.Tests.GeneratorTestCase.UpTo
{
    internal sealed class When_GivenValue_LessThanTwo
    {
        [Test]
        public void ShouldYield_NoValues ( )
        {
            Assert.IsEmpty(MegaprimeGenerator.UpTo(1));
            Assert.IsEmpty(MegaprimeGenerator.UpTo(0));
        }

        [Test]
        public void ShouldNot_ReturnNull ( )
            => Assert.IsNotNull(MegaprimeGenerator.UpTo(1));
    }
}
