using AutoFixture;
using FsCheck;
using FsCheck.Xunit;
using SFX.EventAggregation.Model;
using System;
using Xunit;
using static SFX.Utils.Infrastructure.HashCodeHelpers;
using static System.String;

namespace SFX.EventAggregation.Test.Unit.Model
{
    [Trait("Category", "Unit")]
    public sealed class TypeAndNameTest
    {
        private readonly IFixture _fixture;
        private readonly Type _type;
        private readonly Name _name;
        private readonly int _hashCode;

        public TypeAndNameTest()
        {
            _fixture = new Fixture();
            _type = _fixture.Create<Type>();
            _name = _fixture.Create<Name>();
            _hashCode = ComputeHashCodeForObjectArray(_type, _name);

            Generators.RegisterAll();
        }

        #region Type test
        [Fact]
        public void TypeAndName_Is_A_ValueType() =>
            Assert.True(typeof(TypeAndName).IsValueType);

        [Fact]
        public void TypeAndName_Is_IEquatable() =>
            Assert.True(typeof(IEquatable<TypeAndName>).IsAssignableFrom(typeof(TypeAndName)));
        #endregion

        #region Initialization test
        [Fact]
        public void Initializing_Sets_Type()
        {
            var sut = new TypeAndName(_type, _name);

            Assert.Same(_type, sut.Type);
        }

        [Fact]
        public void Initializing_Sets_Name()
        {
            var sut = new TypeAndName(_type, _name);

            Assert.Equal(_name, sut.Name);
        }

        [Fact]
        public void Initializing_Sets_HashCode()
        {
            var sut = new TypeAndName(_type, _name);

            Assert.Equal(_hashCode, sut.HashCode);
        }
        #endregion

        #region IsValid
        [Fact]
        public void IsValid_Works_For_Null_Type()
        {
            var sut = new TypeAndName(null, _name);

            Assert.False(sut.IsValid());
        }

        [Fact]
        public void IsValid_Works_For_Null_Name()
        {
            var sut = new TypeAndName(_type, new Name(null));

            Assert.False(sut.IsValid());
        }

        [Fact]
        public void IsValid_Works_For_Empty_Name()
        {
            var sut = new TypeAndName(_type, new Name(Empty));

            Assert.False(sut.IsValid());
        }

        [Fact]
        public void IsValid_Works_For_White_Space_Name()
        {
            var sut = new TypeAndName(_type, new Name("  \t  "));

            Assert.False(sut.IsValid());
        }

        [Property]
        public Property IsValid_works(TypeAndName x) =>
            ((!(x.Type is null) && x.Name.IsValid()) == x.IsValid()).ToProperty();
        #endregion

        #region Equals
        [Property]
        public Property Equals_null_works(TypeAndName x) =>
            (!x.Equals(null)).ToProperty();

        [Property]
        public Property Equals_other_type_works(TypeAndName x, string y) =>
            (!x.Equals(y)).ToProperty();

        [Property]
        public Property Equals_works(TypeAndName x, TypeAndName y) =>
            (!x.IsValid() || !y.IsValid() ? !x.Equals(y) :
            ((x.Type == y.Type && x.Name.Equals(y.Name)) == x.Equals(y)))
            .ToProperty();
        #endregion

        #region ToString
        [Property]
        public Property ToString_works(TypeAndName x) =>
            (($"{x.Type?.Name}-{x.Name.Value}" == x.ToString())).ToProperty();
        #endregion

        #region GetHashCode
        [Property]
        public void GetHashCode_Works()
        {
            var sut = new TypeAndName(_type, _name);

            Assert.Equal(_hashCode, sut.GetHashCode());
        }
        #endregion
    }
}
