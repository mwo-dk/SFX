using AutoFixture;
using System;
using System.Linq;
using Xunit;
using static SFX.Utils.Infrastructure.HashCodeHelpers;

namespace SFX.Utils.Test.Unit.Infrastructure
{
    [Trait("Category", "Unit")]
    public class ComputeHashCodeTests
    {
        private readonly IFixture _fixture;
        private readonly int _prime1;
        private readonly int _prime2;
        private readonly int[] _data;
        private readonly object[] _objectData;

        public ComputeHashCodeTests()
        {
            _fixture = new Fixture();

            _prime1 = _fixture.Create<int>(); // Not important that it is a prime
            _prime2 = _fixture.Create<int>();
            _data = _fixture.CreateMany<int>().ToArray();
            _objectData = new object[]
            {
                _fixture.Create<string>(),
                null,
                _fixture.Create<double>()
            };
        }

        [Fact]
        public void ComputeHashCode_With_Null_Array_Throws() =>
            Assert.Throws<ArgumentNullException>(() =>
            {
                int n = (null as int[]).ComputeHashCode(_prime1, _prime2);
            });

        [Fact]
        public void ComputeHashCode_With_Empty_Array_Works() =>
            Assert.Throws<ArgumentException>(() =>
            {
                int n = (new int[] { }).ComputeHashCode(_prime1, _prime2);
            });

        [Fact]
        public void ComputeHashCode_Works()
        {
            var expected = _prime1;
            unchecked
            {
                for (var n = 0; n < _data.Length; ++n)
                    expected = _prime2 * expected + _data[n];
            }

            var result = _data.ComputeHashCode(_prime1, _prime2);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ComputeHashCodeForObjectArray_Works()
        {
            var expected = _prime1;
            unchecked
            {
                expected = _prime2 * expected + _objectData[0].GetHashCode();
                expected = _prime2 * expected + _objectData[2].GetHashCode();
            }

            var result = ComputeHashCodeForObjectArray(_prime1, _prime2, _objectData);

            Assert.Equal(expected, result);
        }
    }
}
