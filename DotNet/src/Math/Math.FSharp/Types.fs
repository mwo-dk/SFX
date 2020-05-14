namespace SFX.FSharp.Math

[<AutoOpen>]
module Types =
    type BigRational = {
        Numerator: bigint
        Denominator: bigint
    }
    type bigrational = BigRational
    type BigFloat = {
        Mantissa: bigint
        Exponent: bigint
    }
    type bigfloat = BigFloat
