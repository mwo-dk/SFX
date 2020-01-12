using System;

namespace SFX.CSharp.System.Model
{
    /// <summary>
    /// Entity representing the path to a given file on the filesystem.
    /// </summary>
    public sealed class FolderPath : IEquatable<FolderPath>
    {
        /// <summary>
        /// The actual path
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Flag telling whether to ignore case. On windows systems, this is 
        /// how it works. On Unix-type systems it is not. Defaults to false
        /// to honor both worlds
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// Compares current <see cref="FolderPath"/> to <paramref name="other"/>
        /// </summary>
        /// <param name="other">The <see cref="FolderPath"/> to be compared against</param>
        /// <returns>True if (ignoring case) the folder paths are equal. Else false.</returns>
        public bool Equals(FolderPath other) =>
            IgnoreCase ?
            string.Compare(Value, other?.Value, StringComparison.InvariantCultureIgnoreCase) == 0 :
            string.Compare(Value, other?.Value, StringComparison.InvariantCulture) == 0;

        /// <summary>
        /// Overrides equals method
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if logically equal (ignoring case). Else false</returns>
        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is FolderPath))
                return false;
            return Equals((FolderPath)obj);
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
            (Value is null) ? 0 :
            IgnoreCase ?
            Value.ToLower().GetHashCode() :
            Value.GetHashCode();
    }
}
