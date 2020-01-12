using Microsoft.Win32;
using SFX.CSharp.ROP;
using SFX.CSharp.System.Windows.Model.Registry;
using System;
using static SFX.CSharp.ROP.Library;

namespace SFX.CSharp.System.Infrastructure
{
    public sealed class RegistryReader : IRegistryReader
    {
        /// <inheritdoc/>
        public Result<RegistryStringValue>
            ReadStringValue(RegistryRootKey rootKey, RegistrySubPath path, RegistryKeyName name)
        {
            RegistryKey key = default;
            try
            {
                key = rootKey.Value.OpenSubKey(path.Value);
                var kind = key.GetValueKind("");
                if (kind != RegistryValueKind.String)
                    return Fail<RegistryStringValue>(new InvalidCastException());
                return Succeed(new RegistryStringValue { Value = (string)key.GetValue(name.Value) });
            }
            catch (Exception exn)
            {
                return Fail<RegistryStringValue>(exn);
            }
            finally
            {
                key?.Close();
            }
        }

        /// <inheritdoc/>
        public Result<RegistryBlobValue>
            ReadBlobValue(RegistryRootKey rootKey, RegistrySubPath path, RegistryKeyName name)
        {
            RegistryKey key = default;
            try
            {
                key = rootKey.Value.OpenSubKey(path.Value);
                var kind = key.GetValueKind("");
                if (kind != RegistryValueKind.String)
                    return Fail<RegistryBlobValue>(new InvalidCastException());
                return Succeed(new RegistryBlobValue { Value = (byte[])key.GetValue(name.Value) });
            }
            catch (Exception exn)
            {
                return Fail<RegistryBlobValue>(exn);
            }
            finally
            {
                key?.Close();
            }
        }
    }
}
