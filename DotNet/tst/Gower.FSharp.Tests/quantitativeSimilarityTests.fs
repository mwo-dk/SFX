namespace Gower.Tests

open Xunit
open FsCheck.Xunit
open SFX.Gower

[<Trait("Category", "Unit")>]
module quantitativeSimilarityTests =
    [<Property>]
    let ``quantitativeSimilarity works when range is zero``(a: float, b: float) =
        match quantitativeSimilarity a b 0.0 with
        | (_, false) -> true
        | _ -> false

    [<Property>]
    let ``quantitativeSimilarity works``(a: float, b: float, range: float) =
        let (value, ok) = quantitativeSimilarity a b range
        match a, b, range with
        | Value, Value, Value ->
            if range = 0.0 then System.Double.IsNaN(value) && (ok |> not)
            else value = 1.0-abs(a-b)/range && ok
        | _ -> System.Double.IsNaN(value) && (ok |> not)
