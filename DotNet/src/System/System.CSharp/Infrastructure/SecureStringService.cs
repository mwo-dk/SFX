using SFX.CSharp.ROP;
using System;
using System.Runtime.InteropServices;
using System.Security;
using static SFX.CSharp.ROP.Library;

namespace SFX.CSharp.System.Infrastructure
{
    /// <summary>
    /// Provides conversion between <see cref="string"/> and <see cref="SecureString"/>
    /// </summary>
    public sealed class SecureStringService : ISecureStringService
    {
        /// <inheritdoc/>
        public Result<SecureString> ToSecureString(string input)
        {
            if (input is null)
                return Fail<SecureString>(new ArgumentNullException(nameof(input)));

            try
            {
                SecureString secure = new SecureString();
                foreach (char c in input)
                {
                    secure.AppendChar(c);
                }
                secure.MakeReadOnly();
                return Succeed(secure);
            }
            catch (Exception error)
            {
                return Fail<SecureString>(error);
            }
        }

        /// <inheritdoc/>
        public Result<string> ToInsecureString(SecureString input)
        {
            if (input is null)
                return Fail<string>(new ArgumentNullException(nameof(input)));

            IntPtr ptr = default;
            string result = default;
            bool errorOccurred = false;
            Exception error = default;
            try
            {
                ptr = Marshal.SecureStringToBSTR(input);
                result = Marshal.PtrToStringBSTR(ptr);
            }
            catch (Exception exn)
            {
                errorOccurred = true;
                error = exn;
            }
            finally
            {
                if (ptr != default)
                    try
                    {
                        Marshal.ZeroFreeBSTR(ptr);
                    }
                    catch (Exception innerError)
                    {
                        errorOccurred = true;
                        error = innerError;
                    }
            }

            return errorOccurred ?
                Fail<string>(error) :
                Succeed(result);
        }
    }
}
