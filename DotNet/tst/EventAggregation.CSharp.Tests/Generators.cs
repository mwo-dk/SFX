using FsCheck;
using SFX.EventAggregation.Model;
using System;
using System.Linq;

namespace SFX.EventAggregation.Test
{
    public class Generators
    {
        public static Arbitrary<Name> Name() =>
            Gen.OneOf(Arb.Generate<string>().Select(x => new Name(x)))
            .ToArbitrary();
        private static readonly Type[] Types = new[]
        {
            typeof(bool), typeof(int), typeof(double),
            typeof(decimal), typeof(string),
            typeof(TimeSpan), typeof(DateTimeOffset)
        };
        public static Arbitrary<TypeAndName> TypeAndName() =>
            Gen.OneOf(Arb.Generate<Name>()
                .Select(x => new TypeAndName(typeof(string), x)))
            .ToArbitrary();

        internal static void RegisterAll()
        {
            Arb.Register<Generators>();
        }
    }
}
