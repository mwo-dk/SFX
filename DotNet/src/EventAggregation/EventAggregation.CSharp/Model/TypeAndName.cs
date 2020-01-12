using System;
using static SFX.Utils.Infrastructure.HashCodeHelpers;

namespace SFX.EventAggregation.Model
{
    /// <summary>
    /// Represents the combination of a type name of an <see cref="IEventAggregator{T}"/>
    /// </summary>
    public struct TypeAndName : IEquatable<TypeAndName>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">The <see cref="Type"/></param>
        /// <param name="name">The name value</param>
        public TypeAndName(Type type, Name name)
        {
            Type = type;
            Name = name;

            _isValid = !(type is null || !name.IsValid());
            if (_isValid)
                HashCode = ComputeHashCodeForObjectArray(type, name);
            else HashCode = 0;
        }

        /// <summary>
        /// The type
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The name
        /// </summary>
        public Name Name { get; }

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
        /// <returns>If the fields are equal, then true else false</returns>
        public bool Equals(TypeAndName other)
        {
            if (!_isValid || !other._isValid)
                return false;
            return Type == other.Type && Name.Equals(other.Name);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is TypeAndName))
                return false;
            return Equals((TypeAndName)obj);
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Type?.Name}-{Name.Value}";

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode;
    }
}
