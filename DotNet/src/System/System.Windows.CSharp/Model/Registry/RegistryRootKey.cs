using Microsoft.Win32;
using System;

namespace SFX.CSharp.System.Windows.Model.Registry
{
    /// <summary>
    /// Struct representing root elements in the registry hive.
    /// </summary>
    public struct RegistryRootKey : IEquatable<RegistryRootKey>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The root <see cref="RegistryKey"/></param>
        internal RegistryRootKey(RegistryKey value) =>
            Value = value ?? throw new ArgumentNullException(nameof(value));

        /// <summary>
        /// The actual <see cref="RegistryKey"/>
        /// </summary>
        public RegistryKey Value { get; }

        /// <summary>
        /// Compares current <see cref="RegistryRootKey"/> to <paramref name="other"/>
        /// </summary>
        /// <param name="other">The <see cref="RegistryRootKey"/> to be compared against</param>
        /// <returns>True if the <see cref="RegistryRootKey"/>s are equal. Else false.</returns>
        public bool Equals(RegistryRootKey other) =>
            ReferenceEquals(Value, other.Value);

        /// <summary>
        /// Overrides equals method
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if logically equal. Else false</returns>
        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is RegistryRootKey))
                return false;
            return Equals((RegistryRootKey)obj);
        }

        /// <summary>
        /// Override of ToString
        /// </summary>
        /// <returns>The file path</returns>
        public override string ToString() => Value.ToString();

        /// <summary>
        /// Determines the hash-code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() =>
            Value.GetHashCode();
    }
}
