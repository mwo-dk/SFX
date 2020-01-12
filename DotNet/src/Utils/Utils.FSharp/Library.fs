namespace SFX.Utils

open SFX.ROP
open SFX.ROP.Bridge
open SFX.Utils.Infrastructure

module Time =

    let private dateTimeProvider = DateTimeProvider()
    /// Creates a DateTimeProvider
    let createDateTimeProvider = DateTimeProvider
    let private timeZoneProvider = TimeZoneProvider()
    /// Creates a TimeZoneProvider
    let createTimeZoneProvider = TimeZoneProvider
    let private dateTimeConverter = DateTimeConverter(timeZoneProvider)
    
    /// Gets the local "now" (DateTimeOffset)
    let getNow = dateTimeProvider.GetNow
    /// Gets "now" in Utc (DateTimeOffset)
    let getUtcNow = dateTimeProvider.GetUtcNow

    /// Gets the local TimeZoneInfo
    let getLocalTimeZone = timeZoneProvider.GetLocal
    /// Gets the Utc TimeZomeInfo
    let getUtcTimeZone = timeZoneProvider.GetUtc
    /// Attempts to find the TimeZoneInfo denoted by the argument
    let findSystemTimeZoneById = 
        (fun x -> timeZoneProvider.FindSystemTimeZoneById(x).ToFSharpResult()) >> toResult

    /// Converts dt to a DateTimeOffset by tz
    let convert dt tz = 
        dateTimeConverter.Convert(dt, tz).ToFSharpResult() |> toResult
    /// Converts the argument to Utc
    let toUtc = 
        (fun x -> dateTimeConverter.ToUtc(x).ToFSharpResult()) >> toResult

module Timer =
    open Microsoft.FSharp.Core
    open System

    let private timerProvider = TimerProvider()
    /// Creates a TimerProvider
    let createTimerProvider = TimerProvider

    /// Creates a Timer running with the interval denoted invoking the handler
    /// provided upon each firing of the timer. The parameter autoStart denotes
    /// whether to automatically start the timer
    let createTimer interval (handler: unit -> unit) autoStart =
        timerProvider.Create(interval, System.Action(handler), autoStart).ToFSharpResult() |> toResult

    /// Rperesents the errors, that can occur when working with timers
    type TimerError =
    /// The timer provided is null
    | Null
    /// The timer provided is already disposed
    | TimerDisposed
    /// An exception has occurred
    | Other of exn

    let private toResult (x: SFX.CSROP.Result<'a>) =
        if x.Error |> isNull then () |> succeed
        else 
            if (x.Error :?> ObjectDisposedException) <> null then TimerDisposed |> fail
            else x.Error |> Other |> fail
    /// Starts the timer
    let startTimer (timer: ITimer) =
        if timer |> isNull then Null |> fail
        else timer.Start() |> toResult
    /// Stops the timer
    let stopTimer (timer: ITimer) =
        if timer |> isNull then Null |> fail
        else timer.Stop() |> toResult
    // Closes (disposes) the timer
    let closeTimer (timer: ITimer) = 
        if timer |> isNull then Null |> fail
        else timer.Dispose() |> succeed

module Initializable =
    /// Represents an initializable object
    type InitializableObject =
    /// The initializable object is IInitializable
    | Sync of IInitializable
    /// The initializable object is IAsyncInitializable
    | Async of IAsyncInitializable
    /// Converts an IInitializable to an InitializableObject (Sync)
    let sync = Sync
    /// Converts an IAsyncInitializable to an InitializableObject (Async)
    let async = Async

    /// Represents the errors that can occur when working with InitializableObjects
    type InitializationError =
    /// An error occurred while initializing an object
    | InitializationFailed of exn
    /// An error occurred while checking an object's initialization state
    | InitializationCheckFailed of exn

    /// Initializes an InitializableObject
    let initialize =
        let doInitialize x =
            match x with
            | Sync x -> x.Initialize()
            | Async x -> x.InitializeAsync() |> Async.AwaitTask |> Async.RunSynchronously
        let handleError error = error |> InitializationFailed
        tryCatch doInitialize handleError

    /// Checks whether an InitializableObject is initialized
    let isInitialized =
        let doIsInitilized x =
            match x with
            | Sync x -> x.IsInitialized()
            | Async x -> x.IsInitialized()
        let handleError error = error |> InitializationCheckFailed
        tryCatch doIsInitilized handleError

module Initializer =
    /// Represents an object that can perform initialization (of something other than itself)
    type InitializerObject =
    /// The initializable object is IInitializer
    | Sync of IInitializer
    /// The initializable object is IAsyncInitializer
    | Async of IAsyncInitializer
    /// Converts an IInitializer to an InitializerObject (Sync)
    let sync = Sync
    /// Converts an IAsyncInitializer to an InitializerObject (Async)
    let async = Async
    
    /// Performs initialization
    let initialize =
        let doInitialize x =
            match x with
            | Sync x -> x.Initialize()
            | Async x -> x.InitializeAsync() |> Async.AwaitTask |> Async.RunSynchronously
        tryCatch doInitialize id        

module HashHelpers =
    /// Represents the errors that can occur when computing hashes
    type HashError = 
    /// An argument is null
    | Null
    /// A sequence is empty
    | Empty

    let inline private hash' prime1 prime2 x = 
        x |> Array.fold (fun acc n -> acc*prime2 + n) prime1
    let inline private hash'' prime1 prime2 x =
        match x |> isNull with
        | false ->
            match x |> Array.isEmpty with
            | false -> x |> hash' prime1 prime2 |> succeed
            | true -> Empty |> fail
        | true -> Null |> fail
    let inline private hash''' x = x |> hash'' 19 31
    let inline private hash'''' (convert: 'a -> int) x =
        x |> Array.map convert |> hash'''

    /// Represents arrays to the hashing functions
    type HashArgs =
    /// An array of bytes
    | B of byte array
    /// An array of integers
    | I of int array
    /// An array of objects
    | O of obj array
    let byteArray = B
    let intArray = I
    let objArray = O

    /// Computes the hash of the provided argument (an array)
    let hash x =
        match x with
        | B x -> x |> hash'''' (fun b -> int(b))
        | I x -> x |> hash'''
        | O x -> 
            match x |> isNull with
            | false -> x |> Array.map (fun x -> x.GetHashCode()) |> hash'''
            | true -> Null |> fail

module StringHelpers =
    open System

    /// Shortcut for String.IsNullOrEmpty
    let isNullOrEmpty = String.IsNullOrEmpty
    /// Shortcut for String.IsNullOrWhiteSpace
    let isNullOrWhiteSpace = String.IsNullOrWhiteSpace