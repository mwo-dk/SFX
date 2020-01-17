# SFX.Utils

Basic helper routines. The repository contains packages for F# as well as C#, and are exposed in the nuget packages [SFX.Utils](https://www.nuget.org/packages/SFX.Utils/) and [SFX.Utils.CSharp](https://www.nuget.org/packages/SFX.Utils.CSharp/).

## Usage C#

The C# library is basically a small collection of helper classes and interfaces (neat when using then in a DI context and faking etc), and deal can be grouped into three scopes:

* Time. Helpers to work with actual time and timers.
* Initialization. Helper interfaces and a single class to let instances initialize themselves or others.
* Hash code

### Time

This small collection, deals with date time and timers.

#### (I)DateTimeProvider

The ```IDateTimeProvider``` is a simple interface, that can deliver the current ```DateTimeOffset``` either locally (on the running machine) or in UTC:

``` csharp
public interface IDateTimeProvider
{
    DateTimeOffset GetNow();
    DateTimeOffset GetUtcNow();
}
```

Not the hardest interface, and even the youngest develper can figure out it's implementation. It's done many other places and in other ways, searching faking time or asking Rx how to deal with it, gives a lot of good and more advanced options. I like it, because it's simple to fake it in tests.

#### (I)TimeZoneProvider

The ```ITimeZoneProvider``` is another no-brainer, that delivers ```TimeZoneInfo```:

``` csharp
public interface ITimeZoneProvider 
{
    TimeZoneInfo GetLocal();
    TimeZoneInfo GetUtc();
    Result<TimeZoneInfo> FindSystemTimeZoneById(string id);
}
``` 

