using SFX.CSharp.BitHack;
using System;
using System.Linq;
using Xunit;
using static SFX.CSharp.BitHack.BitVectorHelpers;

namespace SFX.BitHack.CSharp.Tests
{
    [Trait("Category", "Unit")]
    public class BitVectorHelperTests
    {
        private Random _rnd = new System.Random();

        #region Up
        [Fact]
        public void Up_all_bits_are_set_as_expected()
        {
            for (var n = 0; n < 64; ++n)
            {
                var up = Up[n];
                for (var i = 0; i < 64; ++i)
                {
                    var mask = At(i);
                    if (i < n)
                        Assert.Equal(0L, up & mask);
                    else
                        Assert.Equal(mask, up & mask);
                }
            }
        }
        #endregion

        #region Down
        [Fact]
        public void High_all_bits_are_set_as_expected()
        {
            for (var n = 0; n < 64; ++n)
            {
                var down = Down[n];
                for (var i = 0; i < 64; ++i)
                {
                    var mask = At(i);
                    if (i <= n)
                        Assert.Equal(mask, down & mask);
                    else
                        Assert.Equal(0L, down & mask);
                }
            }
        }
        #endregion

        #region Small bitvectors
        [Fact]
        public void GetMask_small_works_for_empty_mask()
        {
            var mask = 0L;
            for (var n = 0; n < 1000; ++n)
            {
                var vector = Rnd();
                Assert.True(vector.GetMask(mask));
            }
        }

        [Fact]
        public void GetMask_small_works()
        {
            for (var n = 0; n < 1000; ++n)
            {
                var vector = Rnd();
                var mask = vector;
                Assert.True(vector.GetMask(mask));
                if (Count(vector) == 0)
                    continue;
                vector = ClearFirstSet(vector);
                Assert.False(vector.GetMask(mask));
            }
        }

        [Fact]
        public void SetMask_small_works()
        {
            for (var n = 0; n < 64; ++n)
            {
                var mask = At(n);
                var vector = 0L;
                vector = vector.SetMask(mask, true);
                for (var i = 0; i < 64; ++i)
                {
                    if (n == i)
                        Assert.Equal(1L << n, vector & (1L << i));
                    else
                        Assert.Equal(0L, vector & (1L << i));
                }
            }
        }

        [Fact]
        public void Get_small_works()
        {
            for (var n = 0; n < 64; ++n)
            {
                var vector = 1L << n;
                for (var i = 0; i < 64; ++i)
                {
                    if (n == i)
                        Assert.True(vector.Get(i));
                    else
                        Assert.False(vector.Get(i));
                }
            }
        }

        [Fact]
        public void GetSafe_small_in_range_works()
        {
            var vector = Rnd();
            for (var n = 0; n < 64; ++n)
            {
                var (ok, result) = vector.GetSafe(n);
                Assert.True(ok);
                if (0L != (vector & At(n)))
                    Assert.True(result);
                else Assert.False(result);
            }
        }

        [Fact]
        public void GetSafe_small_below_range_works()
        {
            for (var n = -64; n < 0; ++n)
            {
                var vector = Rnd();
                var (ok, result) = vector.GetSafe(n);
                Assert.False(ok);
                Assert.Equal(default(bool), result);
            }
        }

        [Fact]
        public void GetSafe_small_above_range_works()
        {
            for (var n = 64; n < 128; ++n)
            {
                var vector = Rnd();
                var (ok, result) = vector.GetSafe(n);
                Assert.False(ok);
                Assert.Equal(default(bool), result);
            }
        }

        [Fact]
        public void Set_small_works()
        {
            for (var n = 0; n < 64; ++n)
            {
                var vector = 0L;
                vector = vector.Set(n, true);
                for (var i = 0; i < 64; ++i)
                {
                    if (n == i)
                        Assert.True(vector.Get(i));
                    else
                        Assert.False(vector.Get(i));
                }
            }

            for (var n = 0; n < 64; ++n)
            {
                var vector = ~0L;
                vector = vector.Set(n, false);
                for (var i = 0; i < 64; ++i)
                {
                    if (n == i)
                        Assert.False(vector.Get(i));
                    else
                        Assert.True(vector.Get(i));
                }
            }
        }

