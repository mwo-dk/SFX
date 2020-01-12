# SFX.Random
Helper methods for utilizing [RNGCryptoServiceProvider](https://referencesource.microsoft.com/#mscorlib/system/security/cryptography/rngcryptoserviceprovider.cs,d525bf7d9ca1d38a) as a randomizer. Two libraries are supported:

* [SFX.Random](https://www.nuget.org/packages/SFX.Random/)

Basically the libraries just support functions for generating primitive types: bool, sbyte, char, short, int,
long, byte, ushort, uint, ulong, float, double, decimal, TimeSpan and DateTimeOffset.

Also functions for generating them in ranges are supported.

## Usage C#

The utility functions are basically all methods in the static class ```SFX.Random.CSharp.RandomHelpers```, which exposes the methods:

* ```GetRandomBool() -> bool```.
* ```GetRandomSignedByte() -> sbyte```.
* ```GetRandomShort() -> short```.
* ```GetRandomInt() -> int```.
* ```GetRandomLong() -> long```.
* ```GetRandomByte() -> byte```.
* ```GetRandomUnsignedShort() -> ushort```.
* ```GetRandomUnsignedInt() -> uint```.
* ```GetRandomUnsignedLong() -> ulong```.
* ```GetRandomFloat() -> float```.
* ```GetRandomDouble() -> double```.
* ```GetRandomDecimal() -> decimal```.
* ```GetRandomTimeSpan() -> TimeSpan```.
* ```GetRandomDateTimeOffset() -> DateTimeOffset```.

And they do what you expect:

``` csharp
using static SFX.Random.CSharp.RandomHelpers;

...

var someRandomDate = GetRandomDateTimeOffset(); // Should be "very random"
```

With regards to ```GetRandomDateTimeOffset```, you'll get a valid random ```DateTimeOffset``` in a valid time of day in a valid time-zone. 

In principle, running the methods in tight loops in multiple threads could incur congestion, since the [RNGCryptoServiceProvider](https://referencesource.microsoft.com/#mscorlib/system/security/cryptography/rngcryptoserviceprovider.cs,d525bf7d9ca1d38a) works on arrays of ```bytes```, and a static instance of the provider is utilized together with a shared 
array of 8 bytes protected by a [System.Threading.SpinLock](https://docs.microsoft.com/en-us/dotnet/api/system.threading.spinlock?view=netframework-4.8). This is chosen instead of continously allocating data - ```stackalloc``` has also been avoided, since it either required unsafe code or code-duplication, since ```Func<>```'s of ```Span<>```'s do not lend them selves well to generic methods.

```SFX.Random.CSharp.RandomHelpers``` also exposes methods to get random values in a range:

* ```GetRandomSignedByteInRange(sbyte a, sbyte b) -> sbyte```.
* ```GetRandomShortInRange(short a, short b) -> short```.
* ```GetRandomIntInRange(int a, int b) -> int```.
* ```GetRandomLongInRange(long a, long b) -> long```.
* ```GetRandomByteInRange(byte a, byte b) -> byte```.
* ```GetRandomUnsignedShortInRange(ushort a, ushort b) -> ushort```.
* ```GetRandomUnsignedIntInRange(uint a, uint b) -> uint```.
* ```GetRandomUnsignedLongInRange(ulong a, ulong b) -> ulong```.
* ```GetRandomFloatInRange(float a, float b) -> float```.
* ```GetRandomDoubleInRange(double a, double b) -> double```.
* ```GetRandomDecimalInRange(decimal a, decimal b) -> decimal```.
* ```GetRandomTimeSpanInRange(TimeSpan a, TimeSpan b) -> TimeSpan```.
* ```GetRandomDateTimeOffsetInRange(DateTimeOffset a, DateTimeOffset b) -> DateTimeOffset```.

That do what you'd expect. It has been chosen to simply not validate arguments **and** loop till the random generated value is *in range* => you can end in an infinite loop.

## Usage F#

As mentioned above, the F# library sits on top of the C# library, and thus exposes functions with names identical to the ones methoned above, with the difference of sticking to the convension of camel-casing, in the module ```SFX.Random```:

``` fsharp
open SFX.Random

...

let someRandomDate = getRandomDateTimeOffset()
```