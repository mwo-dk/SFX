using System;
using System.Buffers;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

namespace SFX.Random.CSharp
{
    /// <summary>
    /// Helper class to generate random entities based on the <see cref="RNGCryptoServiceProvider"/>
    /// </summary>
    public static class RandomHelpers
    {
        /// <summary>
        /// Generates a random boolean value
        /// </summary>
        /// <returns></returns>
        public static bool GetRandomBool() => GetRandomLong() % 2L == 1L;
        /// <summary>
        /// Generates a random <see cref="byte"/>
        /// </summary>
        /// <returns></returns>
        public static sbyte GetRandomSignedByte() => CreateRandom(x => (sbyte)x[0]);
        /// <summary>
        /// Generates a random <see cref="char"/>
        /// </summary>
        /// <returns></returns>
        public static char GetRandomChar() => CreateRandom(x => BitConverter.ToChar(x, 0));
        /// <summary>
        /// Generates a random <see cref="short"/>
        /// </summary>
        /// <returns></returns>
        public static short GetRandomShort() =>  CreateRandom(x => BitConverter.ToInt16(x, 0));
        /// <summary>
        /// Generates a random <see cref="int"/>
        /// </summary>
        /// <returns></returns>
        public static int GetRandomInt() => CreateRandom(x => BitConverter.ToInt32(x, 0));
        /// <summary>
        /// Generates a random <see cref="long"/>
        /// </summary>
        /// <returns></returns>
        public static long GetRandomLong() => CreateRandom(x => BitConverter.ToInt64(x, 0));
        /// <summary>
        /// Generates a random <see cref="byte"/>
        /// </summary>
        /// <returns></returns>
        public static byte GetRandomByte() => CreateRandom(x => x[0]);
        /// <summary>
        /// Generates a random <see cref="UInt16"/>
        /// </summary>
        /// <returns></returns>
        public static ushort GetRandomUnsignedShort() => CreateRandom(x => BitConverter.ToUInt16(x, 0));
        /// <summary>
        /// Generates a random <see cref="UInt32"/>
        /// </summary>
        /// <returns></returns>
        public static uint GetRandomUnsignedInt() => CreateRandom(x => BitConverter.ToUInt32(x, 0));
        /// <summary>
        /// Generates a random <see cref="UInt64"/>
        /// </summary>
        /// <returns></returns>
        public static ulong GetRandomUnsignedLong() => CreateRandom(x => BitConverter.ToUInt64(x, 0));
        /// <summary>
        /// Generates a random <see cref="float"/>
        /// </summary>
        /// <returns></returns>
        public static float GetRandomFloat() => CreateRandom(x => BitConverter.ToSingle(x, 0));
        /// <summary>
        /// Generates a random <see cref="double"/>
        /// </summary>
        /// <returns></returns>
        public static double GetRandomDouble() => CreateRandom(x => BitConverter.ToDouble(x, 0));
        /// <summary>
        /// Generates a random <see cref="decimal"/>
        /// </summary>
        /// <returns></returns>
        public static decimal GetRandomDecimal()
        {
            var data = GetRandomIntArray(4);
            return new decimal(data);
        }
        /// <summary>
        /// Generates a random <see cref="TimeSpan"/>
        /// </summary>
        /// <returns></returns>
        public static TimeSpan GetRandomTimeSpan() =>
            TimeSpan.FromTicks(GetRandomLong());

        private static readonly TimeSpan[] ValidUtcOffsets = new[]
        {
            TimeSpan.FromHours(-12.0),
            TimeSpan.FromHours(-11.0),
            TimeSpan.FromHours(-10.0),
            TimeSpan.FromHours(-9.5), // French Polynesia
            TimeSpan.FromHours(-9.0),
            TimeSpan.FromHours(-8.0),
            TimeSpan.FromHours(-7.0),
            TimeSpan.FromHours(-6.0),
            TimeSpan.FromHours(-5.0),
            TimeSpan.FromHours(-4.0),
            TimeSpan.FromHours(-3.5), // New Foundland & Labrador
            TimeSpan.FromHours(-3.0),
            TimeSpan.FromHours(-2.0),
            TimeSpan.FromHours(-1.0),
            TimeSpan.FromHours(0.0),
            TimeSpan.FromHours(1.0),
            TimeSpan.FromHours(2.0),
            TimeSpan.FromHours(3.0),
            TimeSpan.FromHours(3.5), // Iran
            TimeSpan.FromHours(4.0),
            TimeSpan.FromHours(4.5), // Afghanistan
            TimeSpan.FromHours(5.0),
            TimeSpan.FromHours(5.5), // India + Sri Lanka
            TimeSpan.FromMinutes(345.0), // Nepal (5:45)
            TimeSpan.FromHours(6.0),
            TimeSpan.FromHours(6.5), // Australia (Cocos (Keeling) Islands) + Myanmar (Burma)
            TimeSpan.FromHours(7.0),
            TimeSpan.FromHours(8.0),
            TimeSpan.FromMinutes(525.0), // Western Australia (8:45)
            TimeSpan.FromHours(9.0),
            TimeSpan.FromHours(9.5), // Australia (New South Wales, Northern Territory, Southern Australia)
            TimeSpan.FromHours(10.0),
            TimeSpan.FromHours(10.5), // Australia (New South Wales)
            TimeSpan.FromHours(11.0),
            TimeSpan.FromHours(12.0),
            TimeSpan.FromMinutes(765.0), // New Zealand (Chatham Islands) (12:45)
            TimeSpan.FromHours(13.0),
            TimeSpan.FromHours(14.0),
        };
        /// <summary>
        /// Generates a random <see cref="DateTimeOffset"/>
        /// </summary>
        /// <returns></returns>
        public static DateTimeOffset GetRandomDateTimeOffset()
        {
            var data = GetRandomIntArray(8);
            int PutInRange(int x, int min, int max) => 1 + x % (max - min);
            var year = PutInRange(data[0], 1, 9999);
            var month = PutInRange(data[1], 1, 12);
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var day = PutInRange(data[2], 1, daysInMonth);
            var hour = PutInRange(data[3], 0, 23);
            var minute = PutInRange(data[4], 0, 59);
            var second = PutInRange(data[5], 0, 59);
            var millisecond = PutInRange(data[6], 0, 999);
            var offsetIndex = PutInRange(data[7], 0, ValidUtcOffsets.Length - 1);
            var offset = ValidUtcOffsets[offsetIndex];
            return new DateTimeOffset(year, month, day, hour, minute, second, millisecond, offset);
        }

