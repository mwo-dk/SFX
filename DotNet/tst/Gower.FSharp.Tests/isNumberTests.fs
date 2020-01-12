namespace Gower.Tests

open Xunit
open FsCheck.Xunit
open SFX.Gower

[<Trait("Category", "Unit")>]
module isNumberTests =
    [<Property>]
    let ``isNumber works``(x: float) =
        let result = x |> isNumber
        match x with
        | Value -> result
        | _ -> result |> not

