using SFX.CSharp.System.Model;
using Xunit;

namespace SFX.System.Test.Unit.Model
{
    [Trait("Category", "Unit")]
    public class FolderPathTest
    {
        #region Equals
        [Fact]
        public void Equals_null_works()
        {
            var sut = new FolderPath { Value = "C:\\joHn.dOe" };

            Assert.False(sut.Equals(null));
        }

        [Fact]
        public void Equals_other_type_works()
        {
            var sut = new FolderPath { Value = "C:\\joHn.dOe" };

            Assert.False(sut.Equals(123));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Equals_does_not_ignore_case()
        {
            var sut1 = new FolderPath { Value = "C:\\joHn.dOe" };
            var sut2 = new FolderPath { Value = "c:\\JOhN.DoE" };

            Assert.NotEqual(sut1, sut2);
        }

        [Fact]
        public void Equals_ignores_case()
        {
            var sut1 = new FolderPath { Value = "C:\\joHn.dOe", IgnoreCase = true };
            var sut2 = new FolderPath { Value = "c:\\JOhN.DoE", IgnoreCase = true };

            Assert.Equal(sut1, sut2);
        }
        #endregion

        #region ToString
        [Fact]
        public void ToString_works()
        {
            var sut = new FolderPath { Value = "C:\\joHn.dOe" };

            Assert.Equal("C:\\joHn.dOe", sut.Value);
        }
        #endregion

        #region GetHashCode
        [Fact]
        public void GetHashCode_works_1()
        {
            var sut1 = new FolderPath { Value = "C:\\joHn.dOe" };
            var sut2 = new FolderPath { Value = "c:\\JOhN.DoE" };

            Assert.NotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GetHashCode_works_2()
        {
            var sut1 = new FolderPath { Value = "C:\\joHn.dOe", IgnoreCase = true };
            var sut2 = new FolderPath { Value = "c:\\JOhN.DoE", IgnoreCase = true };

            Assert.Equal(sut1.GetHashCode(), sut2.GetHashCode());
        }
        #endregion
    }
}
