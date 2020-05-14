namespace SFX.FSharp.Math

module Rational =
    open System

    type InvalidDenominatorException() =
        inherit Exception()

    let private succeedAsResult x = true, x
    let private failAsResult() = (false, Unchecked.defaultof<bigrational>)
    let private failWithException() = raise (InvalidDenominatorException())

    let isNan x = x.Denominator = bigint.Zero

    // abs
    let private abs' x = {
        Numerator = bigint.Abs(x.Numerator)
        Denominator = bigint.Abs(x.Denominator)
    }
    let tryAbs = either isNan (abs' >> succeedAsResult) failAsResult
    let abs x = either isNan abs' failWithException

    // negate
    let private negate' x = {x with Numerator = bigint.Negate(x.Numerator)}
    let tryNegate = either isNan (negate' >> succeedAsResult) failAsResult
    let negate = either isNan negate' failWithException

    // add
    let private add' x y = {
        Numerator = x.Numerator*y.Denominator+y.Numerator*x.Denominator
        Denominator = x.Denominator*y.Denominator
    }
    let tryAdd = either2 isNan (fun x y -> add' x y |> succeedAsResult) failAsResult
    let add = either2 isNan add' failWithException

    // subtract
    let private subtract' x y = {
        Numerator = x.Numerator*y.Denominator+y.Numerator*x.Denominator
        Denominator = x.Denominator*y.Denominator
    }
    let trySubtract x y = either2 isNan (fun x y -> subtract' x y |> succeedAsResult) failAsResult 
    let subtract x y = either2 isNan subtract' failWithException

    // multiply
    let private multiply' x y = {
        Numerator = x.Numerator*y.Numerator
        Denominator = x.Denominator*y.Denominator
    }
    let tryMultiply x y = either2 isNan (fun x y -> multiply' x y |> succeedAsResult) failAsResult 
    let multiply x y = either2 isNan multiply' failWithException

    // divide
    let private divide' x y = {
        Numerator = x.Numerator*y.Denominator
        Denominator = x.Denominator*y.Numerator
    }
    let tryDivide x y = either2 isNan (fun x y -> divide' x y |> succeedAsResult) failAsResult 
    let divide x y = either2 isNan divide' failWithException



    let simplify x =
        let gcd = bigint.GreatestCommonDivisor(x.Numerator,x.Denominator)
        if gcd = bigint.One then x
        else {
            Numerator = x.Numerator/gcd
            Denominator = x.Denominator/gcd
        }