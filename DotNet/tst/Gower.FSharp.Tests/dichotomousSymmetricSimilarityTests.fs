namespace Gower.Tests

open Xunit
open FsCheck.Xunit
open SFX.Gower

[<Trait("Category", "Unit")>]
module dichotomousSymmetricSimilarityTests =
    [<Property>]
    let ``dichotomousSymmetricSimilarity works``(a: bool, b: bool) =
        let result = dichotomousSymmetricSimilarity a b
        match a, b with
        | true, true | false, false -> result = _1_1
        | true, false | false, true -> result = _0_1

