using AutoFixture;
using FsCheck;
using FsCheck.Xunit;
using SFX.EventAggregation.Model;
using System;
using Xunit;
using static System.String;

namespace SFX.EventAggregation.Test.Unit.Model
{
    [Trait("Category", "Unit")]
    public sealed class NameTest
    {
        private readonly IFixture _fixture;
        private readonly string _value;
        private readonly int _hashCode;

        public NameTest()
        {
            _fixture = new Fixture();
            _value = _fixture.Create<string>();
            _hashCode = _value.GetHashCode();

            Generators.RegisterAll();
        }

        #region Type test
        [Fact]
        public void Name_Is_A_ValueType() =>
            Assert.True(typeof(Name).IsValueType);

        [Fact]
        public void Name_Is_IEquatable() =>
            Assert.True(typeof(IEquatable<Name>).IsAssignableFrom(typeof(Name)));
        #endregion

        #region Initialization test
        [Fact]
        public void Initializing_Sets_Value()
        {
            var sut = new Name(_value);

            Assert.Equal(_value, sut.Value);
        }

        [Fact]
        public void Initializing_Sets_HashCode()
        {
            var sut = new Name(_value);

            Assert.Equal(_hashCode, sut.HashCode);
        }
        #endregion

        #region IsValid
        [Fact]
        public void IsValid_Works_For_Null()
        {
            var sut = new Name(null);

            Assert.False(sut.IsValid());
        }

        [Fact]
        public void IsValid_Works_For_Empty()
        {
            var sut = new Name(string.Empty);

            Assert.False(sut.IsValid());
        }

        [Fact]
        public void IsValid_Works_For_White_Space()
        {
            var sut = new Name("  \t  ");

            Assert.False(sut.IsValid());
        }

        [Property]
        public Property IsValid_works(Name x) =>
            ((!IsNullOrWhiteSpace(x.Value)) == x.IsValid()).ToProperty();
        #endregion

        #region Equals
        [Property]
        public Property Equals_null_works(Name x) =>
            (!x.Equals(null)).ToProperty();

        [Property]
        public Property Equals_other_type_works(Name x, string y) =>
            (!x.Equals(y)).ToProperty();

        [Property]
        public Property Equals_works(Name x, Name y) =>
            (!x.IsValid() || !y.IsValid() ? !x.Equals(y) :
            (0 == Compare(x.Value, y.Value, StringComparison.InvariantCulture)) == x.Equals(y)).ToProperty();
        #endregion

        #region ToString
        [Fact]
        public void ToString_For_Valid_Works()
        {
            var sut = new Name(_value);

            Assert.Equal(_value, sut.ToString());
        }

        [Property]
        public Property ToString_works(Name x) =>
            (x.Value == x.ToString()).ToProperty();
        #endregion

        #region GetHashCode
        [Fact]
        public void GetHashCode_Works()
        {
            var sut = new Name(_value);

            Assert.Equal(_hashCode, sut.GetHashCode());
        }
        #endregion
    }
}
