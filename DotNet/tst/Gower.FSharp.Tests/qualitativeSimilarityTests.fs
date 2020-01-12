namespace Gower.Tests

open Xunit
open FsCheck.Xunit
open SFX.Gower

[<Trait("Category", "Unit")>]
module qualitativeSimilarityTests =
    [<Property>]
    let ``qualitativeSimilarity works``(a: int, b: int) =
        let result = qualitativeSimilarity a b
        if a = b then result = _1_1
        else result = _0_1