        [Fact]
        public void SetSafe_small_in_range_works()
        {
            for (var n = 0; n < 64; ++n)
            {
                var vector = 0L;
                var (ok, result) = vector.SetSafe(n, true);
                Assert.True(ok);
                for (var i = 0; i < 64; i++)
                {
                    if (n == i)
                        Assert.True(result.Get(i));
                    else
                        Assert.False(result.Get(i));
                }
            }

            for (var n = 0; n < 64; ++n)
            {
                var vector = ~0L;
                var (ok, result) = vector.SetSafe(n, false);
                Assert.True(ok);
                for (var i = 0; i < 64; i++)
                {
                    if (n == i)
                        Assert.False(result.Get(i));
                    else
                        Assert.True(result.Get(i));
                }
            }
        }

        [Fact]
        public void SetSafe_small_below_range_works()
        {
            for (var n = -64; n < 0; ++n)
            {
                var vector = Rnd();
                var (ok, result) = vector.SetSafe(n, true);
                Assert.False(ok);
                Assert.Equal(default(long), result);
            }

            for (var n = -64; n < 0; ++n)
            {
                var vector = Rnd();
                var (ok, result) = vector.SetSafe(n, false);
                Assert.False(ok);
                Assert.Equal(default(long), result);
            }
        }

        [Fact]
        public void SetSafe_small_above_range_works()
        {
            for (var n = 64; n < 128; ++n)
            {
                var vector = Rnd();
                var (ok, result) = vector.SetSafe(n, true);
                Assert.False(ok);
                Assert.Equal(default(long), result);
            }

            for (var n = 64; n < 128; ++n)
            {
                var vector = Rnd();
                var (ok, result) = vector.SetSafe(n, false);
                Assert.False(ok);
                Assert.Equal(default(long), result);
            }
        }

        [Fact]
        public void GetRange_small_works()
        {
            var vector = 0L;
            for (var i = 0; i < 64; ++i)
                for (var j = i; j < 64; ++j)
                    Assert.False(vector.GetRange(i, j));

            vector = ~0L;
            for (var i = 0; i < 64; ++i)
                for (var j = i; j < 64; ++j)
                    Assert.True(vector.GetRange(i, j));

            for (var i = 0; i < 64; ++i)
            {
                vector = 0L;
                for (var j = i; j < 64; ++j)
                {
                    vector = vector.Set(j, true);
                    for (var k = i; k <= j; ++k)
                        for (var l = k; l <= j; ++l)
                            Assert.True(vector.GetRange(k, l));
                    if (i > 0)
                        for (var k = 0; k < i; ++k)
                            for (var l = k + 1; l <= j; ++l)
                                Assert.False(vector.GetRange(k, l));
                    if (j < 63)
                    {
                        for (var k = i; k <= j; ++k)
                            for (var l = j + 1; l <= j; ++l)
                                Assert.False(vector.GetRange(k, l));
                    }
                }
            }

            for (var i = 0; i < 64; ++i)
            {
                vector = (~0L).Set(i, false);
                Assert.False(vector.GetRange(0, 63));
            }
        }

        [Fact]
        public void GetRangeSafe_small_works()
        {
            var vector = 0L;
            for (var i = 0; i < 64; ++i)
                for (var j = i; j < 64; ++j)
                {
                    var (ok, result) = vector.GetRangeSafe(i, j);
                    Assert.True(ok);
                    Assert.False(result);
                }

            vector = ~0L;
            for (var i = 0; i < 64; ++i)
                for (var j = i; j < 64; ++j)
                {
                    var (ok, result) = vector.GetRangeSafe(i, j);
                    Assert.True(ok);
                    Assert.True(result);
                }

            for (var i = 0; i < 64; ++i)
            {
                vector = 0L;
                for (var j = i; j < 64; ++j)
                {
                    vector = vector.Set(j, true);
                    for (var k = i; k <= j; ++k)
                        for (var l = k; l <= j; ++l)
                        {
                            var (ok, result) = vector.GetRangeSafe(k, l);
                            Assert.True(ok);
                            Assert.True(result);
                        }
                    if (i > 0)
                        for (var k = 0; k < i; ++k)
                            for (var l = k + 1; l <= j; ++l)
                            {
                                var (ok, result) = vector.GetRangeSafe(k, l);
                                Assert.True(ok);
                                Assert.False(result);
                            }
                    if (j < 63)
                    {
                        for (var k = i; k <= j; ++k)
                            for (var l = j + 1; l <= j; ++l)
                            {
                                var (ok, result) = vector.GetRangeSafe(k, l);
                                Assert.True(ok);
                                Assert.False(result);
                            }
                    }
                }
            }

            for (var i = 0; i < 64; ++i)
            {
                vector = (~0L).Set(i, false);
                var (ok, result) = vector.GetRangeSafe(0, 63);
                Assert.True(ok);
                Assert.False(result);
            }
        }

