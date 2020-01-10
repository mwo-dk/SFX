module SFX.ROP

/// The two-track type
type Result<'TSuccess,'TFailure> = 
| Success of 'TSuccess
| Failure of 'TFailure

/// Convert a single value into a two-track result
let succeed = Success

/// Convert a single value into a two-track result
let fail = Failure

/// Apply either a success function or failure function
let either successFunc failureFunc twoTrackInput =
    match twoTrackInput with
    | Success s -> successFunc s
    | Failure f -> failureFunc f

/// Convert a switch function into a two-track function
let bind f = 
    fail |> either f

/// Pipe a two-track value into a switch function 
let (>>=) x f = 
    bind f x

/// Compose two switches into another switch
let (>=>) s1 s2 = 
    s1 >> bind s2

/// Convert a one-track function into a switch
let switch f = 
    f >> succeed

/// Convert a one-track function into a two-track function
let map f = 
    either (f >> succeed) fail

/// Convert a dead-end function into a one-track function
let tee f x = 
    f x; x 

/// Convert a one-track function into a switch with exception handling
let tryCatch f exnHandler x =
    try
        f x |> succeed
    with
    | ex -> exnHandler ex |> fail

/// Convert two one-track functions into a two-track function
let doubleMap successFunc failureFunc =
    either (successFunc >> succeed) (failureFunc >> fail)

/// Add two switches in parallel
let plus addSuccess addFailure switch1 switch2 x = 
    match (switch1 x),(switch2 x) with
    | Success s1,Success s2 -> Success (addSuccess s1 s2)
    | Failure f1,Success _  -> Failure f1
    | Success _ ,Failure f2 -> Failure f2
    | Failure f1,Failure f2 -> Failure (addFailure f1 f2)

/// Partitions a sequence of Result<,>s into successes and failures
let partitionResults results =
    let successes =
        results |>
        Seq.map (fun x ->
            match x with
            | Success value -> value |> Some
            | _ -> None) |>
        Seq.choose id |>
        Seq.toArray
    let failures = 
        results |>
        Seq.map (fun x ->
            match x with
            | Failure value -> value |> Some
            | _ -> None) |>
        Seq.choose id |>
        Seq.toArray
    (successes, failures)

/// Represents a combination of successes and failures. Used when partitioning
/// multiple Result<,>s
type PartialFailure<'TSuccess, 'TFailure> = 
    {
        Successes: 'TSuccess array
        Failures: 'TFailure array
    }

/// Simple constructor for PartialFailure<,>
let private toPartialFailure successes failures =
    {
        Successes = successes
        Failures = failures
    }

    
/// Represents the summary of multiple Result<,>s. 
/// Empty means that no results were present
/// Partial means, that there is a combination of successes and failures
/// Full means that everything failed
type ResultSummary<'TSuccess, 'TFailure> =
| Empty
| Partial of PartialFailure<'TSuccess, 'TFailure>
| Full  of 'TFailure array

/// Aggregates a Result<,> seq into a combined
/// Result<'a array, ResultSummary<'a,'b>>
let aggregateResults results =
    let successes, failures = results |> partitionResults
    match successes |> Array.isEmpty, failures |> Array.isEmpty with
    | false, false ->
        toPartialFailure successes failures |> Partial |> fail
    | false, true ->
        successes |> Seq.toArray |> succeed
    | true, false ->
        failures |> Full |> fail
    | true, true ->
        Empty |> fail

module Bridge =
    [<Struct>]
    type Result<'a> = {
        Value: 'a
        Error: exn
    } 

    /// Convert a C# Result<'a> to the local sum type
    let toResult (x: Result<'a>) =
        if x.Error |> isNull then x.Error |> fail
        else x.Value |> succeed