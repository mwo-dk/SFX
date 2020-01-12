using System;
using System.Linq;
using static System.Math;

namespace SFX.CSharp.BitHack
{
    /// <summary>
    /// Extensions methods for working on bit-arrays based on 64 bit signed <see cref="Int64"/>s
    /// </summary>
    public static class BitVectorHelpers
    {
        #region Static helper masks
        internal static readonly long[] Up =
            Enumerable.Range(0, 64)
            .Select(n => ~0L << n)
            .ToArray();
        internal static readonly long[] Down =
            Enumerable.Range(0, 64)
            .Select(n => n < 63 ? ~((~0L) << (n + 1)) : ~0L)
            .ToArray();
        #endregion

        #region Small bitvectors
        public static bool GetMask(this long vector, long mask) =>
            0L == mask || ~0L == (~mask ^ (vector & mask));
        public static long SetMask(this long vector, long mask, bool value) =>
            value ? (vector | mask) : (vector & ~mask);

        public static bool Get(this long vector, int n) => GetMask(vector, 1L << n);
        public static (bool IndexOk, bool Result) GetSafe(this long vector, int n) =>
            0 <= n && n < 64 ? (true, vector.Get(n)) : (false, default);
        public static long Set(this long vector, int n, bool value) => SetMask(vector, 1L << n, value);
        public static (bool IndexOk, long Result) SetSafe(this long vector, int n, bool value) =>
            0 <= n && n < 64 ? (true, vector.Set(n, value)) : (false, default);

        public static bool GetRange(this long vector, int i, int j)
        {
            (i, j) = j < i ? (j, i) : (i, j);
            var mask = Up[i] & Down[j];
            return mask == (mask & vector);
        }
        public static (bool IndexOk, bool Result) GetRangeSafe(this long vector, int i, int j) =>
            0 <= i && i < 64 && 0 <= j && j < 64 ? (true, GetRange(vector, i, j)) : (false, default);
        public static long SetRange(this long vector, int i, int j, bool value)
        {
            (i, j) = j < i ? (j, i) : (i, j);
            return SetMask(vector, Up[i] & Down[j], value);
        }
        public static (bool IndexOk, long Result) SetRangeSafe(this long vector, int i, int j, bool value) =>
            0 <= i && i < 64 && 0 <= j && j < 64 ? (true, SetMask(vector, Up[i] & Down[j], value)) : (false, default);

        public static string Format(this long vector, int size = 64)
        {
            if (size <= 0)
                return string.Empty;
            return new string(Enumerable.Range(0, Min(size, 64)).Select(n => Get(vector, n) ? '1' : '0')
                .ToArray());
        }
        #endregion

        #region Large bitvectors
        public static bool GetMask(this long[] vectors, long[] masks)
        {
            var result = vectors[0].GetMask(masks[0]);
            for (var n = 1;
                result && n < vectors.Length;
                result = vectors[n].GetMask(masks[n]), ++n) ;
            return result;
        }
        public static (bool Success, string Error, bool Result) GetMaskSafe(this long[] vectors, long[] masks)
        {
            if (vectors is null)
                return (false, "Vector is null", default);
            if (vectors.Length == 0)
                return (false, "Vector is empty", default);
            if (masks is null)
                return (false, "Mask is null", default);
            if (masks.Length == 0)
                return (false, "Mask is empty", default);
            if (vectors.Length != masks.Length)
                return (false, "Vector and mask do not match in size", default);
            return (true, default, GetMask(vectors, masks));
        }
        public static void SetMask(this long[] vectors, long[] masks, bool value)
        {
            for (var n = 0; n < vectors.Length; ++n)
                vectors[n] = value ? (vectors[n] | masks[n]) : (vectors[n] & ~masks[n]);
        }
        public static (bool Success, string Error) SetMaskSafe(this long[] vectors, long[] masks, bool value)
        {
            if (vectors is null)
                return (false, "Vector is null");
            if (vectors.Length == 0)
                return (false, "Vector is empty");
            if (masks is null)
                return (false, "Mask is null");
            if (masks.Length == 0)
                return (false, "Mask is empty");
            if (vectors.Length != masks.Length)
                return (false, "Vector and mask do not match in size");
            SetMask(vectors, masks, value);
            return (true, default);
        }

