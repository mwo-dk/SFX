# SFX.BitHack
Basic library for bit array/vector utilization. In short it is similar to the official Microsoft
implementation of [System.Collections.Bittarray](https://referencesource.microsoft.com/#mscorlib/system/collections/bitarray.cs,90295d431f28b046), but
with the following differences:

- Works on 64 bit signed longs.
- Does not throw exceptions (except upon construction).

The repository contains packages for F# as well as C#, and are exposed in the nuget packages [SFX.BitHack](https://www.nuget.org/packages/SFX.BitHack/).

## Usage C#

The C# library ([SFX.BitHack.CSharp](https://www.nuget.org/packages/SFX.BitHack.CSharp/)) essentially implements the ability to work with arrays of bits via 64 bit signed longs.
The core facility is implemented in the static class [BitVectorHelpers](https://github.com/mwo-dk/SFX.BitHack/tree/master/src/BitHack.CSharp/BitVectorHelpers.cs) and is utilized in the class
[BitArray](https://github.com/mwo-dk/SFX.BitHack/tree/master/src/BitHack.CSharp/BitArray.cs).

### BitArray

```BitArray``` is a reference type utilized to work with arrays of bits, and implements ```IEnumerable<bool>```. Mind the object is not immutable, since it supports non-atomic in place modifications of the content. A ```Bitarray``` is initialized via the constructors:

```csharp
BitArray(int length, bool defaultValue) -> BitArray
BitArray(int length) -> BitArray
```

which throw ```ArgumentOutOfRangeException``` in the case of the provided ```lenght``` is negative. The parameter 
```defaultValue``` denotes whether the bit array is initialized with all bits set or not.

#### Accessing individual items

Individual bits are accessed via the accessors

``` csharp
Get(int n) -> Result<bool>
Set(int n, bool value) -> Result<Unit>
```

See the discussion of Result<> below. The type ```Unit``` is a type embracing ```void```, and is shortly described below.

Reading an item in the bit array, can be done in three ways:

``` csharp
var x = new BitArray(size);
...
var result = x.Get(n);
```

Here result is then a ```Result<bool> whose members can be inspected. The method does not throw any exceptions, instead the members ```Error``` and ```Value``` can be inspected. Result<> also supports tuple 
deconstruction, so another way is:

``` csharp
var x = new BitArray(size);
...
var (value, error) = x.Get(n);
```

where the ```error``` then is an ```IndexOutOfRangeException``` and value is a ```bool```. The last way is to utilize the implicitcasting of ```BitArrayAccessResult<bool>``` to ```bool```:

``` csharp
var x = new BitArray(size);
...
try {
    bool result = x.Get(n);
|
catch (IndexOutOfRangeException _) {
    // n is out of bound
}
catch (InvalidCasatException _) {
    // Should actually never happen. Means that Value is null
}
```

Similarly values can be set via the ```Set``` method.

``` csharp
var x = new BitArray(size);
...
var result = x.Set(n, true);
```

Here result is then a ```Result<Unit> whose members can be inspected. The method does not throw any exceptions, instead the members ```Error``` and ```Value``` can be inspected. Again, since ```Result<>``` supports tuple deconstruction, so another way is:

``` csharp
var x = new BitArray(size);
...
var (value, error) = x.Set(n, true);
```

where the ```error``` then is an ```IndexOutOfRangeExcention``` and value is a ```Unit```. The last way is to utilize the implicitcasting of ```Result<Unit>``` to ```Unit```:

``` csharp
var x = new BitArray(size);
...
try {
    Unit result = x.Set(n, true);
|
catch (IndexOutOfRangeException _) {
    // n is out of bound
}
catch (InvalidCasatException _) {
    // Should actually never happen. Means that Value is null
}
```

#### Accessing ranges of bits

Accessing ranges of bits, are done via:

``` csharp
GetRange(int from, int to) -> Result<bool>
SetRange(int from, int to, bool value) -> Result<Unit>
```

That works similarly to the indivual bit item accessors.

#### Setting, clearing and inverting all bits

Is done via

``` csharp
Set() -> void
Clear() -> void
Invert() -> void
```

Mind that this violates immutability. The operations are done on the existing object.

#### Bitwise masking

Lastly the bitwise operators, that are expected for arrays of bits are implemented:

``` csharp
! BitArray -> BitArray
| (BitArray, BitArray) -> BitArray
& (BitArray, BitArray) -> BitArray
^ (BitArray, BitArray) -> BitArray
```

Which implements (immutable, but not atomic) inversion, bitwise or, bitwise and as well as bitwise exclusive or respectively.

### Result<>

As mentioned above, the accessor methods, do not directly return void (setting values) or bool (getting values). Instead these operations return a Result<> - a value type, that contains two get-only properties, who can be inspected. It can be utilized in two (or three) ways:

* Read the value and inspect the properties, that are both nullable.
* Utilize tuple deconstruction - as shown above
* Directly casting the result to the bool or ```Unit``` type and rely on exceptions to be thrown in case of access violation.

This type - like ```Unit``` is defined in [SFX.ROP.CSharp.Types](https://www.nuget.org/packages/SFX.ROP.CSharp.Types/).

### BitVectorHelpers

The core logic of accessing and modifying arrays of bits represented by 64 bit signed integers, is implemented in the static class ```BitVectorHelpers```, whose methods are:

* ``` GetMask(this long vector, long mask) -> bool```, takes an single 64 bit array ```vector``` and validates whether the  bits denoted in the ```mask``` bit array. The result of the operation is ```true``` in case all bits in ```vector``` according to ```mask``` is set, and ```false``` otherwise.
* ```SetMask(this long vector, long mask, bool value) -> long```, takes a single 64 bit array ```vector```, and either sets or clears the bits denoted by the ```mask``` bit array. The parameter ```value``` denotes whether the bits should be set or cleared.
* ```Get(this long vector, int n) -> bool```, checks a single 64 bit array ```vector``` to check whether the bit at place ```n``` is set or not. It is not validated whether ```n``` is a valid index.
* ```GetSafe(this long vector, int n) -> (bool, bool)```, does a safe checking of whether the bit ```n``` is set or not. It returns a value tuple, where the first item tells whether ```n``` is valid, and it is, the second item is the result of the operation.
* ```Set(this long vector, int n, bool value) -> long```, sets the ```n```th bit in ```vector``` to either 0 or 1 depending on ```value```. It is not validated whether ```n``` is a valid index.
* ```SetSafe(this long vector, int n, bool value) -> (bool, long)```, sets the ```n```th bit in ```vector``` to either 0 or 1 depending on ```value```. It returns a value tuple, where the first item tells whether ```n``` is valid, and it is, the second item is the result of the operation.
* ```GetRange(this long vector, int i, int j) -> bool```, checks a single 64 bit array ```vector``` for whether all bits in the index range ```[i..j]``` are set. No validation of the indices is performed.
* ```GetRangeSafe(this long vector, int i, int j) -> (bool, bool)```, does a safe checking of whether all bits in ```vector``` in the index range ```[i..j]``` are set. The first item in the result denotes validity of the result.
* ```SetRange(this long vector, int i, int j, bool value) -> long```, sets or clears all bits in ```vector``` in the index range ```[i..j]```. Not index validation of the indices are performed.
* ```SetRangeSafe(this long vector, int i, int j, bool value) -> (bool, long)```, sets or clears all bits in ```vector``` in the index range ```[i..j]```. The first item in the result denotes validity of the result.
* ```Format(this long vector, int size = 64) -> string``` formats ```vector``` to a readable format.
* ```GetMask(this long[] vectors, long[] masks) -> bool```, validats whether all bits in ```vectors``` according to the masks in ```masks``` are set or not. 
* ```GetMaskSafe(this long[] vectors, long[] masks) -> (bool, string, bool)```, validats whether all bits in ```vectors``` according to the masks in ```masks``` are set or not. The first item in the tuple denotes success, and in the case of error, the second is an error string. The last item is the result - in case the first item is ```true```.
* ```SetMask(this long[] vectors, long[] masks, bool value) -> void```, sets all bits in ```vectors``` (in place) according to the provided ```masks``` and ```value```. No validation occurs.
* ```SetMaskSafe(this long[] vectors, long[] masks, bool value) -> (bool, string)```, , sets all bits in ```vectors``` (in place) according to the provided ```masks``` and ```value```. The first item in the tuple denotes success, and in the case of error, the second is an error string.
* ```Get(this long[] vectors, int n) -> bool```, checks whether the ```n```th bit in ```vectors``` is set. No parameter validation occurs.
* ```GetSafe(this long[] vectors, int size, int n) -> bool```, checks whether the ```n```th bit in ```vectors``` is set. No parameter validation occurs. The first item in the tuple denotes success. The second item is the result - in case the first item is ```true```.
* ```Set(this long[] vectors, int n, bool value) -> void```, sets the ```n```th bit in ```vectors``` to the value denoted by ```value```. No validation occurs.
* ```SetSafe(this long[] vectors, int size, int n, bool value) -> bool```, sets the ```n```th bit in ```vectors``` to the value denoted by ```value```. The result of the operation denotes success.
* ```GetRange(this long[] vectors, int i, int j) -> bool```, checks whether all bits in ```vectors``` in the index range ```[i..j]``` are set. No validation occurs.
* ```GetRangeSafe(this long[] vectors, int size, int i, int j) -> (bool, bool)```, checks whether all bits in ```vectors``` in the index range ```[i..j]``` are set. The first item denotes success of the operation and in case of success, the second item is the result.
* ```SetRange(this long[] vectors, int i, int j, bool value) -> void```, sets all bits in ```vectors``` in the index range ```[i..j]``` to either 0 or 1 depending on ```value```. No validation occcurs.
* ```SetRangeSafe(this long[] vectors, int size, int i, int j, bool value) -> bool```, sets all bits in ```vectors``` in the index range ```[i..j]``` to either 0 or 1 depending on ```value```. The method returns the success of the operation.
* ```ClearAll(this long[] vectors) -> void``` clears all bits in ```vectors```. No validation occurs.
* ```SetAll(this long[] vectors) -> void```, sets all bits in ```vectors```. No validation occurs.
* ```Not(this long[] x, long[] result) -> vpid```, writes the inversion of ```x``` and writes it to ```result```. No validation occurs.
* ```NotSafe(this long[] x, long[] result) -> bool```, similar to ```Not```, but does validation of parameters but validates parameters and returns the success of the operation.
* ```And(this long[] x, long[] y, long[] result) -> void``` performs a bitwise and of ```x``` and ```y``` and writes the result in ```result```. No validation occurs.
* ```AndSafe(this long[] x, long[] y, long[] result) -> bool```, similar to ```And```, but does validation of parameters but validates parameters and returns the success of the operation.
* ```Or(this long[] x, long[] y, long[] result) -> void``` performs a bitwise or of ```x``` and ```y``` and writes the result in ```result```. No validation occurs.
* ```OrSafe(this long[] x, long[] y, long[] result) -> bool```, similar to ```Or```, but does validation of parameters but validates parameters and returns the success of the operation.
* ```Xor(this long[] x, long[] y, long[] result) -> void``` performs a bitwise exclusive or of ```x``` and ```y``` and writes the result in ```result```. No validation occurs.
* ```XorSafe(this long[] x, long[] y, long[] result) -> bool```, similar to ```Xor```, but does validation of parameters but validates parameters and returns the success of the operation.
* ```Format(this long[] vectors, int size) -> string``` formats ```vectors``` to a readable format.

## Usage F#

The F# library ([SFX.BitHack](https://www.nuget.org/packages/SFX.BitHack/), depends on [SFX.BitHack.CSharp](https://www.nuget.org/packages/SFX.BitHack.CSharp/)) is a package that does a similar thing as the C# library, but in a simpler manner. The library also depends on [SFX.ROP](https://www.nuget.org/packages/SFX.ROP/) in order to facilitate railway oriented programming and thus pattern matching instead of throwing exceptions or return tuple results.

The library contains the record type:

``` fsharp
type BitVector = int64 array
type BitArray = {
    Length: int
    Data: BitVector
}
```

The library implements the following functions:

* ```create: int -> bool -> Result<BitArray, BitAccessError>```, creates a ```Result<BitArray, BitAccessError>``` in case that the lenght is valid (>= 0) and initializes the data array according to the second parameter. 
* ```zeroCreate: int -> Result<BitArray, BitAccessError>``` creates a ```Result<BitArray, BitAccessError>``` with all bits cleared.
* ```get: int -> BitArray -> Result<bool, BitAccessError>```, checks whether a given bit in ```BitArray``` is set.
* ```set: int -> bool -> Result<unit, BitAccessError>```, sets a bit in the bit array.
* ```getRange: int -> int -> BitArray -> Result<bool, BitAccessError>```, checks whether all bits in a range are set.
* ```setRange: int -> int -> bool -> BitArray -> Result<unit, BitAccessError>```, sets all bits in the bit array.
* ```invert: BitArray -> BitArray```, creates an inverted bit array.
* ```bitwiseAndOp: BitArray -> BitArray -> Result<BitArray, BitAccessError>``` performs a bitwise and operation.
* ```bitwiseOrOp: BitArray -> BitArray -> Result<BitArray, BitAccessError>``` performs a bitwise or operation.
* ```bitwiseXorOp: BitArray -> BitArray -> Result<BitArray, BitAccessError>``` performs a bitwise exclusive operation.

Besides this, the for operators ```~~~```, ```&&&```, ```|||``` and ```^^^``` are implemented via ```invert```, ```bitwiseAndOp```, ```bitwiseOrOp``` and ```bitwiseXorOp``` respectively.

Sample utilization is:

``` fsharp
// Create an array of lenght 5 with all bits set
match true |> create 5 with
| Success x -> // Work with x
| _ -> // Happens never

// In case 'n' can be negative, ie parameter
match true |> create n with
| Success x -> // Work with x
| Failure err -> // Handle error case

// Create an array of length 42 with all bits cleared
match 42 |> zeroCreate with
| Success x -> // Work with x
| _ -> // Happens never

// In case 'n' can be negative, ie parameter
match n |> zeroCreate with
| Success x -> // Work with x
| Failure err -> // Handle error case

// get an element
match x |> get n with
| Success isSet -> // Work with result
| Failure err -> // Handle error case

// set an element
match x |> set n value with
| Success x -> // Work with result, x is shadowed
| Failure err -> // Handle error case

// check a range
match x |> getRange i j with
| Success isSet -> // Work with result
| Failure err -> // Handle error case

// set a range
match x |> setRange i j value with
| Success x -> // Work with result, x is shadowed
| Failure err -> // Handle error case

// invert
match x |> invert with
| Success x -> // Work with result, x is shadowed
| Failure err -> // Handle error case

// Utilizing operator
match ~~~x with
| Success x -> // Work with result, x is shadowed
| Failure err -> // Handle error case

// bitwise and
match bitwiseAndOp x y with
| Success result -> // Work with result
| Failure err -> // Handle error case

// Utilizing operator
match x &&& y with
| Success result -> // Work with result
| Failure err -> // Handle error case

// bitwise or
match bitwiseOrOp x y with
| Success result -> // Work with result
| Failure err -> // Handle error case

// Utilizing operator
match x ||| y with
| Success result -> // Work with result
| Failure err -> // Handle error case

// bitwise exclusive or
match bitwiseXorOp x y with
| Success result -> // Work with result
| Failure err -> // Handle error case

// Utilizing operator
match x ^^^ y with
| Success result -> // Work with result
| Failure err -> // Handle error case
```

### BitAccessError

The type ```BitAccessError```

``` fsharp
type BitAccessError =
| InvalidArrayLength of int
| IndexOutOfRange of int
| IndicesOutOfRange of int*int
| IncompatipleLengths of int*int
```
Is utilized to report error and can be utilized for pattern matching.