module setTests

open FsCheck
open FsCheck.Xunit
open SFX.FSharp.ROP
open SFX.FSharp.BitHack

[<Property>]
let ```set with negative index works``(length: NonNegativeInt, index: NegativeInt) =
    let n = length.Get

    match true |> create n with
    | Success result ->
        match result |> set index.Get true with
        | Failure error ->
            match error with
            | IndexOutOfRange n' -> n' = index.Get
            | _ -> false
        | Success _ -> false
    | Failure _ -> 
        false

[<Property>]
let ``set works``(length: NonNegativeInt, index: NonNegativeInt, value: bool) =
    let n = length.Get
    if n = 0 then true
    else 
        match value |> create n with
        | Success result ->
            let index' = (index.Get)%n
            match result |> set index' (value |> not) with
            | Success _ -> 
                let checkBit i =
                    let vector = result.Data.[i/64]
                    let offset = i%64
                    let value' = if index' = i then (value |> not) else value
                    if value' then 0L <> (vector &&& (1L <<< offset))
                    else 0L = (vector &&& (1L <<< offset))
                let allBitsOk = 
                    [|0..n-1|] |> Array.fold (fun acc i -> acc && (checkBit i)) true
                allBitsOk
            | _ -> false
        
        | _ -> false