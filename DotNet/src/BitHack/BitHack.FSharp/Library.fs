module SFX.BitHack

open SFX.ROP
open SFX.BitHack.CSharp

/// Represents the erros that can occur when accessing a BitArray
type BitAccessError =
/// The array length is invalid - must be non-negative
| InvalidArrayLength of int
/// The index is invalid, that is out of the range represented by the BitArray
| IndexOutOfRange of int
/// The indices are invalid that is out of the range represented by the BitArray
| IndicesOutOfRange of int*int
/// The two BitArrays represent incompatiple sizes.
| IncompatipleLengths of int*int

/// Alias for int64 array
type BitVector = int64 array
/// Represents an array of bits
type BitArray = {
    Length: int
    Data: BitVector
}

let private getArrayLength n div = if n > 0 then ((n-1)/div) + 1 else 0

/// Creates a BitArray of length n. If defaultValue is true, then all bits are
/// set, else cleared
let create n defaultValue =
    match n with
    | n when n < 0 -> n |> InvalidArrayLength |> fail
    | _ -> 
        let length = getArrayLength n 64
        {
            Length = n;
            Data = if defaultValue then Array.init length (fun _ -> ~~~0L) else  Array.zeroCreate length
        } |> succeed

/// Creates a BitArray of length n with all bits cleared
let zeroCreate n = false |> create n

/// Reads the n'th bit of the BitArray array
let get n array = 
    let struct (ok, result) = BitVectorHelpers.GetSafe(array.Data, array.Length, n)
    if ok then result |> succeed
    else n |> IndexOutOfRange |> fail

/// Sets the n'th bit of the BitArray array
let set n value array =
    let ok = BitVectorHelpers.SetSafe(array.Data, array.Length, n, value)
    if ok then () |> succeed
    else n |> IndexOutOfRange |> fail

/// Reads whether all bits in the range from to _to in the BitArray array are set
let getRange from _to array =
    let struct (ok, result) = BitVectorHelpers.GetRangeSafe(array.Data, array.Length, from, _to)
    if ok then result |> succeed
    else (from, _to) |> IndicesOutOfRange |> fail

/// Sets all bits in the range from from to _to in the BitArray array to value
let setRange from _to value array =
    let ok = BitVectorHelpers.SetRangeSafe(array.Data, array.Length, from, _to, value)
    if ok then () |> succeed
    else (from, _to) |> IndicesOutOfRange |> fail

/// Inverts all bits in the BitArray array
let invert array = 
    let data = Array.zeroCreate (getArrayLength array.Length 64)
    BitVectorHelpers.Not(array.Data, data)
    {array with Data = data}

let private bitwiseOp x y op =
    let xLength, yLength = x.Length, y.Length
    if xLength <> yLength then (xLength, yLength) |> IncompatipleLengths |> fail
    else
        let data = Array.zeroCreate xLength
        op(x.Data, y.Data, data)
        {Length = xLength; Data = data} |> succeed

/// Performs a bitwise AND operation on all positionally paired bits in x and y
let bitwiseAndOp x y = BitVectorHelpers.And |> bitwiseOp x y
/// Performs a bitwise OR operation on all positionally paired bits in x and y
let bitwiseOrOp x y = BitVectorHelpers.Or |> bitwiseOp x y
/// Performs a bitwise XOR operation on all positionally paired bits in x and y
let bitwiseXorOp x y = BitVectorHelpers.Xor |> bitwiseOp x y

module Operators =
    /// Inverts all bits in the BitArray
    let (~~~) = invert
    /// Performs a bitwise AND operation on all positionally paired bits
    let (&&&) = bitwiseAndOp
    /// Performs a bitwise OR operation on all positionally paired bits
    let (|||) = bitwiseOrOp
    /// Performs a bitwise XOR operation on all positionally paired bits
    let (^^^) = bitwiseXorOp
