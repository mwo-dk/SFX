using SFX.CSharp.ROP;
using SFX.CSharp.System.Model;
using System;
using System.Security;
using System.Text;
using static SFX.CSharp.ROP.Library;

namespace SFX.CSharp.System.Infrastructure
{

    /// <summary>
    /// Implementation of <see cref="IEncryptionService"/>
    /// </summary>
    public sealed class EncryptionService : IEncryptionService
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="secureStringService"><see cref="ISecureStringService"/> utilized for handling <see cref="SecureString"/>s</param>
        public EncryptionService(ISecureStringService secureStringService) =>
            SecureStringService = secureStringService ?? throw new ArgumentNullException(nameof(secureStringService));

        /// <summary>
        /// The <see cref="ISecureStringService"/> utlized to secure strings
        /// </summary>
        internal ISecureStringService SecureStringService { get; }

        /// <inheritdoc/>
        public Result<byte[]> Encrypt(byte[] input, Salt salt)
        {
            if (input is null)
                return Fail<byte[]>(new ArgumentNullException(nameof(input)));
            if (salt is null)
                return Fail<byte[]>(new ArgumentNullException(nameof(salt)));
            if (!salt.IsValid())
                return Fail<byte[]>(new ArgumentException("Salt is not valid"));

            try
            {
                byte[] encryptedData = global::System.Security.Cryptography.ProtectedData.Protect(
                    input, salt.Value,
                    global::System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return Succeed(encryptedData);
            }
            catch (Exception exn)
            {
                return Fail<byte[]>(exn);
            }
        }

        /// <inheritdoc/>
        public Result<byte[]> EncryptString(string input, Salt salt)
        {
            if (input is null)
                return Fail<byte[]>(new ArgumentNullException(nameof(input)));
            if (salt is null)
                return Fail<byte[]>(new ArgumentNullException(nameof(salt)));
            if (!salt.IsValid())
                return Fail<byte[]>(new ArgumentException("Salt is not valid"));

            try
            {
                byte[] encryptedData = global::System.Security.Cryptography.ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(input), salt.Value,
                    global::System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return Succeed(encryptedData);
            }
            catch (Exception exn)
            {
                return Fail<byte[]>(exn);
            }
        }

        /// <inheritdoc/>
        public Result<byte[]> EncryptSecureString(SecureString input, Salt salt)
        {
            if (input is null)
                return Fail<byte[]>(new ArgumentNullException(nameof(input)));
            if (salt is null)
                return Fail<byte[]>(new ArgumentNullException(nameof(salt)));
            if (!salt.IsValid())
                return Fail<byte[]>(new ArgumentException("Salt is not valid"));

            var (success, str, error) =
                SecureStringService.ToInsecureString(input);
            if (!success)
                return Fail<byte[]>(error);

            try
            {
                byte[] encryptedData = global::System.Security.Cryptography.ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(str), salt.Value,
                    global::System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return Succeed(encryptedData);
            }
            catch (Exception exn)
            {
                return Fail<byte[]>(exn);
            }
        }

        /// <inheritdoc/>
        public Result<byte[]> Decrypt(byte[] encryptedData, Salt salt)
        {
            if (encryptedData is null)
                return Fail<byte[]>(new ArgumentNullException(nameof(encryptedData)));
            if (salt is null)
                return Fail<byte[]>(new ArgumentNullException(nameof(salt)));
            if (!salt.IsValid())
                return Fail<byte[]>(new ArgumentException("Salt is not valid"));

            try
            {
                byte[] decryptedData =
                    global::System.Security.Cryptography.ProtectedData.Unprotect(
                        encryptedData,
                        salt.Value,
                        global::System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return Succeed(decryptedData);
            }
            catch (Exception exn)
            {
                return Fail<byte[]>(exn);
            }
        }

        /// <inheritdoc/>
        public Result<string> DecryptString(byte[] encryptedData, Salt salt)
        {
            if (encryptedData is null)
                return Fail<string>(new ArgumentNullException(nameof(encryptedData)));
            if (encryptedData.Length == 0)
                return Fail<string>(new ArgumentException("Unable to decrypt empty data"));
            if (salt is null)
                return Fail<string>(new ArgumentNullException(nameof(salt)));
            if (!salt.IsValid())
                return Fail<string>(new ArgumentException("Salt is not valid"));

            try
            {
                byte[] decryptedData =
                    global::System.Security.Cryptography.ProtectedData.Unprotect(
                        encryptedData,
                        salt.Value,
                        global::System.Security.Cryptography.DataProtectionScope.CurrentUser);
                var result = Encoding.Unicode.GetString(decryptedData);
                return Succeed(result);
            }
            catch (Exception exn)
            {
                return Fail<string>(exn);
            }
        }

        /// <inheritdoc/>
        public Result<SecureString> DecryptSecureString(byte[] encryptedData, Salt salt)
        {
            if (encryptedData is null)
                return Fail<SecureString>(new ArgumentNullException(nameof(encryptedData)));
            if (encryptedData.Length == 0)
                return Fail<SecureString>(new ArgumentException("Unable to decrypt empty data"));
            if (salt is null)
                return Fail<SecureString>(new ArgumentNullException(nameof(salt)));
            if (!salt.IsValid())
                return Fail<SecureString>(new ArgumentException("Salt is not valid"));

            try
            {
                byte[] decryptedData =
                    global::System.Security.Cryptography.ProtectedData.Unprotect(
                        encryptedData,
                        salt.Value,
                        global::System.Security.Cryptography.DataProtectionScope.CurrentUser);
                var (success, result, error) =
                    SecureStringService.ToSecureString(Encoding.Unicode.GetString(decryptedData));
                if (!success)
                    return Fail<SecureString>(error);
                return Succeed(result);
            }
            catch (Exception exn)
            {
                return Fail<SecureString>(exn);
            }
        }
    }
}
