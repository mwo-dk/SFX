namespace Gower.Tests

open Xunit
open FsCheck.Xunit
open SFX.Gower

[<Trait("Category", "Unit")>]
module dichotomousAsymmetricSimilarityTests =
    [<Property>]
    let ``dichotomousAsymmetricSimilarity works``(a: bool, b: bool) =
        let result = dichotomousAsymmetricSimilarity a b
        match a, b with
        | true, true -> result = _1_1
        | true, false | false, true -> result = _0_1
        | _ -> result = _0_0