        [Fact]
        public void GetRangeSafe_small_below_range_works()
        {
            for (var n = -64; n < 0; ++n)
            {
                var vector = Rnd();
                var (ok, result) = vector.GetRangeSafe(n, 63);
                Assert.False(ok);
                Assert.Equal(default(bool), result);
            }
        }

        [Fact]
        public void GetRangeSafe_small_above_range_works()
        {
            for (var n = 64; n < 128; ++n)
            {
                var vector = Rnd();
                var (ok, result) = vector.GetRangeSafe(0, n);
                Assert.False(ok);
                Assert.Equal(default(bool), result);
            }
        }

        [Fact]
        public void Format_small_works()
        {
            var vector = 0L;
            for (var n = 1; n < 64; ++n)
            {
                vector = Rnd() & Down[n];
                var fmt = vector.Format(n);
                Assert.Equal(n, fmt.Length);
                for (var i = 0; i < n; ++i)
                {
                    var on = 0L != (vector & (1L << i));
                    if (on)
                        Assert.Equal('1', fmt[i]);
                    else
                        Assert.Equal('0', fmt[i]);
                }
            }

            vector = Rnd();
            var fmt_ = vector.Format(65);
            Assert.Equal(64, fmt_.Length);
        }
        #endregion

        #region Large bitvectors
        [Fact]
        public void GetMask_large_works_for_empty_mask()
        {
            var mask = _0(3);
            for (var n = 0; n < 1000; ++n)
            {
                var vector = RndLarge(3);
                Assert.True(vector.GetMask(mask));
            }
        }

        [Fact]
        public void GetMask_large_works()
        {
            for (var n = 0; n < 1000; ++n)
            {
                var vector = RndLarge(3);
                var mask = vector.Select(x => x).ToArray();
                Assert.True(vector.GetMask(mask));
                if (Count(vector) == 0)
                    continue;
                ClearFirstSet(vector);
                Assert.False(vector.GetMask(mask));
            }
        }

        [Fact]
        public void SetMask_large_works()
        {
            for (var j = 0; j < 3; ++j)
                for (var n = 0; n < 64; ++n)
                {
                    var mask = _0(3);
                    mask[j] = At(n);
                    var vector = _0(3);
                    vector.SetMask(mask, true);
                    for (var i = 0; i < 64; ++i)
                    {
                        if (n == i)
                            Assert.Equal(1L << n, vector[j] & (1L << i));
                        else
                            Assert.Equal(0L, vector[j] & (1L << i));
                    }
                }
        }

        [Fact]
        public void Get_large_works()
        {
            for (var j = 0; j < 3; ++j)
                for (var n = 0; n < 64; ++n)
                {
                    var vector = _0(3);
                    vector[j] = 1L << n;
                    for (var i = 0; i < 64; ++i)
                    {
                        if (n == i)
                            Assert.True(vector.Get(j * 64 + i));
                        else
                            Assert.False(vector.Get(j * 64 + i));
                    }
                }
        }

        [Fact]
        public void GetSafe_large_in_range_works()
        {
            var vector = RndLarge(3);
            for (var i = 0; i < 3; ++i)
                for (var n = 0; n < 64; ++n)
                {
                    var (ok, result) = vector.GetSafe(3 * 64, i * 64 + n);
                    Assert.True(ok);
                    if (0L != (vector[i] & At(n)))
                        Assert.True(result);
                    else Assert.False(result);
                }
        }

        [Fact]
        public void GetSafe_large_below_range_works()
        {
            for (var n = -64; n < 0; ++n)
            {
                var vector = RndLarge(3);
                var (ok, result) = vector.GetSafe(3 * 64, n);
                Assert.False(ok);
                Assert.Equal(default(bool), result);
            }
        }

