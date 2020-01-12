using SFX.CSharp.ROP;
using SFX.CSharp.System.Windows.Model.Registry;

namespace SFX.CSharp.System.Infrastructure
{
    /// <summary>
    /// Interface describing the capability of queriying registry keys
    /// </summary>
    public interface IRegistryReader
    {
        /// <summary>
        /// Attempts to read the <see cref="RegistryStringValue"/> denoted by
        /// <paramref name="rootKey"/> and <paramref name="path"/>
        /// </summary>
        /// <param name="rootKey">The <see cref="RegistryRootKey"/> under which the result has to be found</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns>
        Result<RegistryStringValue>
            ReadStringValue(RegistryRootKey rootKey, RegistrySubPath path, RegistryKeyName name);

        /// <summary>
        /// Attempts to read the <see cref="RegistryBlobValue"/> denoted by
        /// <paramref name="rootKey"/> and <paramref name="path"/>
        /// </summary>
        /// <param name="rootKey">The <see cref="RegistryRootKey"/> under which the result has to be found</param>
        /// <param name="path">The <see cref="RegistrySubPath"/> under <paramref name="rootKey"/> where the result has to be found</param>
        /// <param name="name">The <see cref="RegistryKeyName"/> of the key to be read</param>
        /// <returns>The result of the query</returns
        Result<RegistryBlobValue>
            ReadBlobValue(RegistryRootKey rootKey, RegistrySubPath path, RegistryKeyName name);
    }
}