Again, no explanation should be required here. ```FindSystemTimeZoneById``` returns an ```Result<TimeZoneInfo>```, which is due to the fact, that the underlying call to the static method by the same name in ```TimeZoneInfo``` can throw exceptions. ```Result<>``` gives the caller the option of what kind of usage pattern to use. ```Result<>``` is a loan from [SFX.ROP.CSharp](https://www.nuget.org/packages/SFX.ROP.CSharp/) and introduced below.

#### (I)DateTimeConverter

The ```IDateTimeConverter``` interface also should be managable to grasp for people without degrees in telepathy, universal healing and lots of thesises written about Area 51 and 9/11:

``` csharp
public interface IDateTimeConverter
{
    Result<DateTimeOffset> Convert(DateTimeOffset dateTimeOffset, TimeZoneInfo timeZoneInfo);
    Result<DateTimeOffset> ToUtc(DateTimeOffset dateTimeOffset);
}
``` 

#### (I)Timer and (I)TimerProvider

Similar to ```IDateTimeProvider``` an abstraction over timers and providers thereof have been provided in:

``` csharp
public interface ITimer : IDisposable
{
    Result<Unit> Start();
    Result<Unit> Stop();
}

public interface ITimerProvider
{
    Result<ITimer> Create(TimeSpan interval, Action handler, bool autoStart);
}
```

Again very simple. Testable, and the implementation provided in ```Timer``` and ```TimerProvider``` rely on ```System.Threading.Timer```, which is good in servers, but the other timers available can be implemented for as well.

### Initialization

The notion of post 'construction' initialization is convenient, ie in scenarios where you hook up a large application and then want to start things - throwing exceptions upon object initialization (object construction) or even to heavy processing is a mess there. Then it is simpler to hook up things in a manner:

``` csharp
using static System.Threading.Interlocked;

public sealed class SomeAggregatedService : IInitializable 
{
    private static readonly ObjectName = typeof(SomeAggregatedService).FullName;

    public SomeAggregatedService(ISomeDependency someDependency, IAnotherDependency anotherDependency) =>
        (SomeDependency, AnotherDependency) = 
            (someDependency ?? throw new ArgumentNullException(nameof(someDependency), anotherdependency ?? throw new ArgumentNullException(nameof(anotherDependency))));
    
    internal ISomeDependency SomeDependency {get;} // Internal - easier to test
    internal IAnotherDependency AnotherDependency {get;}

    internal long IsInitializationRunningCount = 0L;
    internal long IsInitializedCount = 0L;
    public void Initialize() 
    {
        try 
        {
            if (1L < Increment(ref IsInitializationRunningCount) || IsInitialized())
                return;

            // Do the heavy lifting...

            Increment(ref IsInitializedCount);
        }
        finally 
        {
            Decrement(ref IsInitializationRunningCount);
        }
    }
    public bool IsInitialized() => 0L < Read(ref IsInitializedCount);

    public void ServiceSomeOne() 
    {
        if (!IsInitialized())
            throw new InvalidOperationException($"{ObjectName} is not initialized");

        // Do the servicing
    }
}
``` 

This is where the interfaces:

``` csharp
public interface IInitializable
{
    void Initialize();
    bool IsInitialized();
}
public interface IAsyncInitializable
{
    Task InitializeAsync();
    bool IsInitialized();
}
``` 

come in handy, wrt wiring up the monolith. Similarly interfaces for wiring up others have been provided:

``` csharp
public interface IInitializer
{
    void Initialize();
}
public interface IAsyncInitializer
{
    Task InitializeAsync();
}
```
### Hash codes

The static class ```SFX.Utils.Infrastructure.HashCodeHelpers``` is just a simple wrapper for doing the standard way of computing the hashcode of complex objects utilizing the primes 19 and 31. The methods are:

* ```ComputeHashCode(this int[] items) -> Result<int>```
* ```ComputeHashCode<T>(this T[] items, Func<T, int> getHash) -> Result<int>```
* ```ComputeHashCode<T>(this IEnumerable<T> items, Func<T, int> getHash) -> Result<int>```
* ```ComputeHashCodeForObjectArray(params object[] items) -> Result<int>```

It should be fairly self-explanatory. Why provide a library for this? Reason is, that I myself often have been working with large in memory caches where ie. the key's have been fairly complex, so:

* Pre-calculating the hash of these (immutable) objects upon (maybe) construction, returning that value in ```GetHashCode()``` and then 
* Override ```IEquatable<>``` makes doing this convenient.

### Result<>

```Result<>``` is a simple structure, that enables various invocation patterns, not entirely unlike ```Nullable<>```. Its total implementation is:

``` csharp
public struct Result<T>
{
    internal Result(Exception error, T value) =>
        (Error, Value) = (error, value);
    
    public T Value { get; }
    public Exception Error { get; }

    public void Deconstruct(out bool success, out Exception error, out T value) =>
        (success, error, value) = (Error is null, Error, Value);

    public static implicit operator T(Result<T> x)
    {
        if (!(x.Error is null))
            throw x.Error;
        else return x.Value;
    }
}
```

That is: it can either represent a successfull or a failing result. As can be seen, it supports tuple deconstruction, meaning that:

``` csharp
static Result<double> f() {...} // Does not throw any exception

var opResult = f(); // opResult is an Result<double>. No exceptions thrown.
var (exn, result) = f(); // exn is of type Exception, non-null in case of error. result is a double. No exceptions thrown. 
double result_ = f(); // result_ is a double. The implicit cast to double throws exception if there is (was) one.
```

For methods that otherwise would return nothing (```System.Void```), the type ```Unit``` has been introduced in the same project as ```Result<>```.

Utilizing ```Result<>``` has to be done via the static class ```SFX.ROP.CSharp.Library```, and can be utilized in the following manner:

``` csharp
using static SFX.ROP.CSharp.Library;

...

Result<T> f() {
    ...
    if (ok) return Succeed(result); // result is T
    else return Fail<T>(theErrorThatHappened); // theErrorThatHappened is some exception
}
```

## Usage F#

The F# library delivers a few modules that essentially mostly wrap the C# library.

### SFX.FSharp.Utils.Time
The ```Time``` module is simply a wrapper around the similar methods in the corresponding C# library:

* ```createDateTimeProvider: unit -> DateTimeProvider```
* ```createTimeZoneProvider: unit -> TimeZoneProvider```
* ```createDateTimeConverter: unit -> DateTimeConverter```
* ```getNow: unit -> DateTimeOffset```
* ```getUtcNow: unit -> DateTimeOffset```
* ```getLocalTimeZone: unit -> TimeZoneInfo```
* ```getUtcTimeZone: unit -> TimeZoneInfo```
* ```findSystemTimeZoneById: string -> Result<TimeZoneInfo, exn>```

which do what you would expect.

### SFX.FSharp.Utils.Timer
The ```Timer``` module is also a more functional wrapper around the timer library:

* ```createTimerProvider: unit -> TimerProvider```, creates a new ```TimerProvider```.
* ```createTimer: TimeSpan -> (unit -> unit) -> bool -> Result<ITimer,exn>```, creates a new ```ITimer```, where the first argument is the period, the second the handler and the last argument is a flag telling whether to automatically start the timer.
* ```startTimer: ITimer -> Result<unit, TimerError>```, starts a timer.
* ```stopTimer: ITimer -> Result<unit, TimerError>```, stops a timer.
* ```closeTimer: ITimer -> Result<unit, TimerError>```, disposes off a timer.

where ```TimerError``` is:

``` fsharp
type TimerError =
| Null
| TimerDisposed
| Other of exn
```

Mind that ```ITimer``` is ```IDisposable```. 

### SFX.FSharp.Utils.Initializable
The ```Initializable``` module contains two functions:

* ```initialize: InitializableObject -> Result<unit, InitializationError>```
* ```isInitialized: InitializableObject -> Result<bool, InitializationError>```

where ```InitializableObject``` is the sum over ```IInitializable``` and ```IAsyncInitializable```:

``` fsharp
type InitializableObject =
| Sync of IInitializable
| Async of IAsyncInitializable
let sync = Sync
let async = Async
```

and ```InitializationError``` is:

``` fsharp
type InitializationError =
| InitializationFailed of exn
| InitializationCheckFailed of exn
```

### SFX.FSharp.Utils.Initializer
The ```Initializer``` module is similar to ```Initializable```:

* ```initialize: InitializerObject -> Result<unit, exn>```

where ```InitializerObject``` is the sum over ```IInitializer``` and ```IAsyncInitializer```:

``` fsharp
type InitializerObject =
| Sync of IInitializer
| Async of IAsyncInitializer
```
### SFX.FSharp.Utils.HashHelpers
The ```HashHelpers``` module simply provides a hashing function with the primes 19 and 31:

* ```hash: HashArgs -> Result<int, HashError>```

where

```HashArgs``` is:

``` fsharp
type HashArgs =
| B of byte array
| I of int array
| O of obj array
let byteArray = B
let intArray = I
let objArray = O
```

and ```HashError``` is:

``` fsharp
type HashError = 
| IsNull
| IsEmpty
```

### SFX.FSharp.Utils.StringHelpers
The ```StringHelpers``` simply wraps two commonly used members of ```System.String```:

* ```isNullOrEmpty: string -> bool``` and
* ```isNullOrWhiteSpace: string -> bool```