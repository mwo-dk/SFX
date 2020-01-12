using System;
using System.Linq;

namespace SFX.CSharp.System.Windows.Model.Registry
{
    /// <summary>
    /// Entity representing a registry string value.
    /// </summary>
    public struct RegistrySubPath : IEquatable<RegistrySubPath>
    {
        /// <summary>
        /// The actual value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Tells whether the <see cref="RegistrySubPath"/> is valid
        /// </summary>
        /// <returns>If the <see cref="RegistrySubPath"/> is valid, the true, else false</returns>
        public bool IsValid() => !string.IsNullOrWhiteSpace(Value);

        /// <summary>
        /// Creates a <see cref="RegistrySubPath"/> based on the level nesting denoted by
        /// <paramref name="subLevels"/>
        /// </summary>
        /// <param name="subLevels">The sub levels to stitch together</param>
        /// <returns>The resulting <see cref="RegistrySubPath"/></returns>
        public static (bool Success, RegistrySubPath Result)
            Create(params string[] subLevels)
        {
            if (subLevels is null || subLevels.Length == 0 ||
                subLevels.Any(x => string.IsNullOrWhiteSpace(x)))
                return (false, default);
            return (true, new RegistrySubPath { Value = string.Join("\\", subLevels) });
        }

        /// <summary>
        /// Compares current <see cref="RegistrySubPath"/> to <paramref name="other"/>
        /// </summary>
        /// <param name="other">The <see cref="RegistrySubPath"/> to be compared against</param>
        /// <returns>True if the <see cref="RegistrySubPath"/>s are equal. Else false.</returns>
        public bool Equals(RegistrySubPath other) =>
            string.Compare(Value, other.Value, StringComparison.InvariantCulture) == 0;

        /// <summary>
        /// Overrides equals method
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if logically equal. Else false</returns>
        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is RegistrySubPath))
                return false;
            return Equals((RegistrySubPath)obj);
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
