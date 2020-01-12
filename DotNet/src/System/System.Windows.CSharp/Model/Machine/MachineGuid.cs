using System;

namespace SFX.CSharp.System.Windows.Model.Machine
{
    /// <summary>
    /// Entity representing a registry string value.
    /// </summary>
    public struct MachineGuid : IEquatable<MachineGuid>
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
        /// Compares current <see cref="MachineGuid"/> to <paramref name="other"/>
        /// </summary>
        /// <param name="other">The <see cref="MachineGuid"/> to be compared against</param>
        /// <returns>True if the <see cref="MachineGuid"/>s are equal. Else false.</returns>
        public bool Equals(MachineGuid other) =>
            string.Compare(Value, other.Value, StringComparison.InvariantCulture) == 0;

        /// <summary>
        /// Overrides equals method
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if logically equal. Else false</returns>
        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is MachineGuid))
                return false;
            return Equals((MachineGuid)obj);
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
