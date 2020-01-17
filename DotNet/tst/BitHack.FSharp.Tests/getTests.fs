module getTests

open FsCheck
open FsCheck.Xunit
open SFX.FSharp.ROP
open SFX.FSharp.BitHack

[<Property>]
let ```get with negative index works``(length: NonNegativeInt, index: NegativeInt) =
    let n = length.Get

    match true |> create n with
    | Success result ->
        match result |> get index.Get with
        | Failure error ->
            match error with
            | IndexOutOfRange n' -> n' = index.Get
            | _ -> false
        | Success _ -> false
    | Failure _ -> 
        false

[<Property>]
let ``get works``(length: NonNegativeInt, value: bool) =
    let n = length.Get

    match value |> create n with
    | Success result ->
        let checkBit i =
            let vector = result.Data.[i/64]
            let offset = i%64
            if value then 0L <> (vector &&& (1L <<< offset))
            else 0L = (vector &&& (1L <<< offset))
        let allBitsOk = 
            [|0..n-1|] |> Array.fold (fun acc i -> acc && (checkBit i)) true
        allBitsOk
    | _ -> false