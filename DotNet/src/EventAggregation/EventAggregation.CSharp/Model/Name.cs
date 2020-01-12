using System;
using static System.String;

namespace SFX.EventAggregation.Model
{
    /// <summary>
    /// Represents the name of an <see cref="IEventAggregator{T}"/>
    /// </summary>
    public struct Name : IEquatable<Name>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The name value</param>
        public Name(string value)
        {
            Value = value;

            _isValid = !IsNullOrWhiteSpace(value);
            if (_isValid)
                HashCode = value.GetHashCode();
            else HashCode = 0;
        }

        /// <summary>
        /// The value
        /// </summary>
        public string Value { get; }
        private readonly bool _isValid;
        /// <summary>
        /// Flag telling whether the name is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValid() => _isValid;
        /// <summary>
        /// The hash code
        /// </summary>
        internal int HashCode { get; }

        /// <summary>
        /// Resolves equality
        /// </summary>
        /// <param name="other">The other one to compare to</param>
        /// <returns>If the names are equal, then true else false</returns>
        public bool Equals(Name other)
        {
            if (!_isValid || !other._isValid)
                return false;
            return 0 == string.Compare(Value, other.Value, StringComparison.InvariantCulture);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is Name))
                return false;
            return Equals((Name)obj);
        }

        /// <inheritdoc/>
        public override string ToString() => Value;

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode;
    }
}