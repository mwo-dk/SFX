using SFX.CSharp.ROP;

namespace SFX.CSharp.System.Infrastructure
{
    /// <summary>
    /// Describes to capability to encode data to base 64 strings as well as to 
    /// decode back
    /// </summary>
    public interface IBase64Service
    {
        /// <summary>
        /// Converts binary data to a base 64 encoded string
        /// </summary>
        /// <param name="data">The binary data to encode</param>
        /// <returns><paramref name="data"/> converted to a base 64 encoded string</returns>
        Result<string> ToBase64String(byte[] data);

        /// <summary>
        /// Converts a base 64 encoded string to binary data
        /// </summary>
        /// <param name="data">The base 64 encoded string to decode</param>
        /// <returns><paramref name="data"/> decoded to binary data</returns>
        Result<byte[]> FromBase64String(string data);
    }
}
