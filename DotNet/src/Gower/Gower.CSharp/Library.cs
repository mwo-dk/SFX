namespace SFX
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using static System.Double;
    using static System.Math;

    public class GowerConfiguration<T>
    {
        public List<Func<T, bool>> AsymmetricDichotomous { get; set; }
        public List<Func<T, bool>> SymmetricDichotomous { get; set; }
        public List<Func<T, int>> Qualitative { get; set; }
        public List<Func<T, double>> Quantitative { get; set; }
    }

    public static class Gower
    {
        internal static readonly (double, bool) _0_0 = (0.0, false);
        internal static readonly (double, bool) _0_1 = (0.0, true);
        internal static readonly (double, bool) _1_0 = (1.0, false);
        internal static readonly (double, bool) _1_1 = (1.0, true);

        internal static bool IsNumber(double x) =>
            !IsNaN(x) &&
            !IsPositiveInfinity(x) &&
            !IsNegativeInfinity(x) &&
            x != Epsilon &&
            x != MinValue &&
            x != MaxValue;

        internal static (double, bool) DichotomousAsymmetricSimilarity(bool x, bool y) =>
            x && y ? _1_1 : (x != y ? _0_1 : _0_0);
        internal static (double, bool) DichotomousSymmetricSimilarity(bool x, bool y) =>
            x == y ? _1_1 : _0_1;
        internal static (double, bool) QualitativeSimilarity<T>(T x, T y)
            where T : IEquatable<T> =>
            x.Equals(y) ? _1_1 : _0_1;
        internal static (double, bool) QuantitativeSimilarity(double x, double y, double range) 
        {
            if (range == 0.0)
                return (NaN, false);
            if (IsNumber(x) && IsNumber(y) && IsNumber(range))
                return (1.0 - Abs(x - y) / range, true);
            else return (NaN, false);
        }
        internal static (double, bool) GetRange(IEnumerable<(double, bool)> x)
        {
            var valid = x.Where(y => IsNumber(y.Item1) && y.Item2).Select(y => y.Item1);
            if (!valid.Any())
                return (NaN, false);
            var first = valid.First();
            var (a, b) =
                valid
                .Skip(1)
                .Aggregate((Min: first, Max: first), (x, y) => (Min(x.Min, y), Max(x.Max, y)));
            return (b - a, true);
        }

        public static GowerConfiguration<T> 
            WithAsymmetricDichotomousCharacter<T>(this GowerConfiguration<T> conf, 
            Func<T, bool> character)
        {
            if (character is null)
                throw new ArgumentNullException(nameof(character));
            var result = conf ?? new GowerConfiguration<T>();
            if (result.AsymmetricDichotomous is null)
                result.AsymmetricDichotomous = new List<Func<T, bool>> { character };
            else result.AsymmetricDichotomous.Add(character);
            return result;
        }

        public static GowerConfiguration<T>
            WithSymmetricDichotomousCharacter<T>(this GowerConfiguration<T> conf,
            Func<T, bool> character)
        {
            if (character is null)
                throw new ArgumentNullException(nameof(character));
            var result = conf ?? new GowerConfiguration<T>();
            if (result.SymmetricDichotomous is null)
                result.SymmetricDichotomous = new List<Func<T, bool>> { character };
            else result.SymmetricDichotomous.Add(character);
            return result;
        }

        public static GowerConfiguration<T>
            WithQualitativeCharacter<T>(this GowerConfiguration<T> conf,
            Func<T, int> character)
        {
            if (character is null)
                throw new ArgumentNullException(nameof(character));
            var result = conf ?? new GowerConfiguration<T>();
            if (result.Qualitative is null)
                result.Qualitative = new List<Func<T, int>> { character };
            else result.Qualitative.Add(character);
            return result;
        }

        public static GowerConfiguration<T>
            WithQuantitativeCharacter<T>(this GowerConfiguration<T> conf,
            Func<T, double> character)
        {
            if (character is null)
                throw new ArgumentNullException(nameof(character));
            var result = conf ?? new GowerConfiguration<T>();
            if (result.Quantitative is null)
                result.Quantitative = new List<Func<T, double>> { character };
            else result.Quantitative.Add(character);
            return result;
        }

        internal static (double, bool) GetSij(IEnumerable<(double Value, bool IsValid)> x)
        {
            static double AddIf(double x, double y, bool flag) => flag ? x + y : x;
            static int IncIf(int x, bool flag) => flag ? x + 1 : x;
            var (a, b) =
                x
                .Where(y => IsNumber(y.Value) && y.IsValid)
                .Aggregate((Result: 0.0, Count: 0), (w, z) => (AddIf(w.Result, z.Value, z.IsValid), IncIf(w.Count, z.IsValid)));
            if (b == 0)
                return (NaN, false);
            else return (a / b, true);
        }

        internal static Func<T, T, (double, bool)> CreateAsymmetricDichotomous<T>(Func<T, bool> c) =>
            (x, y) => DichotomousAsymmetricSimilarity(c(x), c(y));
        internal static Func<T, T, (double, bool)> CreateSymmetricDichotomous<T>(Func<T, bool> c) =>
            (x, y) => DichotomousSymmetricSimilarity(c(x), c(y));
        internal static Func<T, T, (double, bool)> CreateQualitative<T>(Func<T, int> c) =>
            (x, y) => QualitativeSimilarity(c(x), c(y));
        internal static Func<T, T, double, (double, bool)> CreateQuantitative<T>(Func<T, double> c) =>
            (x, y, range) => QuantitativeSimilarity(c(x), c(y), range);

        public static Func<T, T, IEnumerable<double>, (double Result, bool IsValid)> FromConfig<T>(this GowerConfiguration<T> conf)
        {
            var empty = Enumerable.Empty<Func<T, T, (double, bool)>>();
            var asymmetricDich =
                conf.AsymmetricDichotomous is null ? empty :
                conf.AsymmetricDichotomous
                .Select(x => CreateAsymmetricDichotomous(x));
            var symmetricDich =
                conf.SymmetricDichotomous is null ? empty :
                conf.SymmetricDichotomous
                .Select(x => CreateSymmetricDichotomous(x));
            var qual =
                conf.Qualitative is null ? empty :
                conf.Qualitative
                .Select(x => CreateQualitative(x));
            var quan =
                conf.Quantitative is null ? Enumerable.Empty<Func<T, T, double, (double, bool)>>() :
                conf.Quantitative
                .Select(x => CreateQuantitative(x));
            return (x, y, ranges) =>
            {
                var a = asymmetricDich.Select(w => w(x, y));
                var b = symmetricDich.Select(w => w(x, y));
                var c = qual.Select(w => w(x, y));
                var d = quan.Count() > 0 && ranges.Count() == quan.Count() ?
                    Enumerable.Range(0, quan.Count())
                    .Select(n => (q: quan.ElementAt(n), r: ranges.ElementAt(n)))
                    .Select(f => f.q(x, y, f.r)) :
                    Enumerable.Empty<(double, bool)>();
                return GetSij(a.Concat(b).Concat(c).Concat(d));
            };
        }
    }
}
