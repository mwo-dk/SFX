module zeroCreateTests

open FsCheck
open FsCheck.Xunit
open SFX.FSharp.ROP
open SFX.FSharp.BitHack

[<Property>]
let ``zeroCreate with negative length works`` (length: NegativeInt) =
    let n = length.Get
    match zeroCreate n with
    | Failure error -> 
        match error with
        | InvalidArrayLength n' -> n = n'
        | _ -> false
    | _ -> false
    
[<Property>]
let ``zeroCreate works`` (length: NonNegativeInt) =
    let n = length.Get
    match zeroCreate n with
    | Success result ->
        let lengthOk = n = result.Length
        let dataLength = result.Data |> Array.length
        let dataLengthOk = 
            if n = 0 then (dataLength = 0) 
            else (n-1)/64 + 1 = dataLength
        let dataOk =
            if dataLength = 0 then true
            else 
                let checkBit i =
                    match result |> get i with
                    | Success x -> x = false
                    | _ -> false
                [|0..n-1|] |> Array.fold (fun acc i -> acc && (i |> checkBit)) true
        lengthOk && dataLengthOk && dataOk
    | _ -> false