        [Fact]
        public void GetSafe_large_above_range_works()
        {
            for (var n = 192; n < 256; ++n)
            {
                var vector = RndLarge(3);
                var (ok, result) = vector.GetSafe(3 * 64, n);
                Assert.False(ok);
                Assert.Equal(default(bool), result);
            }
        }

        [Fact]
        public void Set_large_works()
        {
            for (var j = 0; j < 3; ++j)
                for (var n = 0; n < 64; ++n)
                {
                    var vector = _0(3);
                    vector.Set(j * 64 + n, true);
                    for (var i = 0; i < 64; ++i)
                    {
                        if (n == i)
                            Assert.True(vector.Get(j * 64 + i));
                        else
                            Assert.False(vector.Get(j * 64 + i));
                    }
                }

            for (var j = 0; j < 3; ++j)
                for (var n = 0; n < 64; ++n)
                {
                    var vector = _1(3);
                    vector.Set(j * 64 + n, false);
                    for (var i = 0; i < 64; ++i)
                    {
                        if (n == i)
                            Assert.False(vector.Get(j * 64 + i));
                        else
                            Assert.True(vector.Get(j * 64 + i));
                    }
                }
        }

        [Fact]
        public void SetSafe_large_in_range_works()
        {
            for (var j = 0; j < 3; ++j)
                for (var n = 0; n < 64; ++n)
                {
                    var vector = _0(3);
                    var ok = vector.SetSafe(3 * 64, j * 64 + n, true);
                    Assert.True(ok);
                    for (var i = 0; i < 64; i++)
                    {
                        if (n == i)
                            Assert.True(vector.Get(j * 64 + i));
                        else
                            Assert.False(vector.Get(j * 64 + i));
                    }
                }

            for (var j = 0; j < 3; ++j)
                for (var n = 0; n < 64; ++n)
                {
                    var vector = _1(3);
                    var ok = vector.SetSafe(3 * 64, j * 64 + n, false);
                    Assert.True(ok);
                    for (var i = 0; i < 64; i++)
                    {
                        if (n == i)
                            Assert.False(vector.Get(j * 64 + i));
                        else
                            Assert.True(vector.Get(j * 64 + i));
                    }
                }
        }

        [Fact]
        public void SetSafe_large_below_range_works()
        {
            for (var n = -64; n < 0; ++n)
            {
                var vector = RndLarge(3);
                var ok = vector.SetSafe(3 * 64, n, true);
                Assert.False(ok);
            }

            for (var n = -64; n < 0; ++n)
            {
                var vector = RndLarge(3);
                var ok = vector.SetSafe(3 * 64, n, false);
                Assert.False(ok);
            }
        }

        [Fact]
        public void SetSafe_large_above_range_works()
        {
            for (var n = 192; n < 256; ++n)
            {
                var vector = RndLarge(3);
                var ok = vector.SetSafe(3 * 64, n, true);
                Assert.False(ok);
            }

            for (var n = 192; n < 256; ++n)
            {
                var vector = RndLarge(3);
                var ok = vector.SetSafe(3 * 46, n, false);
                Assert.False(ok);
            }
        }

        [Fact]
        public void GetRange_large_works()
        {
            var vector = _0(3);
            for (var i = 0; i < 192; ++i)
                for (var j = i; j < 192; ++j)
                    Assert.False(vector.GetRange(i, j));

            vector = _1(3);
            for (var i = 0; i < 192; ++i)
                for (var j = i; j < 192; ++j)
                    Assert.True(vector.GetRange(i, j));

            for (var i = 0; i < 192; ++i)
            {
                vector = _0(3);
                for (var j = i; j < 192; ++j)
                {
                    vector.Set(j, true);
                    for (var k = i; k <= j; ++k)
                        for (var l = k; l <= j; ++l)
                            Assert.True(vector.GetRange(k, l));
                    if (i > 0)
                        for (var k = 0; k < i; ++k)
                            for (var l = k + 1; l <= j; ++l)
                                Assert.False(vector.GetRange(k, l));
                    if (j < 191)
                    {
                        for (var k = i; k <= j; ++k)
                            for (var l = j + 1; l <= j; ++l)
                                Assert.False(vector.GetRange(k, l));
                    }
                }
            }

            for (var i = 0; i < 192; ++i)
            {
                vector = _1(3);
                vector.Set(i, false);
                Assert.False(vector.GetRange(0, 191));
            }
        }

