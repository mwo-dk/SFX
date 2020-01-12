using AutoFixture;
using FakeItEasy;
using SFX.CSharp.System.Infrastructure;
using SFX.CSharp.System.Model;
using System;
using System.Security;
using System.Text;
using Xunit;
using static FakeItEasy.A;
using static SFX.CSharp.ROP.Library;

namespace SFX.System.Test.Unit.Infrastructure
{
    [Trait("Category", "Unit")]
    public class EncryptionServiceTest
    {
        #region Members
        private readonly Fixture _fixture;
        private readonly Salt _salt;
        #endregion

        #region Test initialization
        public EncryptionServiceTest()
        {
            _fixture = new Fixture();

            _salt = new Salt();
            _salt.SetStringValue("Salt is not a password");
        }
        #endregion

        #region Type test
        [Fact]
        public void EncryptionService_implements_IEncryptionService()
        {
            Assert.True(typeof(IEncryptionService).IsAssignableFrom(typeof(EncryptionService)));
        }

        [Fact]
        public void EncryptionService_is_sealed()
        {
            Assert.True(typeof(EncryptionService).IsSealed);
        }
        #endregion

        #region Initialization test
        [Fact]
        public void Initializing_EncryptionService_with_null_SecureStringService_throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EncryptionService(null));
        }

        [Fact]
        public void Initializing_EncryptionService_sets_SecureStringService_property()
        {
            var secureStringService = Fake<ISecureStringService>();
            var sut = new EncryptionService(secureStringService);

            Assert.Same(secureStringService, sut.SecureStringService);
        }
        #endregion

        #region EncryptString
        [Fact]
        public void Encrypt_null_fails()
        {
            var secureStringService = Fake<ISecureStringService>();
            var sut = new EncryptionService(secureStringService);

            var (success, result, error) = sut.Encrypt(null, _salt);

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentNullException), error.GetType());
            Assert.Null(result);
        }

        [Fact]
        public void Encrypt_invalid_salt_fails()
        {
            var message = "Hello world";
            var data = Encoding.Unicode.GetBytes(message);
            var secureStringService = Fake<ISecureStringService>();
            var sut = new EncryptionService(secureStringService);

            var (success, result, error) = sut.Encrypt(data, new Salt());

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentException), error.GetType());
            Assert.Null(result);
        }
        #endregion

        #region Decrypt
        [Fact]
        public void Decrypt_null_fails()
        {
            var secureStringService = Fake<ISecureStringService>();
            var sut = new EncryptionService(secureStringService);

            var (success, result, error) = sut.Decrypt(null, _salt);

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentNullException), error.GetType());
            Assert.Null(result);
        }

        [Fact]
        public void Decrypt_empty_fails()
        {
            var secureStringService = Fake<ISecureStringService>();
            var sut = new EncryptionService(secureStringService);

            var (success, result, error) = sut.Decrypt(new byte[] { }, _salt);

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Null(result);
        }

        [Fact]
        public void Decrypt_invalid_salt_fails()
        {
            var message = "Hello world";
            var secureStringService = new SecureStringService();
            var sut = new EncryptionService(secureStringService);
            var (_, str_, _) = sut.EncryptString(message, _salt);

            var (success, result, error) = sut.Decrypt(str_, new Salt());

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentException), error.GetType());
            Assert.Null(result);
        }
        #endregion

        #region EncryptString
        [Fact]
        public void EncryptString_null_fails()
        {
            var secureStringService = Fake<ISecureStringService>();
            var sut = new EncryptionService(secureStringService);

            var (success, result, error) = sut.EncryptString(null, _salt);

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentNullException), error.GetType());
            Assert.Null(result);
        }

        [Fact]
        public void EncryptString_invalid_salt_fails()
        {
            var message = "Hello world";
            var secureStringService = Fake<ISecureStringService>();
            var sut = new EncryptionService(secureStringService);

            var (success, result, error) = sut.EncryptString(message, new Salt());

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentException), error.GetType());
            Assert.Null(result);
        }
        #endregion

        #region DecryptString
        [Fact]
        public void DecryptString_null_fails()
        {
            var secureStringService = Fake<ISecureStringService>();
            var sut = new EncryptionService(secureStringService);

            var (success, result, error) = sut.DecryptString(null, _salt);

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentNullException), error.GetType());
            Assert.Null(result);
        }

        [Fact]
        public void DecryptString_empty_fails()
        {
            var secureStringService = Fake<ISecureStringService>();
            var sut = new EncryptionService(secureStringService);

            var (success, result, error) = sut.DecryptString(new byte[] { }, _salt);

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentException), error.GetType());
            Assert.Null(result);
        }

        [Fact]
        public void DecryptString_invalid_salt_fails()
        {
            var message = "Hello world";
            var secureStringService = new SecureStringService();
            var sut = new EncryptionService(secureStringService);
            var (_, str_, _) = sut.EncryptString(message, _salt);

            var (success, result, error) = sut.DecryptString(str_, new Salt());

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentException), error.GetType());
            Assert.Null(result);
        }
        #endregion

        #region EncryptSecureString
        [Fact]
        public void EncryptSecureString_null_fails()
        {
            var secureStringService = Fake<ISecureStringService>();
            var sut = new EncryptionService(secureStringService);

            var (success, result, error) = sut.EncryptSecureString(null, _salt);

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentNullException), error.GetType());
            Assert.Null(result);
        }

        [Fact]
        public void EncryptSecureString_invalid_salt_fails()
        {
            var message = "Hello world";
            var secureStringService = new SecureStringService();
            var (_, str, _) = secureStringService.ToSecureString(message);
            var sut = new EncryptionService(secureStringService);

            var (success, result, error) = sut.EncryptSecureString(str, new Salt());

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentException), error.GetType());
            Assert.Null(result);
        }

        [Fact]
        public void EncryptSecureString_if_SecureStringService_fails_error_is_propagated()
        {
            var message = "Hello world";
            var _secureStringService = new SecureStringService();
            var (_, str, _) = _secureStringService.ToSecureString(message);
            var secureStringService = Fake<ISecureStringService>();
            var error = new Exception(_fixture.Create<string>());
            CallTo(() => secureStringService.ToInsecureString(str))
                .Returns(Fail<string>(error));
            var sut = new EncryptionService(secureStringService);

            var (success, result, error_) = sut.EncryptSecureString(str, _salt);

            Assert.False(success);
            Assert.Same(error, error_);
            Assert.Null(result);
        }
        #endregion

        #region DecryptSecureString
        [Fact]
        public void DecryptSecureString_null_fails()
        {
            var secureStringService = Fake<ISecureStringService>();
            var sut = new EncryptionService(secureStringService);

            var (success, result, error) = sut.DecryptSecureString(null, _salt);

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentNullException), error.GetType());
            Assert.Null(result);
        }

        [Fact]
        public void DecryptSecureString_empty_fails()
        {
            var secureStringService = Fake<ISecureStringService>();
            var sut = new EncryptionService(secureStringService);

            var (success, result, error) = sut.DecryptSecureString(new byte[] { }, _salt);

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentException), error.GetType());
            Assert.Null(result);
        }

        [Fact]
        public void DecryptSecureString_invalid_salt_fails()
        {
            var message = "Hello world";
            var secureStringService = new SecureStringService();
            var (_, str, _) = secureStringService.ToSecureString(message);
            var sut = new EncryptionService(secureStringService);
            var (_, str_, _) = sut.EncryptSecureString(str, _salt);

            var (success, result, error) = sut.DecryptSecureString(str_, new Salt());

            Assert.False(success);
            Assert.NotNull(error);
            Assert.Equal(typeof(ArgumentException), error.GetType());
            Assert.Null(result);
        }

        [Fact]
        public void DecryptSecureString_if_SecureStringService_fails_error_is_propagated()
        {
            var message = "Hello world";
            var _secureStringService = new SecureStringService();
            var (_, str, _) = _secureStringService.ToSecureString(message);
            var (_, data, _) = (new EncryptionService(_secureStringService))
                .EncryptSecureString(str, _salt);
            var secureStringService = Fake<ISecureStringService>();
            var error = new Exception(_fixture.Create<string>());
            CallTo(() => secureStringService.ToSecureString(message))
                .Returns(Fail<SecureString>(error));
            var sut = new EncryptionService(secureStringService);

            var (success, result, error_) = sut.DecryptSecureString(data, _salt);

            Assert.False(success);
            Assert.Same(error, error_);
            Assert.Null(result);
        }
        #endregion
    }
}
