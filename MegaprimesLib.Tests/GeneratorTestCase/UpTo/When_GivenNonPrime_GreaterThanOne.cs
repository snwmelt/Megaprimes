using System;
using System.Linq;
using NUnit.Framework;

namespace MegaprimesLib.Tests.GeneratorTestCase.UpTo
{
    internal sealed class When_GivenNonPrime_GreaterThanOne
    {
        private static UInt32    _ValueUnderTest;
        private static UInt32[]  _ExpectedValues;

        [SetUp]
        public void Setup ( )
        {
            _ValueUnderTest = 10U;
            _ExpectedValues = new[]
            {
                2U,
                3U,
                5U,
                7U
            };
        }

        [Test]
        public void ShouldYield_ExpectedValues ( )
             => Assert.AreEqual(_ExpectedValues, MegaprimeGenerator.UpTo(_ValueUnderTest));

        [Test]
        public void ShouldHaveLength_EqualTo_ExpectedValuesLength ( )
            => Assert.AreEqual(_ExpectedValues.Length, MegaprimeGenerator.UpTo(_ValueUnderTest).Count( ));

        [Test]
        public void ShouldNotYield_ProvidedValue ( )
            => Assert.IsFalse(MegaprimeGenerator.UpTo(_ValueUnderTest).Contains(_ValueUnderTest));
    }
}
