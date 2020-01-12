using SFX.CSharp.System.Model;
using Xunit;

namespace SFX.System.Test.Unit.Model
{
    [Trait("Category", "Unit")]
    public class SaltTest
    {
        #region IsValid
        [Fact]
        public void Null_Is_Invalid()
        {
            var sut = new Salt();

            Assert.False(sut.IsValid());
        }

        [Fact]
        public void Non_null_is_valid()
        {
            var sut = new Salt();
            sut.SetStringValue("Jow");

            Assert.True(sut.IsValid());
        }
        #endregion

        #region Equals
        [Fact]
        public void Equals_null_works()
        {
            var sut = new Salt();
            sut.SetStringValue("C:\\joHn.dOe");

            Assert.False(sut.Equals(null));
        }

        [Fact]
        public void Equals_other_type_works()
        {
            var sut = new Salt();
            sut.SetStringValue("C:\\joHn.dOe");

            Assert.False(sut.Equals(123));
        }
        #endregion

        #region ToString
        [Fact]
        public void ToString_works()
        {
            var sut = new Salt();
            sut.SetStringValue("C:\\joHn.dOe");

            Assert.Equal("C:\\joHn.dOe", sut.GetStringValue());
        }
        #endregion

        #region GetHashCode
        [Fact]
        public void GetHashCode_null_works()
        {
            var sut = new Salt();

            Assert.Equal(0, sut.GetHashCode());
        }

        [Fact]
        public void GetHashCode_works()
        {
            var sut1 = new Salt();
            sut1.SetStringValue("C:\\joHn.dOe");
            var sut2 = new Salt();
            sut2.SetStringValue("c:\\JOhN.DoE");

            Assert.NotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }
        #endregion
    }
}