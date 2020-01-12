using SFX.CSharp.ROP;
using System;
using static SFX.CSharp.ROP.Library;

namespace SFX.CSharp.System.Infrastructure
{
    public sealed class Base64Service : IBase64Service
    {
        /// <inheritdoc/>
        public Result<string> ToBase64String(byte[] data)
        {
            if (data is null)
                return Fail<string>(new ArgumentNullException(nameof(data)));

            try
            {
                var result = Convert.ToBase64String(data);
                return Succeed(result);
            }
            catch (Exception error)
            {
                return Fail<string>(error);
            }
        }

        /// <inheritdoc/>
        public Result<byte[]> FromBase64String(string data)
        {
            if (data is null || data.Length == 0)
                return Fail<byte[]>(new ArgumentNullException(nameof(data)));

            try
            {
                var result = Convert.FromBase64String(data);
                return Succeed(result);
            }
            catch (Exception error)
            {
                return Fail<byte[]>(error);
            }
        }
    }
}