        [Fact]
        public void GetRangeSafe_large_works()
        {
            var vector = _0(3);
            for (var i = 0; i < 192; ++i)
                for (var j = i; j < 192; ++j)
                {
                    var (ok, result) = vector.GetRangeSafe(192, i, j);
                    Assert.True(ok);
                    Assert.False(result);
                }

            vector = _1(3);
            for (var i = 0; i < 192; ++i)
                for (var j = i; j < 192; ++j)
                {
                    var (ok, result) = vector.GetRangeSafe(192, i, j);
                    Assert.True(ok);
                    Assert.True(result);
                }

            for (var i = 0; i < 192; ++i)
            {
                vector = _0(3);
                for (var j = i; j < 192; ++j)
                {
                    vector.Set(j, true);
                    for (var k = i; k <= j; ++k)
                        for (var l = k; l <= j; ++l)
                        {
                            var (ok, result) = vector.GetRangeSafe(192, k, l);
                            Assert.True(ok);
                            Assert.True(result);
                        }
                    if (i > 0)
                        for (var k = 0; k < i; ++k)
                            for (var l = k + 1; l <= j; ++l)
                            {
                                var (ok, result) = vector.GetRangeSafe(192, k, l);
                                Assert.True(ok);
                                Assert.False(result);
                            }
                    if (j < 191)
                    {
                        for (var k = i; k <= j; ++k)
                            for (var l = j + 1; l <= j; ++l)
                            {
                                var (ok, result) = vector.GetRangeSafe(192, k, l);
                                Assert.True(ok);
                                Assert.False(result);
                            }
                    }
                }
            }

            for (var i = 0; i < 192; ++i)
            {
                vector = _1(3);
                vector.Set(i, false);
                var (ok, result) = vector.GetRangeSafe(192, 0, 191);
                Assert.True(ok);
                Assert.False(result);
            }
        }

        [Fact]
        public void GetRangeSafe_large_below_range_works()
        {
            for (var n = -192; n < 0; ++n)
            {
                var vector = RndLarge(3);
                var (ok, result) = vector.GetRangeSafe(192, n, 191);
                Assert.False(ok);
                Assert.Equal(default(bool), result);
            }
        }

        [Fact]
        public void GetRangeSafe_large_above_range_works()
        {
            for (var n = 192; n < 256; ++n)
            {
                var vector = RndLarge(3);
                var (ok, result) = vector.GetRangeSafe(192, 0, n);
                Assert.False(ok);
                Assert.Equal(default(bool), result);
            }
        }

        [Fact]
        public void ClearAll_works()
        {
            var vector = RndLarge(3);
            vector.ClearAll();
            foreach (var x in vector)
                Assert.Equal(0L, x);
        }

        [Fact]
        public void SetAll_works()
        {
            var vector = RndLarge(3);
            vector.SetAll();
            foreach (var x in vector)
                Assert.Equal(~0L, x);
        }

        [Fact]
        public void Not_works()
        {
            var vector = RndLarge(3);
            var result = vector.Select(x => x).ToArray();
            result.Not(result);
            for (var n = 0; n < 3; ++n)
                vector[n] = ~result[n];
        }

        [Fact]
        public void NotSafe_works()
        {
            Assert.False(BitVectorHelpers.NotSafe(null, RndLarge(3)));
            Assert.False(BitVectorHelpers.NotSafe(RndLarge(3), null));
            Assert.False(BitVectorHelpers.NotSafe(RndLarge(2), RndLarge(3)));
            var vector = RndLarge(3);
            var result = vector.Select(x => x).ToArray();
            var ok = result.NotSafe(result);
            Assert.True(ok);
            for (var n = 0; n < 3; ++n)
                vector[n] = ~result[n];
        }

        [Fact]
        public void And_works()
        {
            var x = RndLarge(3);
            var y = RndLarge(3);
            var z = _0(3);
            x.And(y, z);
            for (var n = 0; n < 3; ++n)
                z[n] = x[n] & y[n];
        }

