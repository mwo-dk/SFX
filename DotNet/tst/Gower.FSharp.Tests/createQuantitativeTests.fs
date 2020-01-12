namespace Gower.Tests

open Xunit
open FsCheck.Xunit
open SFX.Gower
open Helpers

[<Trait("Category", "Unit")>]
module createQuantitativeTests =
    let f = createQuantitative quan

    [<Property>]
    let ``createQuantitative result works``(a: float, b: float, range: float) =
        let x, y = createWithValue a, createWithValue b
        let (value, ok) = f x y range
        match a, b, range with
        | Value, Value, r when r = 0.0 -> 
            System.Double.IsNaN(value) && (ok |> not)
        | Value, Value, Value -> 
            value = 1.0-abs(a-b)/range && ok
        | _ -> 
            System.Double.IsNaN(value) && (ok |> not)