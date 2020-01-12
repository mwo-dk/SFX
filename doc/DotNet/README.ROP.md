# SFX.ROP

This repository contains a package supporting Railway Oriented Programming:

* [SFX.ROP](https://www.nuget.org/packages/SFX.ROP/) is a copy  of the ROP library from F# for fun and profit.
    - See videos and slides [here](https://fsharpforfunandprofit.com/rop/)
    - See the detailed motivating blog post [here](https://fsharpforfunandprofit.com/posts/recipe-part2/)

The package also has a wanna-be implementation for C#

## SFX.CSROP

Is a simple attempt to facilitate ROP in C#. It is facilitated via:

* The type ```Result<>```, that represents the outcome of an operation.
* The type ```Unit```, that represents ```System.Void``` in order to facilitate Result for operations that do not return anything at all.
* A library of most helper methods from the C# library, but none of the nice operators.

### Result<> and Unit

```Result<>``` has the following implementation:

``` csharp
public struct Result<T>
{
    internal Result(T value, Exception error) =>
        (Value, Error) = (value, error);

    public T Value { get; }
    public Exception Error { get; }

    public void Deconstruct(out bool success, out T value, out Exception error) =>
        (success, value, error) = (Error is null, Value, Error);

    public static implicit operator T(Result<T> x)
    {
        if (!(x.Error is null))
            throw x.Error;
        else return x.Value;
    }

    public static implicit operator ROP.Bridge.Result<T>(Result<T> x) =>
        new ROP.Bridge.Result<T>(x.Value, x.Error);
}
```

Which is not entirely unlike ```System.Nullable<>```. The usage is illustrated in the following:

``` csharp
using static SFX.ROP.CSharp.Library

static Result<decimal> ComputeThatAmount(bool happy, decimal result) =>
    happy ? Succeed(result) : Fail<decimal>(new UnhappyException());

var aResult = ComputeThatAmount(happy); // aResult is Result<decimal>
var (ok, result, error) = ComputeThatAmount(happy); // ok is bool and if true then error is something, if false error is null and result has a value

try {
    decimal result = ComputeThatAmount(happy); // Using the implicit cast operator, that may throw an exception.
}
catch (UnhappyException error) {

}
```

The latter implicit constructor is only to facilitate bridging to the F# sum type.

Mind, that the constructor is hidden. Use the library functions ```Succeed<>``` and ```Fail<>``` instead - see below.

Unit is simply a placeholder for ```System.Void```. Nothing more - you can replace ```decimal``` with ```Unit``` in the example above.

### The library

The library contains something very much like the F# library. A lot of static methods to create ```Result<>```s build op tracks.

## Bridge

In the F# library, there is a brigde function called ```toResult: SFX.CSROP.Result<'a> -> SFX.ROP.Result<'a,exn> ```, that converts the ```Result<>``` to a real F# sum-type. Similarly in the C# library, there is a function called ```ToResult```, that maps the other way.

This makes it easy to utilize SFX.ROP.* in solutions with a variety of projects utilizing F# (ie for business logic) as well as C# (ie. for basic infra-structure, I/O et al).