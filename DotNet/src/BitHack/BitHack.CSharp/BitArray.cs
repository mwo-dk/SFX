using SFX.CSharp.ROP;
using System;
using System.Collections;
using System.Collections.Generic;
using static SFX.CSharp.ROP.Library;

namespace SFX.CSharp.BitHack
{
    /// <summary>
	/// Simple contract-less bit array class. In order to avoid usage of many nullable (extra bool flag)
	/// in each <see cref="MinuteBasedContainer{T}"/>. The API is also exception-free.
	/// Basic logic is taken from ref-implementation: https://referencesource.microsoft.com/#mscorlib/system/collections/bitarray.cs,90295d431f28b046
	/// but a simpler to-the-purpose exception free API has been decided upon
	/// </summary>
	public sealed class BitArray : IEnumerable<bool>
    {
        private static IndexOutOfRangeException IndexOutOfRange =
            new IndexOutOfRangeException();

        private BitArray() { }

        internal readonly int Length;
        internal readonly long[] Data;

        private BitArray(Span<long> data)
        {
            Length = data.Length;

            Data = new long[Length];
            data.CopyTo(Data);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="length">The length of the bit array</param>
        /// <param name="defaultValue">The default value</param>
        public BitArray(int length, bool defaultValue)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            Length = length;
            Data = new long[GetArrayLength(length, 64)];

            if (defaultValue)
                for (var i = 0; i < Data.Length; ++i)
                    Data[i] = ~0L;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="length">The length of the bit array</param>
        public BitArray(int length) : this(length, false) { }

        /// <summary>
        /// Gets the boolean value of the <paramref name="n"/>th position
        /// </summary>
        /// <param name="n">The position of the value</param>
        /// <returns>The boolean value of the <paramref name="n"/>th position</returns>
        public Result<bool> Get(int n)
        {
            var (ok, result) = Data.GetSafe(Length, n);
            return ok ? Succeed(result) :
                Fail<bool>(IndexOutOfRange);
        }

        /// <summary>
        /// Sets the boolean value of the <paramref name="n"/>th position
        /// </summary>
        /// <param name="n">The position of the value</param>
        /// <param name="value">The value</param>
        /// <returns>True if n is a valid position. Else false</returns>
        public Result<Unit> Set(int n, bool value)
        {
            var ok = Data.SetSafe(Length, n, value);
            return ok ? Succeed(Unit.Value) :
                Fail<Unit>(IndexOutOfRange);
        }

        /// <summary>
        /// Checks whether all values in the provided index range <paramref name="from"/>
        /// to <paramref name="to"/> are set
        /// </summary>
        /// <param name="from">First index in the range</param>
        /// <param name="to">Last index in the range</param>
        /// <returns>True if all bits are set</returns>
        public Result<bool> GetRange(int from, int to)
        {
            var (ok, result) = Data.GetRangeSafe(Length, from, to);
            return ok ? Succeed(result) :
                Fail<bool>(IndexOutOfRange);
        }

        /// <summary>
        /// Sets all bits in the index range <paramref name="from"/> to <paramref name="to"/>
        /// to either 1 (if <paramref name="value"/> is true) or 0 (if <paramref name="value"/> 
        /// is false)
        /// </summary>
        /// <param name="from">First index in the range</param>
        /// <param name="to">Last index in the range</param>
        /// <param name="value">Flag telling whether to set or clear the bits in the range</param>
        /// <returns>True if the index range is valid. Else false</returns>
        public Result<Unit> SetRange(int from, int to, bool value)
        {
            var ok = Data.SetRangeSafe(Length, from, to, value);
            return ok ? Succeed(Unit.Value) :
                Fail<Unit>(IndexOutOfRange);
        }

        /// <summary>
        /// Clears the bit array
        /// </summary>
        public void Clear() =>
            Data.ClearAll();

        /// <summary>
        /// Sets all bits
        /// </summary>
        public void Set() =>
            Data.SetAll();

        /// <summary>
        /// Inverts all bits
        /// </summary>
        public void Invert() =>
            Data.Not(Data);

        /// <summary>
        /// Returns a new <see cref="BitArray"/> that is an inverted edition of <paramref name="x"/>
        /// </summary>
        /// <param name="x">The argument</param>
        /// <returns><paramref name="x"/> inverted</returns>
        public static BitArray operator !(BitArray x)
        {
            if (x is null)
                throw new ArgumentNullException(nameof(x));

            var result = new BitArray(x.Data);
            result.Invert();
            return result;
        }

        /// <summary>
        /// Returns a new <see cref="BitArray"/> that represents the bitwise "and" of
        /// <paramref name="x"/> and <paramref name="y"/>
        /// </summary>
        /// <param name="x">The left hand side</param>
        /// <param name="y">The right hand side</param>
        /// <returns><paramref name="x"/> and <paramref name="y"/> bitwise "and"'ed</returns>
        public static BitArray operator &(BitArray x, BitArray y)
        {
            if (x is null)
                throw new ArgumentNullException(nameof(x));
            if (y is null)
                throw new ArgumentNullException(nameof(y));
            if (x.Length != y.Length)
                throw new ArgumentOutOfRangeException(nameof(y));
            var result = new BitArray(x.Length);
            x.Data.And(y.Data, result.Data);
            return result;
        }

        /// <summary>
        /// Returns a new <see cref="BitArray"/> that represents the bitwise "or" of
        /// <paramref name="x"/> and <paramref name="y"/>
        /// </summary>
        /// <param name="x">The left hand side</param>
        /// <param name="y">The right hand side</param>
        /// <returns><paramref name="x"/> and <paramref name="y"/> bitwise "or"'ed</returns>
        public static BitArray operator |(BitArray x, BitArray y)
        {
            if (x is null)
                throw new ArgumentNullException(nameof(x));
            if (y is null)
                throw new ArgumentNullException(nameof(y));
            if (x.Length != y.Length)
                throw new ArgumentOutOfRangeException(nameof(y));
            var result = new BitArray(x.Length);
            x.Data.Or(y.Data, result.Data);
            return result;
        }

        /// <summary>
        /// Returns a new <see cref="BitArray"/> that represents the bitwise "xor" of
        /// <paramref name="x"/> and <paramref name="y"/>
        /// </summary>
        /// <param name="x">The left hand side</param>
        /// <param name="y">The right hand side</param>
        /// <returns><paramref name="x"/> and <paramref name="y"/> bitwise "xor"'ed</returns>
        public static BitArray operator ^(BitArray x, BitArray y)
        {
            if (x is null)
                throw new ArgumentNullException(nameof(x));
            if (y is null)
                throw new ArgumentNullException(nameof(y));
            if (x.Length != y.Length)
                throw new ArgumentOutOfRangeException(nameof(y));
            var result = new BitArray(x.Length);
            x.Data.Xor(y.Data, result.Data);
            return result;
        }

        /// <inheritdoc/>
        public IEnumerator<bool> GetEnumerator() =>
            new BitArrayEnumerator(this);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() =>
            new BitArrayEnumerator(this);

        private static int GetArrayLength(int n, int div) =>
            n > 0 ? ((n - 1) / div) + 1 : 0;

        private sealed class BitArrayEnumerator : IEnumerator<bool>, IEnumerator
        {
            private readonly BitArray _bitArray;
            private int _index;

            internal BitArrayEnumerator(BitArray bitArray)
            {
                _bitArray = bitArray;
                _index = -1;
            }

            public bool MoveNext()
            {
                if (_index < _bitArray.Length - 1)
                {
                    ++_index;
                    return true;
                }
                else _index = _bitArray.Length;
                return false;
            }

            public bool Current
            {
                get
                {
                    if (_index == -1 || _bitArray.Length <= _index)
                        throw new InvalidOperationException();
                    return _bitArray.Get(_index);
                }
            }

            object IEnumerator.Current => this.Current;

            public void Reset() => _index = -1; // No version check like in the ref source

            public void Dispose() { }
        }
    }
}