        private static int[] GetRandomIntArray(int length) =>
            Enumerable.Range(0, 4).Select(_ => GetRandomInt()).ToArray();

        /// <summary>
        /// Generates a random <see cref="sbyte"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static sbyte GetRandomSignedByteInRange(sbyte a, sbyte b) =>
            TryUntil(x => a <= x && x <= b, GetRandomSignedByte);
        /// <summary>
        /// Generates a random <see cref="char"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static char GetRandomCharInRange(char a, char b) =>
            TryUntil(x => a <= x && x <= b, GetRandomChar);
        /// <summary>
        /// Generates a random <see cref="short"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static short GetRandomShortInRange(short a, short b) =>
            TryUntil(x => a <= x && x <= b, GetRandomShort);
        /// <summary>
        /// Generates a random <see cref="int"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static int GetRandomIntInRange(int a, int b) =>
            TryUntil(x => a <= x && x <= b, GetRandomByte);
        /// <summary>
        /// Generates a random <see cref="long"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static long GetRandomLongInRange(long a, long b) =>
            TryUntil(x => a <= x && x <= b, GetRandomLong);
        /// <summary>
        /// Generates a random <see cref="byte"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static byte GetRandomByteInRange(byte a, byte b) =>
            TryUntil(x => a <= x && x <= b, GetRandomByte);
        /// <summary>
        /// Generates a random <see cref="UInt16"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static ushort GetRandomUnsignedShortInRange(ushort a, ushort b) =>
            TryUntil(x => a <= x && x <= b, GetRandomUnsignedShort);
        /// <summary>
        /// Generates a random <see cref="UInt32"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static uint GetRandomUnsignedIntInRange(uint a, uint b) =>
            TryUntil(x => a <= x && x <= b, GetRandomUnsignedInt);
        /// <summary>
        /// Generates a random <see cref="UInt64"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static ulong GetRandomUnsignedLongInRange(ulong a, ulong b) =>
            TryUntil(x => a <= x && x <= b, GetRandomUnsignedLong);
        /// <summary>
        /// Generates a random <see cref="float"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static float GetRandomFloatInRange(float a, float b) =>
            TryUntil(x => a <= x && x <= b, GetRandomFloat);
        /// <summary>
        /// Generates a random <see cref="double"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static double GetRandomDoubleInRange(double a, double b) =>
            TryUntil(x => a <= x && x <= b, GetRandomDouble);
        /// <summary>
        /// Generates a random <see cref="decimal"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static decimal GetRandomDecimalInRange(decimal a, decimal b) =>
            TryUntil(x => a <= x && x <= b, GetRandomDecimal);
        /// <summary>
        /// Generates a random <see cref="TimeSpan"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static TimeSpan GetRandomTimeSpanInRange(TimeSpan a, TimeSpan b) =>
            TryUntil(x => a <= x && x <= b, GetRandomTimeSpan);
        /// <summary>
        /// Generates a random <see cref="DateTimeOffset"/> in the range between <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">The minimum accepted value</param>
        /// <param name="b">The maximum accepted value</param>
        /// <returns></returns>
        public static DateTimeOffset GetRandomDateTimeOffsetInRange(DateTimeOffset a, DateTimeOffset b) =>
            TryUntil(x => a <= x && x <= b, GetRandomDateTimeOffset);

        private static readonly RNGCryptoServiceProvider rngCsp =
            new RNGCryptoServiceProvider();
        private static SpinLock _lock = new SpinLock();
        private static byte[] _data = new byte[8];
        private static T CreateRandom<T>(Func<byte[], T> converter)
        {
            bool lockTaken = false;
            try
            {
                _lock.TryEnter(ref lockTaken);
                if (!lockTaken)
                    throw new InvalidOperationException();
                rngCsp.GetBytes(_data);
                return converter(_data);
            }
            finally
            {
                if (lockTaken)
                    _lock.Exit(false);
            }
        }

        private static T TryUntil<T>(Predicate<T> predicate, Func<T> f)
        {
            do
            {
                var result = f();
                if (predicate(result))
                    return result;
            } while (true);
        }
    }
}