        [Fact]
        public void AndSafe_works()
        {
            long[][] args = new[]
            {
                null,
                RndLarge(2),
                RndLarge(3)
            };
            for (var i = 0; i < 3; ++i)
                for (var j = 0; j < 3; ++j)
                    for (var k = 0; k < 3; ++k)
                        if (i * j * k != 1 && i * j * k != 8)
                            Assert.False(BitVectorHelpers.AndSafe(args[i], args[j], args[k]));

            var x = RndLarge(3);
            var y = RndLarge(3);
            var z = _0(3);
            var ok = x.AndSafe(y, z);
            Assert.True(ok);
            for (var n = 0; n < 3; ++n)
                z[n] = x[n] & y[n];
        }

        [Fact]
        public void Or_works()
        {
            var x = RndLarge(3);
            var y = RndLarge(3);
            var z = _0(3);
            x.Or(y, z);
            for (var n = 0; n < 3; ++n)
                z[n] = x[n] | y[n];
        }

        [Fact]
        public void OrSafe_works()
        {
            long[][] args = new[]
            {
                null,
                RndLarge(2),
                RndLarge(3)
            };
            for (var i = 0; i < 3; ++i)
                for (var j = 0; j < 3; ++j)
                    for (var k = 0; k < 3; ++k)
                        if (i * j * k != 1 && i * j * k != 8)
                            Assert.False(BitVectorHelpers.AndSafe(args[i], args[j], args[k]));

            var x = RndLarge(3);
            var y = RndLarge(3);
            var z = _0(3);
            var ok = x.OrSafe(y, z);
            Assert.True(ok);
            for (var n = 0; n < 3; ++n)
                z[n] = x[n] | y[n];
        }

        [Fact]
        public void Xor_works()
        {
            var x = RndLarge(3);
            var y = RndLarge(3);
            var z = _0(3);
            x.Xor(y, z);
            for (var n = 0; n < 3; ++n)
                z[n] = x[n] ^ y[n];
        }

        [Fact]
        public void XorSafe_works()
        {
            long[][] args = new[]
            {
                null,
                RndLarge(2),
                RndLarge(3)
            };
            for (var i = 0; i < 3; ++i)
                for (var j = 0; j < 3; ++j)
                    for (var k = 0; k < 3; ++k)
                        if (i * j * k != 1 && i * j * k != 8)
                            Assert.False(BitVectorHelpers.AndSafe(args[i], args[j], args[k]));

            var x = RndLarge(3);
            var y = RndLarge(3);
            var z = _0(3);
            var ok = x.XorSafe(y, z);
            Assert.True(ok);
            for (var n = 0; n < 3; ++n)
                z[n] = x[n] | y[n];
        }

        [Fact]
        public void Format_large_works()
        {
            var vector = 0L;
            for (var n = 1; n < 64; ++n)
            {
                vector = Rnd() & Down[n];
                var fmt = vector.Format(n);
                Assert.Equal(n, fmt.Length);
                for (var i = 0; i < n; ++i)
                {
                    var on = 0L != (vector & (1L << i));
                    if (on)
                        Assert.Equal('1', fmt[i]);
                    else
                        Assert.Equal('0', fmt[i]);
                }
            }

            vector = Rnd();
            var fmt_ = vector.Format(65);
            Assert.Equal(64, fmt_.Length);
        }
        #endregion

        #region Utility
        private static long At(int n) => 1L << n;
        private static int Count(long vector) =>
            Enumerable.Range(0, 64).Count(n => 0L != (At(n) & vector));
        private static int Count(long[] vector) =>
            vector.Sum(x => Count(x));
        private static long ClearFirstSet(long vector)
        {
            var first = Enumerable.Range(0, 64).First(n => 0L != (At(n) & vector));
            return vector & ~At(first);
        }
        private static void ClearFirstSet(long[] vector)
        {
            for (var n = 0; n < vector.Length; ++n)
            {
                if (Count(vector[n]) == 0L)
                    continue;
                var first = Enumerable.Range(0, 64).First(i => 0L != (At(i) & vector[n]));
                vector[n] = vector[n] & ~At(first);
                return;
            }
        }
        private long Rnd() => (((long)_rnd.Next()) << 32) | ((long)_rnd.Next());
        private long[] RndLarge(int size) =>
            Enumerable.Range(0, size)
            .Select(_ => Rnd())
            .ToArray();
        private long[] _0(int size) =>
            Enumerable.Range(0, size)
            .Select(_ => 0L)
            .ToArray();
        private long[] _1(int size) =>
            Enumerable.Range(0, size)
            .Select(_ => ~0L)
            .ToArray();
        #endregion
    }
}