        public static bool Get(this long[] vectors, int n) =>
            GetMask(vectors[n / 64], 1L << (n % 64));
        public static (bool IndexOk, bool Result) GetSafe(this long[] vectors, int size, int n) =>
            !(vectors is null) && 0 <= n && n < size && 0 < size && size <= 64 * vectors.Length ?
            (true, Get(vectors, n)) : (false, default);
        public static void Set(this long[] vectors, int n, bool value) =>
            vectors[n / 64] = SetMask(vectors[n / 64], 1L << (n % 64), value);
        public static bool SetSafe(this long[] vectors, int size, int n, bool value)
        {
            if (!(vectors is null) && 0 <= n && n < size && 0 < size && size <= 64 * vectors.Length)
            {
                Set(vectors, n, value);
                return true;
            }
            return false;
        }

        public static bool GetRange(this long[] vectors, int i, int j)
        {
            (i, j) = j < i ? (j, i) : (i, j);
            var first = i / 64;
            var last = j / 64;
            if (first == last)
                return vectors[first].GetMask(Up[i % 64] & Down[j % 64]);
            else
            {
                var result = vectors[first].GetMask(Up[i % 64]);
                for (var n = first + 1;
                    result && n < last;
                    result = vectors[n].GetMask(~0L), ++n) ;
                if (result && first < last)
                    result = vectors[last].GetMask(Down[j % 64]);
                return result;
            }
        }
        public static (bool IndexOk, bool Result) GetRangeSafe(this long[] vectors, int size, int i, int j) =>
            !(vectors is null) && 0 <= i && i < size && 0 <= j && j < size && 0 < size && size <= 64 * vectors.Length ?
            (true, GetRange(vectors, i, j)) : (false, default);

