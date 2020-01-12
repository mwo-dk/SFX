using System;

namespace SFX.CSharp.System.Windows.Model.Registry
{
    /// <summary>
    /// Entity representing a registry string value.
    /// </summary>
    public struct RegistryStringValue : IEquatable<RegistryStringValue>
    {
        /// <summary>
        /// The actual value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Tells whether the <see cref="HeaderValue"/> is valid
        /// </summary>
        /// <returns>If the <see cref="HeaderValue"/> is valid, the true, else false</returns>
        public bool IsValid() => !string.IsNullOrWhiteSpace(Value);

        /// <summary>
        /// Compares current <see cref="RegistryStringValue"/> to <paramref name="other"/>
        /// </summary>
        /// <param name="other">The <see cref="RegistryStringValue"/> to be compared against</param>
        /// <returns>True if the <see cref="RegistryStringValue"/>s are equal. Else false.</returns>
        public bool Equals(RegistryStringValue other) =>
            string.Compare(Value, other.Value, StringComparison.InvariantCulture) == 0;

        /// <summary>
        /// Overrides equals method
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if logically equal. Else false</returns>
        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is RegistryStringValue))
                return false;
            return Equals((RegistryStringValue)obj);
        }

        /// <summary>
        /// Override of ToString
        /// </summary>
        /// <returns>The file path</returns>
        public override string ToString() => Value;

        /// <summary>
        /// Determines the hash-code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() =>
            Value.GetHashCode();
    }
}
