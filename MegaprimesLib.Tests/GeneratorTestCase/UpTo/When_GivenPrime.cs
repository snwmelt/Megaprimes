using System;
using System.Linq;
using NUnit.Framework;

namespace MegaprimesLib.Tests.GeneratorTestCase.UpTo
{
    internal sealed class When_GivenPrime
    {
        private UInt32    _ValueUnderTest;
        private UInt32[]  _ExpectedValues;

        [SetUp]
        public void Setup ( )
        {
            _ValueUnderTest = 37U;
            _ExpectedValues = new[]
            {
                2U,
                3U,
                5U,
                7U,
                23U,
                37U
            };
        }

        [Test]
        public void ShouldYield_ExpectedValues ( )
            => Assert.AreEqual(_ExpectedValues, MegaprimeGenerator.UpTo(_ValueUnderTest));

        [Test]
        public void ShouldHaveLength_EqualTo_ExpectedValuesLength ( )
            => Assert.AreEqual(_ExpectedValues.Length, MegaprimeGenerator.UpTo(_ValueUnderTest).Count( ));

        [Test]
        public void ShouldYield_ProvidedValue ( )
            => Assert.IsTrue(MegaprimeGenerator.UpTo(_ValueUnderTest).Contains(_ValueUnderTest));
    }
}