        public static void SetRange(this long[] vectors, int i, int j, bool value)
        {
            (i, j) = j < i ? (j, i) : (i, j);
            var first = i / 64;
            var last = j / 64;
            var mask = Up[i % 64];
            vectors[first] = Set(vectors[first], i % 64, value);
            for (var n = first + 1; n <= last; ++n)
            {
                if (n < last)
                    vectors[n] = SetMask(vectors[n], Up[0], value);
                else vectors[n] = Set(vectors[n], j % 64, value);
            }
        }
        public static bool SetRangeSafe(this long[] vectors, int size, int i, int j, bool value)
        {
            if (!(vectors is null) && 0 <= i && i < size && 0 <= j && j < size && 0 < size && size <= 64 * vectors.Length)
            {
                SetRange(vectors, i, j, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears all bits in the provided bit vector <paramref name="vectors"/>
        /// </summary>
        /// <param name="vectors">The bit vector array to clear</param>
        public static void ClearAll(this long[] vectors)
        {
            for (var n = 0; n < vectors.Length; ++n)
                vectors[n] = 0L;
        }

        /// <summary>
        /// Sets all bits in the provided bit vector <paramref name="vectors"/>
        /// </summary>
        /// <param name="vectors">The bit vector array to set</param>
        public static void SetAll(this long[] vectors)
        {
            for (var n = 0; n < vectors.Length; ++n)
                vectors[n] = ~0L;
        }

        /// <summary>
        /// Inverts the individual components of <paramref name="x"/> and <paramref name="y"/>
        /// into the resultant <paramref name="result"/>
        /// </summary>
        /// <param name="x">The left hand side</param>
        /// <param name="result">The resultant</param>
        public static void Not(this long[] x, long[] result)
        {
            var N = Min(x.Length, result.Length);
            for (var n = 0; n < N; ++n)
                result[n] = ~x[n];
        }

        /// <summary>
        /// Inverts the individual components of <paramref name="x"/> and <paramref name="y"/>
        /// into the resultant <paramref name="result"/>
        /// </summary>
        /// <param name="x">The left hand side</param>
        /// <param name="result">The resultant</param>
        /// <returns>True if arguments are not null, non-empty and match in size</returns>
        public static bool NotSafe(this long[] x, long[] result)
        {
            if (x is null || x.Length == 0 ||
                result is null || result.Length == 0 ||
                x.Length != result.Length)
                return false;
            Not(x, result);
            return true;
        }

        /// <summary>
        /// Ands the individual components of <paramref name="x"/> and <paramref name="y"/>
        /// into the resultant <paramref name="result"/>
        /// </summary>
        /// <param name="x">The left hand side</param>
        /// <param name="y">The right hand side</param>
        /// <param name="result">The resultant</param>
        public static void And(this long[] x, long[] y, long[] result)
        {
            var N = Min(x.Length, Min(y.Length, result.Length));
            for (var n = 0; n < N; ++n)
                result[n] = x[n] & y[n];
        }

        /// <summary>
        /// Ands the individual components of <paramref name="x"/> and <paramref name="y"/>
        /// into the resultant <paramref name="result"/>
        /// </summary>
        /// <param name="x">The left hand side</param>
        /// <param name="y">The right hand side</param>
        /// <param name="result">The resultant</param>
        /// <returns>True if arguments are not null, non-empty and match in size</returns>
        public static bool AndSafe(this long[] x, long[] y, long[] result)
        {
            if (x is null || x.Length == 0 ||
                y is null || y.Length == 0 ||
                result is null || result.Length == 0 ||
                x.Length != y.Length ||
                y.Length != result.Length)
                return false;
            And(x, y, result);
            return true;
        }

        /// <summary>
        /// Ors the individual components of <paramref name="x"/> and <paramref name="y"/>
        /// into the resultant <paramref name="result"/>
        /// </summary>
        /// <param name="x">The left hand side</param>
        /// <param name="y">The right hand side</param>
        /// <param name="result">The resultant</param>
        public static void Or(this long[] x, long[] y, long[] result)
        {
            var N = Min(x.Length, Min(y.Length, result.Length));
            for (var n = 0; n < N; ++n)
                result[n] = x[n] | y[n];
        }

        /// <summary>
        /// Ands the individual components of <paramref name="x"/> and <paramref name="y"/>
        /// into the resultant <paramref name="result"/>
        /// </summary>
        /// <param name="x">The left hand side</param>
        /// <param name="y">The right hand side</param>
        /// <param name="result">The resultant</param>
        /// <returns>True if arguments are not null, non-empty and match in size</returns>
        public static bool OrSafe(this long[] x, long[] y, long[] result)
        {
            if (x is null || x.Length == 0 ||
                y is null || y.Length == 0 ||
                result is null || result.Length == 0 ||
                x.Length != y.Length ||
                y.Length != result.Length)
                return false;
            Or(x, y, result);
            return true;
        }

        /// <summary>
        /// Xors the individual components of <paramref name="x"/> and <paramref name="y"/>
        /// into the resultant <paramref name="result"/>
        /// </summary>
        /// <param name="x">The left hand side</param>
        /// <param name="y">The right hand side</param>
        /// <param name="result">The resultant</param>
        public static void Xor(this long[] x, long[] y, long[] result)
        {
            var N = Min(x.Length, Min(y.Length, result.Length));
            for (var n = 0; n < N; ++n)
                result[n] = x[n] ^ y[n];
        }

        /// <summary>
        /// Ands the individual components of <paramref name="x"/> and <paramref name="y"/>
        /// into the resultant <paramref name="result"/>
        /// </summary>
        /// <param name="x">The left hand side</param>
        /// <param name="y">The right hand side</param>
        /// <param name="result">The resultant</param>
        /// <returns>True if arguments are not null, non-empty and match in size</returns>
        public static bool XorSafe(this long[] x, long[] y, long[] result)
        {
            if (x is null || x.Length == 0 ||
                y is null || y.Length == 0 ||
                result is null || result.Length == 0 ||
                x.Length != y.Length ||
                y.Length != result.Length)
                return false;
            Xor(x, y, result);
            return true;
        }

        /// <summary>
        /// Formats the bit vector
        /// </summary>
        /// <param name="vectors">The bit vector to format</param>
        /// <param name="size">The size of the bit vector</param>
        /// <returns><paramref name="vectors"/> formatted</returns>
        public static string Format(this long[] vectors, int size)
        {
            if (vectors is null || size <= 0)
                return string.Empty;
            var count = size <= 64 ? 1 :
                (size % 64 == 0 ? size / 64 : size / 64 + 1);
            var elements = Enumerable.Range(0, Min(vectors.Length, count))
                .Select(n => n < count ? Format(vectors[n]) :
                Format(vectors[n], size % 64))
                .ToArray();
            return string.Join("", elements);
        }
        #endregion
    }
}
