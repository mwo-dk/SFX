using SFX.CSharp.ROP;
using SFX.CSharp.System.Model;
using System.Security;

namespace SFX.CSharp.System.Infrastructure
{
    /// <summary>
    /// Interface facilitating encryption
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        /// Encrypts a byte array
        /// </summary>
        /// <param name="input">The binary data to encrypt</param>
        /// <param name="salt">The <see cref="Salt"/></param>
        /// <returns>Entrypted version of <paramref name="input"/></returns>
        Result<byte[]> Encrypt(byte[] input, Salt salt);

        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="input">The <see cref="string"/> to encrypt</param>
        /// <param name="salt">The <see cref="Salt"/></param>
        /// <returns>Entrypted version of <paramref name="input"/></returns>
        Result<byte[]> EncryptString(string input, Salt salt);

        /// <summary>
        /// Encrypts a secure string
        /// </summary>
        /// <param name="input">The <see cref="SecureString"/> to encrypt</param>
        /// <param name="salt">The <see cref="Salt"/></param>
        /// <returns>Entrypted version of <paramref name="input"/></returns>
        Result<byte[]> EncryptSecureString(SecureString input, Salt salt);

        /// <summary>
        /// Decrypts binary data.
        /// </summary>
        /// <param name="encryptedData">The <see cref="byte[]"/> to decrypt</param>
        /// <param name="salt">The <see cref="Salt"/></param>
        /// <returns>Decrypted version of <paramref name="encryptedData"/></returns>
        Result<byte[]> Decrypt(byte[] encryptedData, Salt salt);

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param name="encryptedData">The <see cref="byte[]"/> to decrypt</param>
        /// <param name="salt">The <see cref="Salt"/></param>
        /// <returns>Decrypted version of <paramref name="encryptedData"/></returns>
        Result<string> DecryptString(byte[] encryptedData, Salt salt);

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param name="encryptedData">The <see cref="byte[]"/> to decrypt</param>
        /// <param name="salt">The <see cref="Salt"/></param>
        /// <returns>Decrypted version of <paramref name="encryptedData"/></returns>
        Result<SecureString> DecryptSecureString(byte[] encryptedData, Salt salt);
    }
}
