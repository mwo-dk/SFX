using System;
using System.Linq;

namespace SFX.CSharp.System.Windows.Model.Registry
{
    /// <summary>
    /// Entity representing a registry blob value.
    /// </summary>
    public struct RegistryBlobValue : IEquatable<RegistryBlobValue>
    {
        /// <summary>
        /// The actual value
        /// </summary>
        public byte[] Value { get; set; }

        /// <summary>
        /// Tells whether the <see cref="RegistryBlobValue"/> is valid
        /// </summary>
        /// <returns>If the <see cref="RegistryBlobValue"/> is valid, the true, else false</returns>
        public bool IsValid() => !(Value is null);

        /// <summary>
        /// Compares current <see cref="RegistryBlobValue"/> to <paramref name="other"/>
        /// </summary>
        /// <param name="other">The <see cref="RegistryBlobValue"/> to be compared against</param>
        /// <returns>True if the <see cref="RegistryBlobValue"/>s are equal. Else false.</returns>
        public bool Equals(RegistryBlobValue other) =>
            !(Value is null) &&
            !(other.Value is null) &&
            Value.Length == other.Value.Length &&
            Value.SequenceEqual(other.Value);

        /// <summary>
        /// Overrides equals method
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if logically equal. Else false</returns>
        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is RegistryBlobValue))
                return false;
            return Equals((RegistryBlobValue)obj);
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
