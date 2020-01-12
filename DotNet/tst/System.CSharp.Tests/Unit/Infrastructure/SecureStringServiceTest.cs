using SFX.CSharp.System.Infrastructure;
using System;
using Xunit;

namespace SFX.System.Test.Unit.Infrastructure
{
    [Trait("Category", "Unit")]
    public class SecureStringServiceTest
    {
        #region ToSecureString
        [Fact]
        public void ToSecureString_null_works()
        {
            var sut = new SecureStringService();

            var (success, result, error) = sut.ToSecureString(null);

            Assert.False(success);
            Assert.IsAssignableFrom<ArgumentNullException>(error);
            Assert.Null(result);
        }

        [Fact]
        public void ToSecureString_works()
        {
            var sut = new SecureStringService();

            var (success, result, error) = sut.ToSecureString("Hello world");

            Assert.True(success);
            Assert.Null(error);
            Assert.NotNull(result);
        }
        #endregion

        #region ToInsecureString
        [Fact]
        public void ToInsecureString_null_works()
        {
            var sut = new SecureStringService();

            var (success, result, error) = sut.ToInsecureString(null);

            Assert.False(success);
            Assert.IsAssignableFrom<ArgumentNullException>(error);
            Assert.Null(result);
        }

        [Fact]
        public void ToInsecureString_works()
        {
            var sut = new SecureStringService();

            var (_, secureString, _) = sut.ToSecureString("Hello world");
            var (success, result, error) = sut.ToInsecureString(secureString);

            Assert.True(success);
            Assert.Null(error);
            Assert.NotNull(result);
            Assert.Equal("Hello world", result);
        }
        #endregion
    }
}
