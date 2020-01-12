using SFX.CSharp.ROP;
using System;
using System.Collections.Generic;
using System.Linq;
using static SFX.CSharp.ROP.Library;

namespace SFX.CSharp.Utils.Infrastructure
{
    /// <summary>
    /// Helpers to compute hashcodes
    /// </summary>
    public static class HashCodeHelpers
    {
        /// <summary>
        /// Computes the hash code of a sequence of integers
        /// </summary>
        /// <param name="items">The items to iterate over</param>
        /// <param name="prime1">The first prime number</param>
        /// <param name="prime2">The second prime number</param>
        /// <returns>The resulting hash code</returns>
        internal static Result<int> ComputeHashCode(this int[] items, int prime1, int prime2)
        {
            if (items is null)
                return Fail<int>(new ArgumentNullException(nameof(items)));
            if (items.Length == 0)
                return Fail<int>(new ArgumentException("Cannot compute hash code of empty array", nameof(items)));
            unchecked
            {
                var result = prime1;
                for (var n = 0; n < items.Length; ++n)
                    result = prime2 * result + items[n];
                return Succeed(result);
            }
        }

        /// <summary>
        /// Computes the hash code of a sequence of integers
        /// </summary>
        /// <param name="items">The items to iterate over</param>
        /// <returns>The resulting hash code</returns>
        public static Result<int> ComputeHashCode(this int[] items) =>
            items.ComputeHashCode(19, 31);

        /// <summary>
        /// Computes the hash code of a sequence of items
        /// </summary>
        /// <param name="items">The items to iterate over</param>
        /// <param name="prime1">The first prime number</param>
        /// <param name="prime2">The second prime number</param>
        /// <returns>The resulting hash code</returns>
        internal static Result<int> ComputeHashCode<T>(this T[] items, Func<T, int> getHash, int prime1, int prime2)
        {
            if (items is null)
                return Fail<int>(new ArgumentNullException(nameof(items)));
            if (getHash is null)
                return Fail<int>(new ArgumentNullException(nameof(getHash)));
            if (items.Length == 0)
                return Fail<int>(new ArgumentException("Cannot compute hash code of empty array", nameof(items)));
            unchecked
            {
                try
                {
                    var result = prime1;
                    for (var n = 0; n < items.Length; ++n)
                        result = prime2 * result + getHash(items[n]);
                    return Succeed(result);
                }
                catch (Exception error)
                {
                    return Fail<int>(error);
                }
            }
        }

        /// <summary>
        /// Computes the hash code of a sequence of items
        /// </summary>
        /// <param name="items">The items to iterate over</param>
        /// <returns>The resulting hash code</returns>
        public static Result<int> ComputeHashCode<T>(this T[] items, Func<T, int> getHash) =>
            items.ComputeHashCode(getHash, 19, 31);

        /// <summary>
        /// Computes the hash code of a sequence of items
        /// </summary>
        /// <param name="items">The items to iterate over</param>
        /// <param name="prime1">The first prime number</param>
        /// <param name="prime2">The second prime number</param>
        /// <returns>The resulting hash code</returns>
        internal static Result<int> ComputeHashCode<T>(this IEnumerable<T> items, Func<T, int> getHash, int prime1, int prime2)
        {
            if (items is null)
                return Fail<int>(new ArgumentNullException(nameof(items)));
            if (getHash is null)
                return Fail<int>(new ArgumentNullException(nameof(getHash)));
            if (!items.Any())
                return Fail<int>(new ArgumentException("Cannot compute hash code of empty array", nameof(items)));
            unchecked
            {
                try
                {
                    var result = prime1;
                    foreach (var item in items)
                        result = prime2 * result + getHash(item);
                    return Succeed(result);
                }
                catch (Exception error)
                {
                    return Fail<int>(error);
                }
            }
        }

        /// <summary>
        /// Computes the hash code of a sequence of items
        /// </summary>
        /// <param name="items">The items to iterate over</param>
        /// <returns>The resulting hash code</returns>
        public static Result<int> ComputeHashCode<T>(this IEnumerable<T> items, Func<T, int> getHash) =>
            items.ComputeHashCode(getHash, 19, 31);

        /// <summary>
        /// Computes the hash code of a sequence of objects
        /// </summary>
        /// <param name="items">The items to iterate over</param>
        /// <param name="prime1">The first prime number</param>
        /// <param name="prime2">The second prime number</param>
        /// <returns>The resulting hash code</returns>
        internal static Result<int> ComputeHashCodeForObjectArray(int prime1, int prime2, params object[] items)
        {
            var items_ = items?.Where(x => !(x is null))
                .Select(x => x.GetHashCode())
                .ToArray();
            return items_.ComputeHashCode(prime1, prime2);
        }

        /// <summary>
        /// Computes the hash code of a sequence of objects
        /// </summary>
        /// <param name="items">The items to iterate over</param>
        /// <returns>The resulting hash code</returns>
        public static Result<int> ComputeHashCodeForObjectArray(params object[] items) =>
            ComputeHashCodeForObjectArray(19, 31, items);
    }
}
